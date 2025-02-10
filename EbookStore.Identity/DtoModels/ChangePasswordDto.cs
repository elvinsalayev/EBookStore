using System.ComponentModel.DataAnnotations;

namespace EbookStore.Identity.DtoModels
{
    public class ChangePasswordDto
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8,
         ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
