using System.ComponentModel.DataAnnotations;

namespace EbookStore.Identity.DtoModels
{
    public class EditProfileDto
    {
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Surname { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Current password is required for verification")]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
