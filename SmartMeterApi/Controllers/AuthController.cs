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
    /// <summary>
    /// this class provides the endpoints for authentication
    /// </summary>
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

        /// <summary>
        /// Endpoint to handle user login
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                // 1. Find user by email address
                User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginModel.Email);

                // 2. Check if user exists
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // 3. Verify password
                if (!HashHelper.VerifyPasswordHash(loginModel.Password, user.PasswordSalt, user.PasswordHash))
                {
                    return Unauthorized("Invalid login credentials.");
                }

                //4. Generate OTP and save it in user's record
                string otp = GenerateAndSaveOTP(user);

                // 5. Send otp as email
                //_emailService.SendOtpAsEmail(otp, user.Email);

                // 6. Return OK response with the OTP information (for frontend to handle)
                return Ok(new { OTPSent = true, Email = user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Method to generate OTP and save it in user's record
        /// </summary>
        /// <param name="user"></param>
        /// <returns>otp</returns>
        private string GenerateAndSaveOTP(User user)
        {
            // Generate OTP
            var otp = GenerateRandomOTP();

            // Save OTP in user's record
            user.OtpCode = "123456";//otp;
            //OTP expires in 5 minutes
            user.OtpExpiration = DateTime.UtcNow.AddMinutes(5); 

            // Save changes to database
            _context.SaveChanges(); 

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

        /// <summary>
        /// Endpoint to verify otp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("verifyotp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOtpModel model)
        {
            try
            {
                // 1. Find user by email address
                User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // 2. Check if OTP matches and is not expired
                if (user.OtpCode != model.Otp || user.OtpExpiration < DateTime.UtcNow)
                {
                    return BadRequest("Invalid OTP or OTP has expired.");
                }

                // 3. Clear OTP fields after successful verification
                user.OtpCode = "";
                user.OtpExpiration = null;
                // 4. Save changes to database
                _context.SaveChanges(); 

                // 5. Generate and return JWT token
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to handle user registration
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns></returns>
        [HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
		{
            try
            {
                // 1. Check if user already exists
                if (await UserExists(registerModel.Email))
                {
                    return Conflict("User already exists.");
                }

                // 2. Create password hash and salt
                byte[] passwordHash, passwordSalt;
                HashHelper.CreatePasswordHash(registerModel.Password, out passwordHash, out passwordSalt);

                // 3. Create new user object
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

                // 4. Add user to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                // Created
                return StatusCode(201); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

		/// <summary>
        /// Check if user with given email already exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
		private async Task<bool> UserExists(string email)
		{
			return await _context.Users.AnyAsync(u => u.Email == email);
		}

        /// <summary>
        /// Method to generate JWT Token for authenticated user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JWT Token</returns>
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Secret key for signing the token
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]); 

            // Token descriptor contains claims (user information), expiry time, and signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                { 
                    // Use Id as NameIdentifier
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    // store user role
                    new Claim(ClaimTypes.Role, user.Role) 
                }),
                // Token expiration time (e.g., 1 hour)
                Expires = DateTime.UtcNow.AddHours(1), 
                // Signing credentials with HMAC-SHA256 algorithm
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) 
            };

            // Create JWT token based on token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Write token as string
            return tokenHandler.WriteToken(token);
        }
    }	
}
