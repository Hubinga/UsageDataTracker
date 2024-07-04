using System.ComponentModel.DataAnnotations;

namespace SmartMeterApp.Models
{
    public class UsageDataModel
    {
        public int Year { get; set; }
        public string Month { get; set; }

        [Required(ErrorMessage = "Value is required")]
        [Range(0, 10000, ErrorMessage = "Value must be between 0 and 10000")]
        public int Value { get; set; }
    }
}
