using Microsoft.EntityFrameworkCore;
using SkiServiceManagement.Models;

namespace SkiServiceManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Benutzer> Benutzer { get; set; }
        public DbSet<Serviceauftrag> Serviceauftraege { get; set; } // Stelle sicher, dass diese Zeile vorhanden ist.
    }
}
