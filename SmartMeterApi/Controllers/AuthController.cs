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

		public AuthController(IConfiguration config, SmartMeterContext context)
		{
			_config = config;
			_context = context;
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
				return NotFound("Benutzer nicht gefunden.");
			}

			// 3. Verify password using HashHelper
			if (!HashHelper.VerifyPasswordHash(loginModel.Password, user.PasswordSalt, user.PasswordHash))
			{
				// Password is incorrect
				return Unauthorized("Ungültige Anmeldeinformationen.");
			}

			// Login successful
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
				return Conflict("Benutzer existiert bereits.");
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
				Role = "User" // Set the default role for new users
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
