using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

[Authorize]
public class AulasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AulasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var aulas = await _context.Aulas
            .Include(a => a.Curso)
            .Include(a => a.Professor)
            .ToListAsync();

        return View(aulas);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aula = await _context.Aulas
            .Include(a => a.Curso)
            .Include(a => a.Professor)
            .Include(a => a.Matriculas.Where(x => !x.Status.Equals(EStatusMatricula.Cancelado)))
            .ThenInclude(x => x.Aluno)
            .Include(a => a.Custos)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aula == null)
        {
            return NotFound();
        }

        aula.DataInicio = aula.DataInicio.ToLocalTime();
        aula.DataFim = aula.DataFim.ToLocalTime();

        return View(aula);
    }

    public async Task<IActionResult> Lancar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aula = await _context.Aulas
            .Include(a => a.Curso)
            .Include(a => a.Professor)
            .Include(a => a.Matriculas.Where(x => !x.Status.Equals(EStatusMatricula.Cancelado)))
            .ThenInclude(x => x.Aluno)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aula == null)
        {
            return NotFound();
        }

        aula.DataInicio = aula.DataInicio.ToLocalTime();
        aula.DataFim = aula.DataFim.ToLocalTime();

        return View(aula);
    }

    [HttpPost]
    public async Task<IActionResult> Lancar(IList<Matricula> matriculas)
    {
        foreach (var matricula in matriculas)
        {
            if (matricula.Compareceu && matricula.NotaPos >= 6)
            {
                matricula.Status = EStatusMatricula.Aprovado;
            }
            else if (matricula.Compareceu && matricula.NotaPos <= 6)
            {
                matricula.Status = EStatusMatricula.Reprovado;
            }
            else
            {
                matricula.NotaPre = 0;
                matricula.NotaPos = 0;
                matricula.Status = EStatusMatricula.Reprovado;
            }

            _context.Update(matricula);
        }

        await _context.SaveChangesAsync();

        var aulaId = matriculas.Select(x => x.AulaId).FirstOrDefault();

        return RedirectToAction("Details", new { id = aulaId });
    }

    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> CreateAsync()
    {
        var viewModel = new Aula()
        {
            Custos = []
        };

        await PreencherViewBagCursoAsync();
        await PreencherViewBagProfessorAsync();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> Create(Aula aula)
    {
        if (ModelState.IsValid)
        {
            aula.DataInicio = aula.DataInicio.ToUniversalTime();
            aula.DataFim = aula.DataFim.ToUniversalTime();

            _context.Add(aula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        aula.Custos ??= [];

        await PreencherViewBagCursoAsync(aula.CursoId);
        await PreencherViewBagProfessorAsync(aula.ProfessorId);

        return View(aula);
    }

    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aula = await _context.Aulas
            .Include(x => x.Custos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (aula == null)
        {
            return NotFound();
        }

        aula.DataInicio = aula.DataInicio.ToLocalTime();
        aula.DataFim = aula.DataFim.ToLocalTime();

        await PreencherViewBagCursoAsync(aula.CursoId);
        await PreencherViewBagProfessorAsync(aula.ProfessorId);

        return View(aula);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> Edit(int id, Aula aula)
    {
        if (id != aula.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PreencherViewBagCursoAsync(aula.CursoId);
            await PreencherViewBagProfessorAsync(aula.ProfessorId);

            return View(aula);
        }

        try
        {
            aula.DataInicio = aula.DataInicio.ToUniversalTime();
            aula.DataFim = aula.DataFim.ToUniversalTime();
            _context.RemoveRange(await _context.Custos.Where(x => x.AulaId == aula.Id).ToListAsync());
            _context.Update(aula);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AulaExists(aula.Id))
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

    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aula = await _context.Aulas
            .Include(a => a.Curso)
            .Include(a => a.Professor)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aula == null)
        {
            return NotFound();
        }

        aula.DataInicio = aula.DataInicio.ToLocalTime();
        aula.DataFim = aula.DataFim.ToLocalTime();

        return View(aula);
    }

    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> Finalizar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aula = await _context.Aulas
            .Include(a => a.Curso)
            .Include(a => a.Professor)
            .Include(a => a.Matriculas.Where(x => !x.Status.Equals(EStatusMatricula.Cancelado)))
            .ThenInclude(x => x.Aluno)
            .Include(a => a.Custos)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aula == null)
        {
            return NotFound();
        }

        aula.DataInicio = aula.DataInicio.ToLocalTime();
        aula.DataFim = aula.DataFim.ToLocalTime();

        return View(aula);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var aula = await _context.Aulas.FindAsync(id);

        if (aula != null)
        {
            aula.Status = EStatusAula.Cancelada;
            _context.Update(aula);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Finalizar")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> FinalizarConfirmed(int id)
    {
        var aula = await _context.Aulas.FindAsync(id);

        if (aula != null)
        {
            aula.Status = EStatusAula.Finalizada;
            _context.Update(aula);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AulaExists(int id)
    {
        return _context.Aulas.Any(e => e.Id == id);
    }

    private async Task PreencherViewBagCursoAsync(int? id = null)
    {
        var cursos = await _context.Cursos
            .Where(x => x.Ativo)
            .ToListAsync();

        ViewData["CursoId"] = new SelectList(cursos, "Id", "Nome", id);
    }

    private async Task PreencherViewBagProfessorAsync(string? id = null)
    {
        var idusuarios = _context.UserRoles
            .Where(x => x.RoleId.Equals("2"))
            .Select(x => x.UserId)
            .ToList();

        var usuarios = await _userManager.Users
            .Where(x => idusuarios.Contains(x.Id))
            .OrderBy(x => x.FullName)
            .ToListAsync();

        ViewData["ProfessorId"] = new SelectList(usuarios, "Id", "FullName", id);
    }
}
