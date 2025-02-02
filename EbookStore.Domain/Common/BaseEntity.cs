namespace EbookStore.Domain.Common
{
    namespace EbookStore.Domain.Common
    {
        public class BaseEntity
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public Guid? UpdatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsDeleted { get; set; } = false;
            public Guid? DeletedBy { get; set; }
            public DateTime? DeletedAt { get; set; }
        }
    }
}
