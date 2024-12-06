using Microsoft.EntityFrameworkCore;
using SkiServiceManagement.Models;

namespace SkiServiceManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet für Benutzer
        public DbSet<Benutzer> Benutzer { get; set; }
        public DbSet<Serviceauftrag> Serviceauftraege { get; set; }
    }
}
