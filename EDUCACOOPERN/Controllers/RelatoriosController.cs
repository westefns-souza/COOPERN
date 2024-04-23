using EDUCACOOPERN.Data;
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
}
