using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EDUCACOOPERN.Models;

namespace EDUCACOOPERN.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EDUCACOOPERN.Models.AreaAtuacao> AreaAtuacao { get; set; } = default!;
    }
}
