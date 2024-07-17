using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Firstname is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Firstname can only contain letters.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Lastname can only contain letters.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must beat least 12 characters.")]
        [RegularExpression(@"^(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?/~])(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{12,}$",
            ErrorMessage = "Password must contain at least one special character, one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; }
    }
}
