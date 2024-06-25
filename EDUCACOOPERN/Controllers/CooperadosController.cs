using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using EDUCACOOPERN.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using X.PagedList;

namespace EDUCACOOPERN.Controllers;

[Authorize(Roles = "Coordenador")]
public class CooperadosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;

    public CooperadosController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore
    )
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? nome = null, int? pagina = 1)
    {
        var idusuarios = _context.UserRoles
            .Where(x => x.RoleId.Equals("3"))
            .Select(x => x.UserId)
            .ToList();

        IPagedList<ApplicationUser> usuarios;

        if (!nome.IsNullOrEmpty())
        {
            usuarios = await _userManager.Users
                .Where(x => idusuarios.Contains(x.Id))
                .OrderBy(x => x.Email)
                .Where(x => x.FullName.ToUpper().StartsWith(nome.ToUpper()))
                .OrderBy(x => x.FullName)
                .ToPagedListAsync((int)pagina, 20);
        }
        else
        {
            usuarios = await _userManager.Users
                .Where(x => idusuarios.Contains(x.Id))
                .OrderBy(x => x.FullName)
                .ToPagedListAsync((int)pagina, 20);
        }

        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new CooperadoViewModel()
        {
            Ativo = true,
            AreasAtuacao = [],
            PDIs = []
        };

        PreencherPDIs();
        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CooperadoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            PreencherPDIs();
            PreencherAreasDeAtuacao();
            viewModel.AreasAtuacao ??= [];
            viewModel.PDIs ??= [];
            return View(viewModel);
        }

        var user = Activator.CreateInstance<ApplicationUser>();

        user.PhoneNumber = viewModel.Celular;
        user.Email = viewModel.Email;
        user.UserName = viewModel.Email;
        user.EmailConfirmed = true;
        user.LockoutEnabled = false;
        user.FullName = viewModel.Nome;
        user.Registro = viewModel.Registro;
        user.NomeAlternativo = viewModel.NomeAlternativo;
        user.CelularAlternativo = viewModel.CelularAlternativo;

        var result = await _userManager.CreateAsync(user, "EducaCOOPERN$2024");

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Cooperado");

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new UsuarioAreaAtuacao { UsuarioId = user.Id, AreaAtuacaoId = x.Id }).ToList());
            if (viewModel.PDIs != null && viewModel.PDIs.Any())
            {
                await _context.AddRangeAsync(viewModel.PDIs.Select(x => new UsuarioPDI { UsuarioId = user.Id, PDIId = x.Id }).ToList());
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditAsync(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _userManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (usuario == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.UsuarioAreaAtuacao
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var pdis = _context.UsuarioPDIs
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.PDI)
            .ToList();

        var viewModel = new CooperadoViewModel()
        {
            Id = id,
            Nome = usuario.FullName,
            Celular = usuario.PhoneNumber,
            Email = usuario.Email,
            Registro = usuario.Registro,
            Ativo = usuario.LockoutEnabled,
            AreasAtuacao = areasAtuacao,
            PDIs = pdis,
            CelularAlternativo = usuario.CelularAlternativo,
            NomeAlternativo = usuario.NomeAlternativo,
        };

        PreencherPDIs();
        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditAsync(CooperadoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AreasAtuacao ??= [];
            viewModel.PDIs ??= [];
            PreencherPDIs();
            PreencherAreasDeAtuacao();
            return View(viewModel);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(viewModel.Id));

        user.PhoneNumber = viewModel.Celular;
        user.Email = viewModel.Email;
        user.UserName = viewModel.Email;
        user.EmailConfirmed = true;
        user.LockoutEnabled = viewModel.Ativo;
        user.FullName = viewModel.Nome;
        user.Registro = viewModel.Registro;
        user.NomeAlternativo = viewModel.NomeAlternativo;
        user.CelularAlternativo = viewModel.CelularAlternativo;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            _context.RemoveRange(await _context.UsuarioAreaAtuacao.Where(x => x.UsuarioId.Equals(user.Id)).ToListAsync());
            _context.RemoveRange(await _context.UsuarioPDIs.Where(x => x.UsuarioId.Equals(user.Id)).ToListAsync());

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new UsuarioAreaAtuacao { UsuarioId = user.Id, AreaAtuacaoId = x.Id }).ToList());

            if (viewModel.PDIs != null && viewModel.PDIs.Any())
            {
                await _context.AddRangeAsync(viewModel.PDIs.Select(x => new UsuarioPDI { UsuarioId = user.Id, PDIId = x.Id }).ToList());
            }
           
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.Id.Equals(id));

        if (usuario == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.UsuarioAreaAtuacao
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var pdis = _context.UsuarioPDIs
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.PDI)
            .ToList();

        var viewModel = new CooperadoViewModel()
        {
            Id = id,
            Nome = usuario.FullName,
            Celular = usuario.PhoneNumber,
            Email = usuario.Email,
            Registro = usuario.Registro,
            Ativo = usuario.LockoutEnabled,
            AreasAtuacao = areasAtuacao,
            PDIs = pdis,
            CelularAlternativo = usuario.CelularAlternativo,
            NomeAlternativo = usuario.NomeAlternativo,
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.Id.Equals(id));

        if (usuario == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.UsuarioAreaAtuacao
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var pdis = _context.UsuarioPDIs
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.PDI)
            .ToList();

        var viewModel = new CooperadoViewModel()
        {
            Id = id,
            Nome = usuario.FullName,
            Celular = usuario.PhoneNumber,
            Email = usuario.Email,
            Registro = usuario.Registro,
            Ativo = usuario.LockoutEnabled,
            AreasAtuacao = areasAtuacao,
            PDIs = pdis,
            CelularAlternativo = usuario.CelularAlternativo,
            NomeAlternativo = usuario.NomeAlternativo,
        };

        return View(viewModel);
    }

    // POST: AreaAtuacoes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.Id.Equals(id));

        var areasAtuacao = _context.UsuarioAreaAtuacao
            .Where(x => x.UsuarioId.Equals(id))
            .ToList();

        var pdis = _context.UsuarioPDIs
            .Where(x => x.UsuarioId.Equals(id))
            .ToList();

        if (usuario != null)
        {
            await _userManager.RemoveFromRoleAsync(usuario, "Cooperado");
            _context.RemoveRange(areasAtuacao);
            _context.RemoveRange(pdis);
            await _userManager.DeleteAsync(usuario);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    #region ViewBag

    private void PreencherAreasDeAtuacao()
    {
        var areasDeAtuacao = _context.AreaAtuacao
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nome })
            .ToList();

        ViewBag.AreaAtuacao = areasDeAtuacao;
    }

    private void PreencherPDIs()
    {
        var pdis = _context.PDIs
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nome })
            .ToList();

        ViewBag.PDIs = pdis;
    }

    #endregion
}
