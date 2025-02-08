using Microsoft.AspNetCore.Identity;

namespace EbookStore.Identity.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;

        public string FullName => $"{Name} {Surname}".Trim();
    }
}
