using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using EDUCACOOPERN.Data.Migrations;
using System.Linq;

namespace EDUCACOOPERN.Controllers;

public class CertificadosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CertificadosController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: Certificados
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cursosAprovados = _context.Matricula
            .Include(x => x.Aula)
            .ThenInclude(x => x.Curso)
            .Where(x => x.AlunoId.Equals(userId) && x.Status == EStatusMatricula.Aprovado)
            .Select(x => x.Aula.Curso.Nome.ToUpper())
            .ToList();

        var certificados = await _context.Certificado
            .Include(c => c.Usuario)
            .Where(x => x.UsuarioId.Equals(userId))
            .ToListAsync();

        var cursosSemCertificados = cursosAprovados.Where(x => !certificados.Any(y => y.Descricao.Equals(x.ToUpper()))).ToList();

        if (cursosSemCertificados.Any())
        {
            var cursosParaGerarCertificados = _context.Matricula
                .Include(x => x.Aula)
                .ThenInclude(x => x.Curso)
                .Where(x => x.AlunoId.Equals(userId) && x.Status == EStatusMatricula.Aprovado && cursosSemCertificados.Contains(x.Aula.Curso.Nome.ToUpper()))
                .Select(x => x.AulaId)
                .ToList();

            foreach(var aulaid in cursosParaGerarCertificados)
            {
                _ = GerarCertificado(userId, aulaid);
            }

            certificados = await _context.Certificado
                .Include(c => c.Usuario)
                .Where(x => x.UsuarioId.Equals(userId))
                .ToListAsync();
        }

        return View(certificados);
    }

    // GET: Certificados/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var certificado = await _context.Certificado
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (certificado == null)
        {
            return NotFound();
        }

        return View(certificado);
    }

    // GET: Certificados/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Certificados/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string descricao, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0 || string.IsNullOrEmpty(descricao))
        {
            return RedirectToAction(nameof(Create));
        }

        using (var stream = arquivo.OpenReadStream())
        {
            byte[] bytes = new byte[arquivo.Length];
            stream.Read(bytes, 0, (int)arquivo.Length);

            var certificado = new Certificado
            {
                Arquivo = Convert.ToBase64String(bytes),
                UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Descricao = descricao,
                Extencao = arquivo.ContentType.Split('/')[1]
            };

            _context.Add(certificado);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Certificados/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var certificado = await _context.Certificado
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (certificado == null)
        {
            return NotFound();
        }

        return View(certificado);
    }

    // POST: Certificados/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var certificado = await _context.Certificado.FindAsync(id);
        if (certificado != null)
        {
            _context.Certificado.Remove(certificado);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult GerarCertificado(string id, int idaula)
    {
        var aula = _context.Aulas
            .Include(x => x.Curso)
            .FirstOrDefault(x => x.Id == idaula);

        var usuario = _context.Usuario.FirstOrDefault(x => x.Id.Equals(id));

        var certificadoExisteste = _context.Certificado
            .Where(x => x.UsuarioId.Equals(id) && x.Descricao.Equals(aula.Curso.Nome.ToUpper()))
            .FirstOrDefault();

        if (certificadoExisteste != null)
        {
            return RedirectToAction("Details", new { id = certificadoExisteste.Id });
        }

        var nomeSaida = id + DateTime.Now.ToString("yyyyMMddhhmmss") + ".pdf";

        var caminhoPdfTemplate = Path.Combine(_env.WebRootPath, "pdf", "modelo_certificado.pdf");
        var caminhoPdfSaida = Path.Combine(_env.WebRootPath, "pdf", nomeSaida);

        using var reader = new PdfReader(caminhoPdfTemplate);
        using var stream = new FileStream(caminhoPdfSaida, FileMode.Create);
        using var stamper = new PdfStamper(reader, stream);

        var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        var bfBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        
        var cb = stamper.GetOverContent(1); // Assumindo que o nome deve ser inserido na primeira página
        var cbCurso = stamper.GetOverContent(1);
        var cbNome = stamper.GetOverContent(1);

        var texto1 = "A COOPERN - COOPERATIVA DE TRABALHO E DE SERVIÇOS DE ENFERMAGEM";
        var texto2 = "DO RIO GRANDE DO NORTE CNPJ 11.601.777/0001-28,";
        var texto3 = "CONFERE A CERTIFICAÇÃO RELATIVA AO CURSO DE";
        var texto4 = $"CONCLUÍDO EM {aula.DataFim.ToString("M").ToUpper()} DE {aula.DataFim:yyyy}";
        
        var tempo = (aula.DataFim - aula.DataInicio).Hours;
        var texto5 = $"COM CARGA HORÁRIA DE {tempo} HORAS.";

        cb.BeginText();
        cb.SetFontAndSize(bf, 16);

        cb.ShowTextAligned(Element.ALIGN_CENTER, texto1, 420, 350, 0);
        cb.ShowTextAligned(Element.ALIGN_CENTER, texto2, 420, 330, 0);
        cb.ShowTextAligned(Element.ALIGN_CENTER, texto3, 420, 310, 0);
        cb.EndText();
        
        cbCurso.BeginText();
        cbCurso.SetFontAndSize(bfBold, 16);
        cbCurso.ShowTextAligned(Element.ALIGN_CENTER, aula.Curso.Nome.ToUpper(), 420, 290, 0);
        cbCurso.EndText();

        cb.BeginText();
        cb.SetFontAndSize(bf, 16);
        cb.ShowTextAligned(Element.ALIGN_CENTER, texto4, 420, 270, 0);
        cb.ShowTextAligned(Element.ALIGN_CENTER, texto5, 420, 250, 0);
        cb.EndText();

        cbNome.SetFontAndSize(bfBold, 24);
        cbNome.SetRGBColorFill(63, 156, 147);
        cbNome.ShowTextAligned(Element.ALIGN_CENTER, usuario.FullName.ToUpper(), 420, 150, 0);
        cbNome.BeginText();
        cbNome.EndText();
        
        var cbLinha = stamper.GetOverContent(1);

        cbLinha.MoveTo(200, 145); // Ajuste as coordenadas conforme necessário
        cbLinha.LineTo(600, 145);
        cbLinha.Stroke();

        stamper.Close();

        var bytes = System.IO.File.ReadAllBytes(caminhoPdfSaida);

        var certificado = new Certificado
        {
            Arquivo = Convert.ToBase64String(bytes),
            UsuarioId = id,
            Descricao = aula.Curso.Nome.ToUpper(),
            Extencao = "pdf"
        };

        _context.Add(certificado);
        _context.SaveChanges();
        
        System.IO.File.Delete(caminhoPdfSaida);

        return RedirectToAction("Details", new { id = certificado.Id});
    }

    private bool CertificadoExists(int id)
    {
        return _context.Certificado.Any(e => e.Id == id);
    }
}
