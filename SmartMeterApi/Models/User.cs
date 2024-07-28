using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
    /*Sicherheitsprinzipien: 
     - Eingabevalidierung: Alle Benutzereingaben werden überprüft, um sicherzustellen, dass sie den erwarteten Formaten entsprechen und keine schädlichen Daten enthalten.
     - Verhinderung von Injection-Angriffen: Durch die Verwendung von regulären Ausdrücken wird sichergestellt, dass nur erlaubte Zeichen akzeptiert werden.*/
    /// <summary>
    /// Represents user data.
    /// </summary>
    public class User
	{
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Firstname is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Firstname must contain only letters.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Lastname must contain only letters.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password hash is required.")]
        public byte[] PasswordHash { get; set; }

        [Required(ErrorMessage = "Password salt is required.")]
        public byte[] PasswordSalt { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Role must contain only letters.")]
        public string Role { get; set; }

        public string OtpCode { get; set; }
        public DateTime? OtpExpiration { get; set; }
    }
}
