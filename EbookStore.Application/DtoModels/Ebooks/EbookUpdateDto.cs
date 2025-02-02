namespace EbookStore.Application.DtoModels.Ebooks
{
    public class EbookUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int Pages { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
    }

}
