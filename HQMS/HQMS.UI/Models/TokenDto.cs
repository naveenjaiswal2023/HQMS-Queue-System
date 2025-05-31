namespace HospitalQueueSystem.Web.Models
{
    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        //public int MyProperty { get; set; }
        //public string RefreshToken { get; set; } = string.Empty;
    }
}
