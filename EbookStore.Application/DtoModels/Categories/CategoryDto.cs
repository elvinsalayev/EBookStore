using EbookStore.Application.DtoModels.Ebooks;

namespace EbookStore.Application.DtoModels.Categories
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<EbookDto> Ebooks { get; set; } = new List<EbookDto>();

        public DateTime CreatedAt { get; set; }
    }

}
