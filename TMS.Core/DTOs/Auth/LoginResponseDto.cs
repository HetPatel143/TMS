namespace TMS.Core.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime TokenExpiry { get; set; }
    }
}
