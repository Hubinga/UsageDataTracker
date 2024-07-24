using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
    public class UserDataModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Firstname is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Firstname must contain only letters.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Lastname must contain only letters.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }
}
