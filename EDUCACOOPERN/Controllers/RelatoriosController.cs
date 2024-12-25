using EDUCACOOPERN.Data;
using EDUCACOOPERN.Data.Migrations;
using EDUCACOOPERN.DTOs;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

[Authorize(Roles = "Coordenador")]
public class RelatoriosController : Controller
{
    private readonly ApplicationDbContext _context;

    public RelatoriosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<IGrouping<string?, UsuarioPDI>> PDIs = await _context.UsuarioPDIs
            .Include(p => p.PDI.PDICursos)
            .Include(p => p.Usuario.Matriculas)
            .GroupBy(p => p.Usuario.FullName)
            .ToListAsync();

        return View(PDIs);
    }

    public async Task<IActionResult> Cursos()
    {
        List<IGrouping<string?, Models.Aula>> Aulas = await _context.Aulas
            .Include(c => c.Curso)
            .GroupBy(c => c.Curso.Nome)
            .ToListAsync();

        return View(Aulas);
    }

    public async Task<IActionResult> Receitas()
    {
        List<IGrouping<string?, Models.Aula>> Aulas = await _context.Aulas
            .Include(x => x.Custos)
            .Include(c => c.Curso)
            .GroupBy(c => c.Curso.Nome)
            .ToListAsync();

        return View(Aulas);
    }

    public async Task<IActionResult> AcompanhamentoPDISintetico()
    {
        var pdis = _context.PDIs
            .Include(x => x.PDICursos)
            .ThenInclude(x => x.Curso)
            .Where(x => x.Ativo)
            .ToList();

        var sintetico = new List<PDISintetico>();

        foreach (var pdi in pdis)
        {
            foreach (var curso in pdi.PDICursos)
            {
                var usuarioPDI = _context.UsuarioPDIs
                    .Include(x => x.Usuario.Matriculas)
                    .ThenInclude(x => x.Aula)
                    .Include(x => x.PDI)
                    .Where(x => x.PDIId == pdi.Id)
                    .ToList();

                var totalPDI = usuarioPDI
                    .Where(x => x.PDIId == pdi.Id)
                    .Count();

                foreach (var usuario in usuarioPDI)
                {
                    foreach (var matricula in usuario.Usuario.Matriculas)
                    {
                        if (matricula.Aula.CursoId == curso.CursoId && matricula.Status != EStatusMatricula.Reprovado && matricula.Status != EStatusMatricula.Cancelado)
                        {
                            sintetico.Add(new PDISintetico
                            {
                                IdPDI = pdi.Id,
                                NomePDI = pdi.Nome,
                                IdCurso = curso.Id,
                                NomeCurso = curso.Curso.Nome,
                                IdUsuario = usuario.UsuarioId,
                                NomeUsuario = usuario.Usuario.FullName,
                                Realizado = matricula.Status == EStatusMatricula.Aprovado ? 1 : 0,
                                Matriculado = matricula.Status == EStatusMatricula.Matriculado ? 1 : 0,
                                TotalPdi = totalPDI
                            });
                        }
                    }
                }
            }
        }

        return View(sintetico);
    }

    public async Task<IActionResult> AcompanhamentoPDIAnalitico()
    {
        var pdis = _context.PDIs
            .Include(x => x.PDICursos)
            .ThenInclude(x => x.Curso)
            .Where(x => x.Ativo)
            .ToList();

        var sintetico = new List<PDISintetico>();

        foreach (var pdi in pdis)
        {
            foreach (var curso in pdi.PDICursos)
            {
                var usuarioPDI = _context.UsuarioPDIs
                    .Include(x => x.Usuario.Matriculas)
                    .ThenInclude(x => x.Aula)
                    .Include(x => x.PDI)
                    .Where(x => x.PDIId == pdi.Id)
                    .ToList();

                var totalPDI = usuarioPDI
                    .Where(x => x.PDIId == pdi.Id)
                    .Count();

                foreach (var usuario in usuarioPDI)
                {
                    foreach (var matricula in usuario.Usuario.Matriculas)
                    {
                        if (matricula.Aula.CursoId == curso.CursoId && matricula.Status != EStatusMatricula.Reprovado && matricula.Status != EStatusMatricula.Cancelado)
                        {
                            sintetico.Add(new PDISintetico
                            {
                                IdPDI = pdi.Id,
                                NomePDI = pdi.Nome,
                                IdCurso = curso.Id,
                                NomeCurso = curso.Curso.Nome,
                                IdUsuario = usuario.UsuarioId,
                                NomeUsuario = usuario.Usuario.FullName,
                                Realizado = matricula.Status == EStatusMatricula.Aprovado ? 1 : 0,
                                Matriculado = matricula.Status == EStatusMatricula.Matriculado ? 1 : 0,
                                TotalPdi = totalPDI
                            });
                        }
                    }

                    if (usuario.Usuario.Matriculas == null || 
                        !usuario.Usuario.Matriculas.Any(x => x.Aula.CursoId == curso.CursoId && (x.Status != EStatusMatricula.Aprovado || x.Status == EStatusMatricula.Matriculado)))
                    {
                        sintetico.Add(new PDISintetico
                        {
                            IdPDI = pdi.Id,
                            NomePDI = pdi.Nome,
                            IdCurso = curso.Id,
                            NomeCurso = curso.Curso.Nome,
                            IdUsuario = usuario.UsuarioId,
                            NomeUsuario = usuario.Usuario.FullName,
                            NaoRealizado = 1,
                            Realizado = 0,
                            Matriculado = 0,
                            TotalPdi = totalPDI
                        });
                    }
                }
            }
        }

        return View(sintetico);
    }
}
