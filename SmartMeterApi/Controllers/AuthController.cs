using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly ILogger<AuthController> _logger;
        private readonly SmartMeterContext _context;
        private EmailService _emailService;

		public AuthController(SmartMeterContext context, ILogger<AuthController> logger)
		{
			_context = context;
			_emailService = new EmailService();
            _logger = logger;
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
                _logger.LogInformation($"Login process started for {loginModel.Email}.");
                // 1. Find user by email address
                User? user = await FindUserByEmail(loginModel.Email);

                // 2. Check if user exists
                if (user == null)
                {
                    _logger.LogInformation("User not found.");
                    return NotFound("User not found.");
                }

                // 3. Verify password
                if (!HashHelper.VerifyPasswordHash(loginModel.Password, user.PasswordSalt, user.PasswordHash))
                {
                    _logger.LogInformation("Invalid login credentials.");
                    return Unauthorized("Invalid login credentials.");
                }

                /*Sicherheitsprinzip:
                 - 2FA: Senden eines otp an die Email des Nutzers
                 - Besserer Schutz, da Angreifer eine weitere Hürde überwinden muss.*/

                //4. Generate OTP and save it in user's record
                string otp = GenerateAndSaveOTP(user);

                // 5. Send otp as email
                _emailService.SendOtpAsEmail(otp, user.Email);

                // 6. Return OK response with the OTP information (for frontend to handle)
                _logger.LogInformation("Login process was successful.");
                return Ok(new { OTPSent = true, Email = loginModel.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login process failed: {ex.Message}.");
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
            user.OtpCode = otp;
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
                _logger.LogInformation($"OTP verification process started for {model.Email}.");
                // 1. Find user by email address
                User? user = await FindUserByEmail(model.Email);

                if (user == null)
                {
                    _logger.LogInformation("User not found.");
                    return NotFound("User not found.");
                }

                // 2. Check if OTP matches and is not expired
                if (user.OtpCode != model.Otp || user.OtpExpiration < DateTime.UtcNow)
                {
                    _logger.LogInformation("Invalid OTP or OTP has expired.");
                    return BadRequest("Invalid OTP or OTP has expired.");
                }

                /*Sicherheitsprinzip:
                 - richtige Verwwendung eines one-time-password
                 - Löschen des Codes aus der User Tabelle, um sicherzustellen, dass der Code wirklich nur einmal verwendet werden kann.*/

                // 3. Clear OTP fields after successful verification
                user.OtpCode = "";
                user.OtpExpiration = null;
                // 4. Save changes to database
                _context.SaveChanges();

                // 5. Generate and return JWT token
                string token = GenerateJwtToken(user);
                _logger.LogInformation("OTP verification process was successful.");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"OTP verification process failed: {ex.Message}.");
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
                _logger.LogInformation($"Registration process started for {registerModel.Email}.");

                // 1. Check if user already exists
                if (await UserExists(registerModel.Email))
                {
                    _logger.LogInformation("User already exists.");
                    return Conflict("User already exists.");
                }

                // 2. Create password hash and salt
                byte[] passwordHash, passwordSalt;
                HashHelper.CreatePasswordHash(registerModel.Password, out passwordHash, out passwordSalt);

                // 3. Create new user object
                var user = new User
                {
                    // Encrypt sensitive user data
                    Firstname = EncryptionHelper.Encrypt(registerModel.Firstname),
                    Lastname = EncryptionHelper.Encrypt(registerModel.Lastname),
                    Email = EncryptionHelper.Encrypt(registerModel.Email),
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
                _logger.LogInformation("Registration process was successful.");
                return StatusCode(201); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration process failed: {ex.Message}.");
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
            string encryptedEmail = EncryptionHelper.Encrypt(email);
            return await _context.Users.AnyAsync(u => u.Email == encryptedEmail);
        }

        /// <summary>
        /// Find user with given email
        /// </summary>
        /// <param name="email"></param>
        private async Task<User?> FindUserByEmail(string email)
        {
            string encryptedEmail = EncryptionHelper.Encrypt(email);
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == encryptedEmail);
        }


        /*Sicherheitsprinzip:
           - sichere und effiziente Authentifizierung und Autorisierung von Benutzern innerhalb der API
           - die Rolle des Benutzers im JWT festgelegt ist und zusätzliche Berechtigungen durch manuelle Zuweisung vergeben werden können
         */

        /// <summary>
        /// Method to generate JWT Token for authenticated user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JWT Token</returns>
        private string GenerateJwtToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            /* Sicherheitsprinzip:
             - Verwendung eines geheimen Schlüssels zum Signieren des Tokens
             - Stellt sicher, dass das Token nicht manipuliert werden kann*/
            // Secret key for signing the token
            byte[] key = ConfigurationHelper.GetJwtSecretKey();

            // Token descriptor contains claims (user information), expiry time, and signing credentials
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
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
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // Write token as string
            return tokenHandler.WriteToken(token);
        }
    }	
}
