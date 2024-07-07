namespace SmartMeterApp.Models
{
    public class TokenData
    {
        public TokenData(string userId, string email, string role, DateTime expirationTime)
        {
            UserId = userId;
            Email = email;
            Role = role;
            ExpirationTime = expirationTime;
        }

        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
