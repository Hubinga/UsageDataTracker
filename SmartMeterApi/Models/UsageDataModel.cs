using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
    public class UsageDataModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Month must contain only letters.")]
        [StringLength(12, ErrorMessage = "Month must be a maximum of 12 characters long.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Value is required")]
        [Range(0, 10000, ErrorMessage = "Value must be between 0 and 10000")]
        public int Value { get; set; }
    }
}
