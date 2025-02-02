using EbookStore.Domain.Common.EbookStore.Domain.Common;

namespace EbookStore.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Ebook> Ebooks { get; set; } = new List<Ebook>();
    }
}
