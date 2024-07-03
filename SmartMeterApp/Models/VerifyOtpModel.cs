using System.ComponentModel.DataAnnotations;

namespace SmartMeterApp.Models
{
    public class VerifyOtpModel
    {
        [Required(ErrorMessage = "OTP is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
        public string Otp { get; set; }
        public string Email { get; set; }
    }
}
