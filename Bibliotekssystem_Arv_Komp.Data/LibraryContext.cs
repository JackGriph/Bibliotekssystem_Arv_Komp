using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LibraryItem>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.UseTphMappingStrategy();
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(b => b.ISBN).IsUnique();
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.MemberId).IsUnique();
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.HasOne(l => l.Item)
                    .WithMany()
                    .HasForeignKey("LibraryItemId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Member)
                    .WithMany()
                    .HasForeignKey("MemberFkId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(l => l.IsOverdue);
                entity.Ignore(l => l.IsReturned);
            });
        }
    }
}
