using System.ComponentModel.DataAnnotations;

namespace SmartMeterApi.Models
{
	public class RegisterModel
	{
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
		public string Password { get; set; }
	}

}
