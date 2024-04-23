using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using EDUCACOOPERN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EDUCACOOPERN.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var home = new HomeViewModel();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (User.IsInRole("Cooperado"))
        {
            var cooperado = new CooperadoHomeViewModel();

            cooperado.PDI = _context.UsuarioPDIs
                .Include(p => p.PDI)
                .Include(p => p.PDI.PDICursos)
                .ThenInclude(p => p.Curso)
                .FirstOrDefault(p => p.UsuarioId.Equals(userId))
                .PDI;

            var aulasConcluidas = _context.Matricula
                .Include(p => p.Aula)
                .Where(p => p.AlunoId.Equals(userId) && p.Status.Equals("Concluído"))
                .ToList();

            var cursosIds = aulasConcluidas.Select(p => p.Aula.CursoId).ToList();
            var cursosDoPDIIds = cooperado.PDI.PDICursos.Select(p => p.CursoId).ToList();

            home.CooperadoHome = cooperado;
            home.CooperadoHome.CursosRealizados = cursosIds;
            home.CooperadoHome.QuantidadeCursosRealizados = aulasConcluidas.Count();
            home.CooperadoHome.QuantidadeCursosNaoRealizados = cursosDoPDIIds.Where(x => !cursosIds.Contains(x)).Count();
        } else if (User.IsInRole("Coordenador")) {
            var administrador = new AdministradorViewModel();

            administrador.QuantidadeDeInscritosPorAreaDeAtuacao = _context.Matricula
                            .Include(p => p.Aula.Matriculas)
                            .Include(p => p.Aula.Curso.CursoAreaAtuacoes)
                            .GroupBy(p => p.Aula.Curso.CursoAreaAtuacoes.Select(x => x.AreaAtuacao.Nome).FirstOrDefault())
                            .Select(p => new { AreaAtuacao = p.Key, Quantidade = p.Count() })
                            .ToDictionary(p => p.AreaAtuacao, p => p.Quantidade);

            administrador.QuantidadeDeInscritosPorCurso = _context.Matricula
                            .Include(p => p.Aula.Matriculas)
                            .Include(p => p.Aula.Curso)
                            .GroupBy(p => p.Aula.Curso.Nome)
                            .Select(p => new { Curso = p.Key, Quantidade = p.Count() })
                            .ToDictionary(p => p.Curso, p => p.Quantidade);

            home.AdministradorHome = administrador;
        }

        return View(home);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
