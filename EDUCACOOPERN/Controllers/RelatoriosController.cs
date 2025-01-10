using EDUCACOOPERN.Data;
using EDUCACOOPERN.Data.Migrations;
using EDUCACOOPERN.DTOs;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<IActionResult> Index(string? nome = null)
    {
        List<IGrouping<string?, UsuarioPDI>> PDIs = await _context.UsuarioPDIs
            .Include(p => p.PDI.PDICursos)
            .Include(p => p.Usuario.Matriculas)
            .GroupBy(p => p.Usuario.FullName)
            .ToListAsync();

        if (!string.IsNullOrEmpty(nome))
        {
            PDIs = PDIs.Where(x => x.Key != null && x.Key.ToUpper().StartsWith(nome.ToUpper())).ToList();
        }

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

    public async Task<IActionResult> AcompanhamentoPDIAnalitico(string? nome = null, int? servico = null)
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
                    .Include(x => x.Usuario.UsuarioAreaDeAtuacao)
                    .Include(x => x.PDI)
                    .Where(x => x.PDIId == pdi.Id)
                    .OrderBy(x => x.Usuario.FullName)
                    .ThenBy(x => x.PDI.Nome)
                    .ToList();

                if (!string.IsNullOrEmpty(nome))
                {
                    usuarioPDI = usuarioPDI
                        .Where(x => x.Usuario.FullName != null && x.Usuario.FullName.ToUpper().StartsWith(nome.ToUpper()))
                        .ToList();
                }

                if (servico != null && servico != 0)
                {
                    usuarioPDI = usuarioPDI
                        .Where(x => x.Usuario.UsuarioAreaDeAtuacao.Any(x => x.ServicosId == servico))
                        .ToList();
                }

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

        PreencherServicos();
        return View(sintetico);
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
}
