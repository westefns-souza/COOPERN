using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using EDUCACOOPERN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

[Authorize(Roles = "Coordenador")]
public class ProfessoresController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfessoresController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarios = await _userManager.Users
            .Where(x => _context.UserRoles.Any(y => y.UserId == x.Id && y.RoleId == "2"))
            .OrderBy(x => x.Email)
            .ToListAsync();

        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new ProfessorViewModel()
        {
            Ativo = true,
            AreasAtuacao = [],
        };

        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(ProfessorViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            PreencherAreasDeAtuacao();
            viewModel.AreasAtuacao ??= [];
            return View(viewModel);
        }

        var user = new ApplicationUser
        {
            PhoneNumber = viewModel.Celular,
            Email = viewModel.Email,
            UserName = viewModel.Email,
            EmailConfirmed = true,
            LockoutEnabled = false,
            FullName = viewModel.Nome
        };

        var result = await _userManager.CreateAsync(user, "EducaCOOPERN$2024");

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Professor");
            
            var usuarioAreaAtuacao = viewModel.AreasAtuacao.Select(x => new UsuarioAreaAtuacao { UsuarioId = user.Id, AreaAtuacaoId = x.Id }).ToList();
            await _context.AddRangeAsync(usuarioAreaAtuacao);
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

        var areasAtuacao = await _context.UsuarioAreaAtuacao
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToListAsync();

        var viewModel = new ProfessorViewModel()
        {
            Id = id,
            Nome = usuario.FullName,
            Celular = usuario.PhoneNumber,
            Email = usuario.Email,
            Ativo = usuario.LockoutEnabled,
            AreasAtuacao = areasAtuacao,
        };

        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditAsync(ProfessorViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            PreencherAreasDeAtuacao();
            viewModel.AreasAtuacao ??= [];
            return View(viewModel);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(viewModel.Id));

        user.PhoneNumber = viewModel.Celular;
        user.Email = viewModel.Email;
        user.UserName = viewModel.Email;
        user.EmailConfirmed = true;
        user.LockoutEnabled = viewModel.Ativo;
        user.FullName = viewModel.Nome;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            _context.RemoveRange(await _context.UsuarioAreaAtuacao.Where(x => x.UsuarioId.Equals(user.Id)).ToListAsync());

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new UsuarioAreaAtuacao { UsuarioId = user.Id, AreaAtuacaoId = x.Id }).ToList());
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

        var areasAtuacao = await _context.UsuarioAreaAtuacao
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToListAsync();

        var viewModel = new ProfessorViewModel()
        {
            Id = id,
            Nome = usuario.FullName,
            Celular = usuario.PhoneNumber,
            Email = usuario.Email,
            Ativo = usuario.LockoutEnabled,
            AreasAtuacao = areasAtuacao,
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

        var areasAtuacao = await _context.UsuarioAreaAtuacao
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToListAsync();

        var viewModel = new ProfessorViewModel()
        {
            Id = id,
            Nome = usuario.FullName,
            Celular = usuario.PhoneNumber,
            Email = usuario.Email,
            Ativo = usuario.LockoutEnabled,
            AreasAtuacao = areasAtuacao,
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

        if (usuario != null)
        {
            await _userManager.RemoveFromRoleAsync(usuario, "Professor");
            _context.RemoveRange(areasAtuacao);
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

    #endregion
}
