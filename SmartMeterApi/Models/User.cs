using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
	/// <summary>
	/// Represents user data.
	/// </summary>
	public class User
	{
		[Key]
		public int Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
		public string Email { get; set; }

		[Required]
		public byte[] PasswordHash { get; set; }

		[Required]
		public byte[] PasswordSalt { get; set; }

		[Required]
		public string Role { get; set; }

        public string OtpCode { get; set; } // New field for OTP
        public DateTime? OtpExpiration { get; set; }
    }
}
