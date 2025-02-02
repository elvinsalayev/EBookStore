using EbookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EbookStore.Persistence.Data
{

    public static class EbookStoreDbSeed
    {
        public static async Task InitDbAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EbookStoreDbContext>();

            await db.Database.MigrateAsync();

            await InitCategoriesAsync(db);
            await InitEbooksAsync(db);
        }

        private static async Task InitCategoriesAsync(EbookStoreDbContext db)
        {
            if (!await db.Categories.AnyAsync())
            {
                await db.Categories.AddRangeAsync(new[]
                {
                new Category { Name = "Fantasy" },
                new Category { Name = "Science Fiction" },
                new Category { Name = "Mystery" },
                new Category { Name = "Romance" },
                new Category { Name = "Horror" },
                new Category { Name = "History" },
                new Category { Name = "Technology" },
                new Category { Name = "Business" }
            });

                await db.SaveChangesAsync();
            }
        }

        private static async Task InitEbooksAsync(EbookStoreDbContext db)
        {
            if (!await db.Ebooks.AnyAsync())
            {
                var categories = await db.Categories.ToListAsync();

                var ebooks = new List<Ebook>
        {
            new Ebook
            {
                Title = "The Hobbit",
                Description = "A fantasy novel by J.R.R. Tolkien.",
                Author = "J.R.R. Tolkien",
                Publisher = "HarperCollins",
                Language = "English",
                Pages = 310,
                Price = 9.99m,
                CategoryId = categories.FirstOrDefault(c => c.Name == "Fantasy")?.Id,
                CoverImageUrl = "https://example.com/hobbit.jpg"
            },
            new Ebook
            {
                Title = "1984",
                Description = "A dystopian social science fiction novel and cautionary tale.",
                Author = "George Orwell",
                Publisher = "Secker & Warburg",
                Language = "English",
                Pages = 328,
                Price = 8.99m,
                CategoryId = categories.FirstOrDefault(c => c.Name == "Science Fiction")?.Id,
                CoverImageUrl = "https://example.com/1984.jpg"
            },
            new Ebook
            {
                Title = "Pride and Prejudice",
                Description = "A romantic novel by Jane Austen.",
                Author = "Jane Austen",
                Publisher = "T. Egerton",
                Language = "English",
                Pages = 279,
                Price = 7.99m,
                CategoryId = categories.FirstOrDefault(c => c.Name == "Romance")?.Id,
                CoverImageUrl = "https://example.com/pride-and-prejudice.jpg"
            }
        };

                await db.Ebooks.AddRangeAsync(ebooks);
                await db.SaveChangesAsync();
            }
        }


    }
}
