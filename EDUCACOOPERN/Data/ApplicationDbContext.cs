using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EDUCACOOPERN.Models;

namespace EDUCACOOPERN.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Usuario { get; set; } = default!;
    public DbSet<AreaAtuacao> AreaAtuacao { get; set; } = default!;
    public DbSet<UsuarioAreaAtuacao> UsuarioAreaAtuacao { get; set; } = default!;
}
