using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
    /*Sicherheitsprinzipien: 
     - Eingabevalidierung: Alle Benutzereingaben werden überprüft, um sicherzustellen, dass sie den erwarteten Formaten entsprechen und keine schädlichen Daten enthalten.
     - Verhinderung von Injection-Angriffen: Durch die Verwendung von regulären Ausdrücken wird sichergestellt, dass nur erlaubte Zeichen akzeptiert werden.*/
    public class VerifyOtpModel
    {
        [Required(ErrorMessage = "OTP is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
        public string Otp { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }
}
