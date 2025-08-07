using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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

    [Authorize(Roles = "Coordenador, Professor")]
    public async Task<IActionResult> Index(int? ano, int? mes)
    {
        if (ano == null)
        {
            var hoje = DateTime.Now;
            ano = hoje.Year;
            mes = hoje.Month;
        }

        var aulas = await _context.Aulas
            .Include(a => a.Curso)
            .Include(a => a.Professor)
            .Where(x =>
                x.Status != EStatusAula.Cancelada
                && x.DataInicio.Year == ano
                && x.DataInicio.Month == mes)
            .OrderByDescending(x => x.DataInicio)
            .ToListAsync();

        if (User.IsInRole("Professor"))
        {
            aulas = aulas.Where(x => x.ProfessorId.Equals(_userManager.GetUserId(User))).ToList();
        }

        await PreencherAnoAsync((int)ano);
        await PreencherMesesAsync((int)mes);
        return View(aulas);
    }

    [Authorize(Roles = "Cooperado")]
    public async Task<IActionResult> Abertas()
    {
        var alunoEmail = User.Identity.Name;
        var alunoId = _context.Usuario
            .FirstOrDefault(x => x.UserName.Equals(alunoEmail))
            .Id;

        var aulas = await _context.Aulas
            .Include(a => a.Curso)
            .Include(a => a.Professor)
            .Include(a => a.Matriculas.Where(x => !x.Status.Equals(EStatusMatricula.Cancelado)))
            .Where(x => x.Status == EStatusAula.Aberta)
            .Where(x => x.Matriculas.All(x => !x.AlunoId.Equals(alunoId)))
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
    public async Task<IActionResult> Lancar(int id, IList<Matricula> matriculas)
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
                matricula.NotaPos = 0;
                matricula.Aproveitamento = 0;
                matricula.Status = EStatusMatricula.Reprovado;
            }

            _context.Update(matricula);
        }

        var aula = await _context.Aulas.FirstOrDefaultAsync(x => x.Id == id);
        aula.Status = EStatusAula.Realizada;

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id });
    }

    [Authorize(Roles = "Coordenador, Professor")]
    public async Task<IActionResult> CreateAsync()
    {
        var viewModel = new Aula()
        {
            Custos = [],
            DataInicio = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm")),
            DataFim = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm")),
        };

        await PreencherViewBagCursoAsync();
        await PreencherViewBagProfessorAsync();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Coordenador, Professor")]
    public async Task<IActionResult> Create(Aula aula)
    {
        if (ModelState.IsValid)
        {
            aula.DataInicio = aula.DataInicio.ToUniversalTime();
            aula.DataFim = aula.DataFim.ToUniversalTime();
            aula.Id = 0;
            _context.Add(aula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        aula.Custos ??= [];

        await PreencherViewBagCursoAsync(aula.CursoId);
        await PreencherViewBagProfessorAsync(aula.ProfessorId);

        return View(aula);
    }

    [Authorize(Roles = "Coordenador, Professor")]
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
    [Authorize(Roles = "Coordenador, Professor")]
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

    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> Custos(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aula = await _context.Aulas
            .Include(x => x.Custos)
            .Include(a => a.Professor)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (aula == null)
        {
            return NotFound();
        }

        aula.DataInicio = aula.DataInicio.ToLocalTime();
        aula.DataFim = aula.DataFim.ToLocalTime();

        return View(aula);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Coordenador")]
    public async Task<IActionResult> Custos(int id, Aula viewModel)
    {
        try
        {
            var aula = _context.Aulas.FirstOrDefault(x => x.Id == id);
            aula.Receita = viewModel.Receita;
            aula.Status = EStatusAula.Pendente;
            _context.RemoveRange(await _context.Custos.Where(x => x.AulaId == aula.Id).ToListAsync());
            _context.AddRange(viewModel.Custos.Select(x => new Custos { AulaId = viewModel.Id, Classificacao = x.Classificacao, Valor = x.Valor }));
            _context.Update(aula);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AulaExists(viewModel.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToAction(nameof(Details), new { id });
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

    private async Task PreencherAnoAsync(int ano)
    {
        var anosComAula = _context.Aulas
            .Select(x => x.DataInicio.Year)
            .Distinct()
            .OrderByDescending(x => x)
            .ToList();

        ViewBag.Anos = new SelectList(anosComAula, ano);
    }

    private async Task PreencherMesesAsync(int mes)
    {
        var mesesComAula = _context.Aulas
            .Select(x => x.DataInicio.Month)
            .Distinct()
            .OrderByDescending(x => x)
            .ToList();

        var meses = new List<object>();

        foreach (var x in mesesComAula)
        {
            var nome = "";

            switch (x)
            {
                case 1: nome = "Janeiro"; break;
                case 2: nome = "Fevereiro"; break;
                case 3: nome = "Março"; break;
                case 4: nome = "Abril"; break;
                case 5: nome = "Maio";break;
                case 6: nome = "Junho"; break;
                case 7: nome = "Julho"; break;
                case 8: nome = "Agosto"; break;
                case 9: nome = "Setembro"; break;
                case 10: nome = "Outubro"; break;
                case 11: nome = "Novembro"; break;
                case 12: nome = "Dezembro"; break;
            }

            meses.Add(new { mes = x, nome = nome });
        }

        ViewBag.Meses = new SelectList(meses, "mes", "nome", mes);
    }
}
