using EbookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EbookStore.Persistence.Data
{
    public class EbookStoreDbContext : DbContext
    {
        public EbookStoreDbContext(DbContextOptions<EbookStoreDbContext> options) : base(options) { }

        public DbSet<Ebook> Ebooks { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ebook>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Ebooks)
                .HasForeignKey(e => e.CategoryId)
                .IsRequired();
        }
    }
}
