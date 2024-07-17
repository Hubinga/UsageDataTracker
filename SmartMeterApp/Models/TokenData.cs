namespace SmartMeterApp.Models
{
    public class TokenData
    {
        public TokenData(int userId, string email, string role, DateTime expirationTime)
        {
            UserId = userId;
            Email = email;
            Role = role;
            ExpirationTime = expirationTime;
        }

        public int UserId { get; set; } = -1;
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
