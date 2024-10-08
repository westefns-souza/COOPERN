﻿using EDUCACOOPERN.Data;
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
    public async Task<IActionResult> Index(IndexCooperados viewModel)
    {
        if (viewModel == null)
        {
            viewModel = new IndexCooperados();
        }

        var idusuarios = _context.UserRoles
            .Where(x => string.IsNullOrEmpty(viewModel.Perfil) || x.RoleId.Equals(viewModel.Perfil))
            .Select(x => x.UserId)
            .ToList();

        if (!viewModel.Nome.IsNullOrEmpty())
        {
            viewModel.Usuario = await _userManager.Users
                .Where(x => idusuarios.Contains(x.Id))
                .OrderBy(x => x.Email)
                .Where(x => x.FullName.ToUpper().StartsWith(viewModel.Nome.ToUpper()))
                .OrderBy(x => x.FullName)
                .ToPagedListAsync(viewModel.Pagina ?? 1, 20);
        }
        else
        {
            viewModel.Usuario = await _userManager.Users
                .Where(x => idusuarios.Contains(x.Id))
                .OrderBy(x => x.FullName)
                .ToPagedListAsync(viewModel.Pagina ?? 1, 20);
        }

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new CooperadoViewModel()
        {
            Ativo = true,
            AreasAtuacao = [],
            PDIs = [],
            Formacoes = []
        };

        PreencherPDIs();
        PreencherAreasDeAtuacao();
        PreencherServicos();
        PreencherTiposFormacao();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CooperadoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            PreencherPDIs();
            PreencherAreasDeAtuacao();
            PreencherServicos();
            PreencherTiposFormacao();
            viewModel.AreasAtuacao ??= [];
            viewModel.PDIs ??= [];
            viewModel.Formacoes ??= [];
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
        user.Formacoes = viewModel.Formacoes;

        var result = await _userManager.CreateAsync(user, "EducaCOOPERN$2024");

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Cooperado");

            if (viewModel.Professor)
            {
                await _userManager.AddToRoleAsync(user, "Professor");
            }

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new UsuarioAreaAtuacao { UsuarioId = user.Id, AreaAtuacaoId = x.Id, ServicosId = x.ServicosId }).ToList());
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

        var usuario = await _userManager.Users
            .Include(x => x.Formacoes)
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (usuario == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.UsuarioAreaAtuacao
            .Include(x => x.Servicos)
            .Where(x => x.UsuarioId.Equals(id))
            .Select(x => new AreaAtuacaoViewModel{ 
                Id = x.AreaAtuacao.Id,
                Nome = x.AreaAtuacao.Nome,
                ServicosId = x.ServicosId,
                ServicosNome = x.Servicos != null ? x.Servicos.Nome : ""
            })
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
            Formacoes = usuario.Formacoes.ToList(),
        };

        PreencherPDIs();
        PreencherAreasDeAtuacao();
        PreencherServicos();
        PreencherTiposFormacao();
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
            PreencherServicos();
            PreencherTiposFormacao();
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
            _context.RemoveRange(user.Formacoes);

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new UsuarioAreaAtuacao { UsuarioId = user.Id, AreaAtuacaoId = x.Id, ServicosId = x.ServicosId }).ToList());
            await _context.AddRangeAsync(viewModel.Formacoes.Select(x => new Formacao { UsuarioId = user.Id, Tipo = x.Tipo, Nome = x.Nome }).ToList());

            if (!viewModel.Professor && _context.UserRoles
                .Where(x => x.UserId.Equals(user.Id) && x.RoleId.Equals("2"))
                .Any()
            )
            {
                await _userManager.RemoveFromRoleAsync(user, "Professor");
            }

            if (viewModel.Professor && !_context.UserRoles
                .Where(x => x.UserId.Equals(user.Id) && x.RoleId.Equals("2"))
                .Any()
            )
            {
                await _userManager.AddToRoleAsync(user, "Professor");
            }

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

        var usuario = await _context.Usuario
            .Include(x => x.Formacoes)
            .FirstOrDefaultAsync(m => m.Id.Equals(id));

        if (usuario == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.UsuarioAreaAtuacao
           .Include(x => x.Servicos)
           .Where(x => x.UsuarioId.Equals(id))
           .Select(x => new AreaAtuacaoViewModel
           {
               Id = x.AreaAtuacao.Id,
               Nome = x.AreaAtuacao.Nome,
               ServicosId = x.ServicosId,
               ServicosNome = x.Servicos != null ? x.Servicos.Nome : ""
           })
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
            Formacoes = usuario.Formacoes.ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuario
            .Include(x => x.Formacoes)
            .FirstOrDefaultAsync(m => m.Id.Equals(id));

        if (usuario == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.UsuarioAreaAtuacao
           .Include(x => x.Servicos)
           .Where(x => x.UsuarioId.Equals(id))
           .Select(x => new AreaAtuacaoViewModel
           {
               Id = x.AreaAtuacao.Id,
               Nome = x.AreaAtuacao.Nome,
               ServicosId = x.ServicosId,
               ServicosNome = x.Servicos != null ? x.Servicos.Nome : ""
           })
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
            Formacoes = usuario.Formacoes.ToList()
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

    private void PreencherServicos()
    {
        var servicos = _context.Servicos
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nome })
            .ToList();

        ViewBag.Servicos = servicos;
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

    private void PreencherTiposFormacao()
    {
        var tiposFormacao = Enum.GetValues(typeof(ETipoFormacao))
            .Cast<ETipoFormacao>()
            .Select(x => new SelectListItem { Value = ((int)x).ToString(), Text = x.ToString() })
            .ToList();

        ViewBag.TiposFormacao = tiposFormacao;
    }

    #endregion
}
