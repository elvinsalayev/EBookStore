using EbookStore.Application.DtoModels.Categories;

namespace EbookStore.Application.DtoModels.Ebooks
{
    public class EbookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Publisher { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int Pages { get; set; }
        public decimal Price { get; set; }
        public Guid? CategoryId { get; set; }
        public virtual CategoryDto? Category { get; set; }

        public string CoverImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
