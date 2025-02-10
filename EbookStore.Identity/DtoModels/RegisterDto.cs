using System.ComponentModel.DataAnnotations;

namespace EbookStore.Identity.DtoModels
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required")]
        [StringLength(50, ErrorMessage = "Surname cannot exceed 50 characters")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 6,
         ErrorMessage = "Username must be 6-20 characters")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8,
         ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;
    }
}