using TMS.Core.DTOs.Auth;

namespace TMS.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
