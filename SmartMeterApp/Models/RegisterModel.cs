using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
	public class RegisterModel
	{
		[Required(ErrorMessage = "Email address is required.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[StringLength(20, MinimumLength = 12, ErrorMessage = "Password must be between 12 and 20 characters.")]
		public string Password { get; set; }
	}
}
