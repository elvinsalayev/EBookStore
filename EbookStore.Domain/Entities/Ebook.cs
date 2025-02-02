using EbookStore.Domain.Common.EbookStore.Domain.Common;

namespace EbookStore.Domain.Entities
{
    public class Ebook : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Publisher { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int Pages { get; set; }
        public decimal Price { get; set; }

        public Guid? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public string CoverImageUrl { get; set; } = string.Empty;
    }
}
