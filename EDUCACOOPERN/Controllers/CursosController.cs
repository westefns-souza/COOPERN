using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using EDUCACOOPERN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Cursos.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);

        if (curso == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.CursoAreaAtuacoes
            .Include(x => x.AreaAtuacao)
            .Where(x => x.CursoId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var viewModel = new CursoViewModel()
        {
            Id = curso.Id,
            Nome = curso.Nome,
            Descricao = curso.Descricao,
            AreasAtuacao = areasAtuacao,
            Ativo = curso.Ativo,
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        var viewModel = new CursoViewModel
        {
            Ativo = true,
        };

        viewModel.AreasAtuacao ??= [];

        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CursoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AreasAtuacao ??= [];
            PreencherAreasDeAtuacao();
            return View(viewModel);
        }

        var curso = new Curso
        {
            Nome = viewModel.Nome,
            Descricao = viewModel.Descricao,
            Ativo = viewModel.Ativo,
        };

        await _context.AddAsync(curso);
        await _context.SaveChangesAsync();
        await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new CursoAreaAtuacao { CursoId = curso.Id, AreaAtuacaoId = x.Id }).ToList());
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.CursoAreaAtuacoes
            .Include(x => x.AreaAtuacao)
            .Where(x => x.CursoId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var viewModel = new CursoViewModel()
        {
            Id = curso.Id,
            Nome = curso.Nome,
            Descricao = curso.Descricao,
            AreasAtuacao = areasAtuacao,
            Ativo = curso.Ativo,
        };

        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CursoViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            viewModel.AreasAtuacao ??= [];
            PreencherAreasDeAtuacao();
            return View(viewModel);
        }

        try
        {
            var curso = _context.Cursos.FirstOrDefault(x => x.Id == id);

            curso.Nome = viewModel.Nome;
            curso.Ativo = viewModel.Ativo;
            curso.Descricao = viewModel.Descricao;

            _context.Update(curso);
            _context.RemoveRange(await _context.CursoAreaAtuacoes.Where(x => x.CursoId.Equals(curso.Id)).ToListAsync());

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new CursoAreaAtuacao { CursoId = curso.Id, AreaAtuacaoId = x.Id }).ToList());
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CursoExists(viewModel.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos.FirstOrDefaultAsync(m => m.Id == id);
        if (curso == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.CursoAreaAtuacoes
            .Include(x => x.AreaAtuacao)
            .Where(x => x.CursoId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var viewModel = new CursoViewModel()
        {
            Id = curso.Id,
            Nome = curso.Nome,
            Descricao = curso.Descricao,
            AreasAtuacao = areasAtuacao
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso != null)
        {
            _context.RemoveRange(await _context.CursoAreaAtuacoes.Where(x => x.CursoId.Equals(curso.Id)).ToListAsync());
            _context.Remove(curso);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CursoExists(int id)
    {
        return _context.Cursos.Any(e => e.Id == id);
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
