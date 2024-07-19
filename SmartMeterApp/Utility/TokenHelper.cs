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
            string exp = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "exp")?.Value;
            ///TODO: use expiration Time from Token
            DateTime expirationTime = DateTimeOffset.FromUnixTimeSeconds(1720185542).DateTime;

            return new TokenData(userId, email, role, expirationTime);
        }
    }
}
