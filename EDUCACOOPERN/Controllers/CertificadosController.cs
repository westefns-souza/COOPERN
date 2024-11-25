using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;

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

        var certificados = await _context.Certificado
            .Include(c => c.Usuario)
            .Where(x => x.UsuarioId.Equals(userId))
            .ToListAsync();

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

    public IActionResult GerarCertificado(string id, string nome, string curso)
    {
        nome = nome.ToUpper();
        curso = curso.ToUpper();
        
        var certificadoExisteste = _context.Certificado
            .Where(x => x.UsuarioId.Equals(id) && x.Descricao.Equals(curso))
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

        var bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        var cb = stamper.GetOverContent(1); // Assumindo que o nome deve ser inserido na primeira página

        cb.BeginText();
        cb.SetFontAndSize(bf, 24);

        cb.ShowTextAligned(Element.ALIGN_CENTER, curso, 420, 300, 0);
        cb.ShowTextAligned(Element.ALIGN_CENTER, nome, 420, 250, 0);

        cb.EndText();
        stamper.Close();

        var bytes = System.IO.File.ReadAllBytes(caminhoPdfSaida);

        var certificado = new Certificado
        {
            Arquivo = Convert.ToBase64String(bytes),
            UsuarioId = id,
            Descricao = curso,
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
