using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartMeterApi.Utility;
using SmartMeterApi.Data;
using SmartMeterApi.Models;

namespace SmartMeterApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly SmartMeterContext _context;
		private EmailService _emailService;

		public AuthController(IConfiguration config, SmartMeterContext context)
		{
			_config = config;
			_context = context;
			_emailService = new EmailService();
		}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // 1. Find user by email address
            User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginModel.Email);

            // 2. Check if user exists
            if (user == null)
            {
                // User not found
                return NotFound("User not found.");
            }

            // 3. Verify password using HashHelper
            if (!HashHelper.VerifyPasswordHash(loginModel.Password, user.PasswordSalt, user.PasswordHash))
            {
                // Password is incorrect
                return Unauthorized("Invalid login credentials.");
            }

            // Generate OTP and save it in user's record
            string otp = GenerateAndSaveOTP(user);
			// Send otp as email
			_emailService.SendOtpAsEmail(otp, user.Email);

            // Return OK response with the OTP information (for frontend to handle)
            return Ok(new { OTPSent = true, Email = user.Email });
        }

        // Method to generate OTP and save it in user's record
        private string GenerateAndSaveOTP(User user)
        {
            // Generate OTP
            var otp = GenerateRandomOTP();

            // Save OTP in user's record
            user.OtpCode = otp;
            user.OtpExpiration = DateTime.UtcNow.AddMinutes(5); //OTP expires in 5 minutes

            _context.SaveChanges(); // Save changes to database

            return otp; 
        }

        // Method to generate a random OTP
        private string GenerateRandomOTP()
        {
            // Generate a random 6-digit OTP
            Random random = new Random();
            int otpValue = random.Next(100000, 999999);
            return otpValue.ToString();
        }

        [HttpPost("verifyotp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOtpModel model)
        {
            // Find user by email address
            User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                // User not found
                return NotFound("User not found.");
            }

            // Check if OTP matches and is not expired
            if (user.OtpCode != model.OTPCode || user.OtpExpiration < DateTime.UtcNow)
            {
                // Invalid OTP or expired
                return BadRequest("Invalid OTP or OTP has expired.");
            }

            // Clear OTP fields after successful verification
            user.OtpCode = null;
            user.OtpExpiration = null;

            _context.SaveChanges(); // Save changes to database

            // Generate and return JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }


        [HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
		{
			// Check if user already exists
			if (await UserExists(registerModel.Email))
			{
				return Conflict("User already exists.");
			}

			// Create password hash and salt
			byte[] passwordHash, passwordSalt;
			HashHelper.CreatePasswordHash(registerModel.Password, out passwordHash, out passwordSalt);

            // Create new user object
            var user = new User
            {
                Firstname = registerModel.Firstname,
                Lastname = registerModel.Lastname,
                Email = registerModel.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User", // Set the default role for new users
                OtpCode = "",
                OtpExpiration = null
            };

			// Add user to database
			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return StatusCode(201); // Created
		}

		// Check if user with given email already exists
		private async Task<bool> UserExists(string email)
		{
			return await _context.Users.AnyAsync(u => u.Email == email);
		}

		// Generate JWT token for authenticated user
		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Use Id as NameIdentifier
                new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role) // Assuming role is stored in User object
				}),
				Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}	
}
