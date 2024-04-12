using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

[Authorize]
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MatriculasController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var alunoEmail = User.Identity.Name;
        var alunoId = _context.Usuario
            .FirstOrDefault(x => x.UserName.Equals(alunoEmail))
            .Id;

        var matriculas = await _context.Matricula
            .Include(m => m.Aluno)
            .Include(m => m.Aula)
            .Include(m => m.Aula.Curso)
            .Include(m => m.Aula.Professor)
            .Where(x => x.AlunoId.Equals(alunoId) && x.Status != EStatusMatricula.Cancelado)
            .ToListAsync();

        return View(matriculas);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var matricula = await _context.Matricula
            .Include(m => m.Aluno)
            .Include(m => m.Aula)
            .Include(m => m.Aula.Curso)
            .Include(m => m.Aula.Professor)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (matricula == null)
        {
            return NotFound();
        }

        matricula.Aula.DataInicio = matricula.Aula.DataInicio.ToLocalTime();
        matricula.Aula.DataFim = matricula.Aula.DataFim.ToLocalTime();

        return View(matricula);
    }

    public async Task<IActionResult> CreateAsync()
    {
        await PreencherAlunosAsync();
        await PrencherAulasAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AulaId")] Matricula matricula)
    {
        matricula.AlunoId = _context.Usuario
            .FirstOrDefault(x => x.UserName.Equals(User.Identity.Name))
            .Id;

        if (!ModelState.IsValid)
        {
            await PreencherAlunosAsync(matricula.AlunoId);
            await PrencherAulasAsync(matricula.AulaId);
            return View(matricula);
        }

        matricula.Status = EStatusMatricula.Matriculado;
        _context.Add(matricula);
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = matricula.Id });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var matricula = await _context.Matricula.FindAsync(id);
        if (matricula == null)
        {
            return NotFound();
        }

        await PreencherAlunosAsync(matricula.AlunoId);
        await PrencherAulasAsync(matricula.AulaId);
        return View(matricula);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AulaId,AlunoId,NotaPre,NotaPos,Compareceu")] Matricula matricula)
    {
        if (id != matricula.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(matricula);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatriculaExists(matricula.Id))
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

        await PreencherAlunosAsync(matricula.AlunoId);
        await PrencherAulasAsync(matricula.AulaId);
        return View(matricula);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var matricula = await _context.Matricula
            .Include(m => m.Aluno)
            .Include(m => m.Aula)
            .Include(m => m.Aula.Curso)
            .Include(m => m.Aula.Professor)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (matricula == null)
        {
            return NotFound();
        }

        return View(matricula);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var matricula = await _context.Matricula.FindAsync(id);
        
        if (matricula != null)
        {
            matricula.Status = EStatusMatricula.Cancelado;
            _context.Update(matricula);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MatriculaExists(int id)
    {
        return _context.Matricula.Any(e => e.Id == id);
    }

    private async Task PreencherAlunosAsync(string? id = null)
    {
        var idusuarios = _context.UserRoles
            .Where(x => x.RoleId.Equals("3"))
            .Select(x => x.UserId)
            .ToList();

        var usuarios = await _userManager.Users
            .Where(x => idusuarios.Contains(x.Id))
            .OrderBy(x => x.Email)
            .ToListAsync();

        ViewData["AlunoId"] = new SelectList(usuarios, "Id", "Id", id);
    }

    private async Task PrencherAulasAsync(int? id = null)
    {
        var aulas = await _context.Aulas
            .ToListAsync();

        ViewData["AulaId"] = new SelectList(aulas, "Id", "ProfessorId", id);
    }
}
