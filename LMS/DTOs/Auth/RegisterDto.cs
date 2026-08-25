using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100,MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
         public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        // note No Rle Feild here on purpose because we will assign the role in the backend based on the registration type (Student or Teacher)
        // every new user is forced to be a student by default, and if they want to become a teacher, they will have to request it and wait for approval from the admin.
        // the prevents a user form resgistering themselver as Admin

    }
}
