using SmartMeterApp.Models;
using System.IdentityModel.Tokens.Jwt;

namespace SmartMeterApp.Utility
{
    public class TokenHelper
    {
        /// <summary>
        /// Extract payload values from stored JWT Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>TokenData Object with extracted payload values</returns>
        public static TokenData? GetTokenData(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            string userId = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value;
            string email = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            string role = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;

            // Get the expiration time from the token
            string expClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "exp")?.Value;

            // If the expiration claim is present and not null, parse it
            DateTime expirationTime = DateTime.MinValue;
            if (long.TryParse(expClaim, out long expUnixTimeSeconds))
            {
                // Convert Unix timestamp to DateTime
                expirationTime = DateTimeOffset.FromUnixTimeSeconds(expUnixTimeSeconds).UtcDateTime;
            }

            return new TokenData(userId, email, role, expirationTime);
        }
    }
}
