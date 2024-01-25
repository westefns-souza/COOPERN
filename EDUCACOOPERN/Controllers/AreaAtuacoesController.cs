using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

[Authorize(Roles = "Coordenador")]
public class AreaAtuacoesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AreaAtuacoesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.AreaAtuacao.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var areaAtuacao = await _context.AreaAtuacao.FirstOrDefaultAsync(m => m.Id == id);

        if (areaAtuacao == null)
        {
            return NotFound();
        }

        return View(areaAtuacao);
    }

    public IActionResult Create()
    {
        var areaAtuacao = new AreaAtuacao
        {
            Ativo = true,
        };

        return View(areaAtuacao);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nome,Ativo")] AreaAtuacao areaAtuacao)
    {
        if (ModelState.IsValid)
        {
            _context.Add(areaAtuacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(areaAtuacao);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var areaAtuacao = await _context.AreaAtuacao.FindAsync(id);
        if (areaAtuacao == null)
        {
            return NotFound();
        }

        return View(areaAtuacao);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Ativo")] AreaAtuacao areaAtuacao)
    {
        if (id != areaAtuacao.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(areaAtuacao);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaAtuacaoExists(areaAtuacao.Id))
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
        return View(areaAtuacao);
    }

    // GET: AreaAtuacoes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var areaAtuacao = await _context.AreaAtuacao.FirstOrDefaultAsync(m => m.Id == id);

        if (areaAtuacao == null)
        {
            return NotFound();
        }

        return View(areaAtuacao);
    }

    // POST: AreaAtuacoes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var areaAtuacao = await _context.AreaAtuacao.FindAsync(id);
        if (areaAtuacao != null)
        {
            _context.AreaAtuacao.Remove(areaAtuacao);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AreaAtuacaoExists(int id)
    {
        return _context.AreaAtuacao.Any(e => e.Id == id);
    }
}
