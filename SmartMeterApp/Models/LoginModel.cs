namespace SmartMeterApi.Models
{
	using System.ComponentModel.DataAnnotations;

    /*Sicherheitsprinzipien: 
	 - Eingabevalidierung: Alle Benutzereingaben werden überprüft, um sicherzustellen, dass sie den erwarteten Formaten entsprechen und keine schädlichen Daten enthalten.
	 - Verhinderung von Injection-Angriffen: Durch die Verwendung von regulären Ausdrücken wird sichergestellt, dass nur erlaubte Zeichen akzeptiert werden.*/
    public class LoginModel
	{
		// Required annotation ensures that the email field cannot be null or empty
		[Required(ErrorMessage = "E-Mail is required")]
		// EmailAddress annotation validates that the input is a valid email address
		[EmailAddress(ErrorMessage = "Invalid email address")]
		[Display(Name = "E-Mail")]
		public string Email { get; set; }

		// Required annotation ensures that the password field cannot be null or empty
		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
