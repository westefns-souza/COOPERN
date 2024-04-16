using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EDUCACOOPERN.Controllers;

public class CertificadosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CertificadosController(ApplicationDbContext context)
    {
        _context = context;
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var certificado = await _context.Certificado
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId.Equals(userId));

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

    private bool CertificadoExists(int id)
    {
        return _context.Certificado.Any(e => e.Id == id);
    }
}
