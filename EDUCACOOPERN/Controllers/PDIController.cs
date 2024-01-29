using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using EDUCACOOPERN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

public class PDIController : Controller
{
    private readonly ApplicationDbContext _context;

    public PDIController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.PDIs.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pdi = await _context.PDIs.FirstOrDefaultAsync(m => m.Id == id);

        if (pdi == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.PDIAreaAtuacoes
            .Include(x => x.AreaAtuacao)
            .Where(x => x.PDIId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var viewModel = new PDIViewModel()
        {
            Id = pdi.Id,
            Nome = pdi.Nome,
            AreasAtuacao = areasAtuacao,
            Ativo = pdi.Ativo,
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        var viewModel = new PDIViewModel
        {
            Ativo = true,
        };

        viewModel.AreasAtuacao ??= [];

        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PDIViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.AreasAtuacao ??= [];
            PreencherAreasDeAtuacao();
            return View(viewModel);
        }

        var pdi = new PDI
        {
            Nome = viewModel.Nome,
            Ativo = viewModel.Ativo,
        };

        await _context.AddAsync(pdi);
        await _context.SaveChangesAsync();
        await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new PDIAreaAtuacao { PDIId = pdi.Id, AreaAtuacaoId = x.Id }).ToList());
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var curso = await _context.PDIs.FindAsync(id);
        if (curso == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.PDIAreaAtuacoes
            .Include(x => x.AreaAtuacao)
            .Where(x => x.PDIId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var viewModel = new PDIViewModel()
        {
            Id = curso.Id,
            Nome = curso.Nome,
            AreasAtuacao = areasAtuacao,
            Ativo = curso.Ativo,
        };

        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PDIViewModel viewModel)
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
            var pdi = _context.PDIs.FirstOrDefault(x => x.Id == id);

            pdi.Nome = viewModel.Nome;
            pdi.Ativo = viewModel.Ativo;

            _context.Update(pdi);
            _context.RemoveRange(await _context.PDIAreaAtuacoes.Where(x => x.PDIId.Equals(pdi.Id)).ToListAsync());
            await _context.SaveChangesAsync();

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new PDIAreaAtuacao { PDIId = pdi.Id, AreaAtuacaoId = x.Id }).ToList());
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

        var pdi = await _context.PDIs.FirstOrDefaultAsync(m => m.Id == id);
        if (pdi == null)
        {
            return NotFound();
        }

        var areasAtuacao = _context.PDIAreaAtuacoes
            .Include(x => x.AreaAtuacao)
            .Where(x => x.PDIId.Equals(id))
            .Select(x => x.AreaAtuacao)
            .ToList();

        var viewModel = new PDIViewModel()
        {
            Id = pdi.Id,
            Nome = pdi.Nome,
            AreasAtuacao = areasAtuacao,
            Ativo = pdi.Ativo,
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pdi = await _context.PDIs.FindAsync(id);
        if (pdi != null)
        {
            _context.RemoveRange(await _context.PDIAreaAtuacoes.Where(x => x.PDIId.Equals(pdi.Id)).ToListAsync());
            _context.Remove(pdi);
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
