using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=library.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurera relationer och constraints här
        }
    }
}
