using Microsoft.Extensions.Configuration;
using TMS.Core.DTOs.Auth;
using TMS.Core.Exceptions;
using TMS.Core.Interfaces.Repositories;
using TMS.Core.Interfaces.Services;
using TMS.Infrastructure.Helpers;

namespace TMS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            INotificationService notificationService,
            JwtHelper jwtHelper,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _notificationService = notificationService;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedException("Your account has been deactivated. Contact admin.");

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid email or password.");

            var fullName = $"{user.FirstName} {user.LastName}";
            var token = _jwtHelper.GenerateToken(user.UserId, user.Email, user.Role.RoleName, fullName, user.RoleId);
            var tokenExpiry = _jwtHelper.GetTokenExpiry();

            return new LoginResponseDto
            {
                UserId = user.UserId,
                FullName = fullName,
                Email = user.Email,
                RoleName = user.Role.RoleName,
                Token = token,
                TokenExpiry = tokenExpiry
            };
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new ValidationException("New password and confirmation do not match.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new ValidationException("Current password is incorrect.");

            user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return;

            var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "").Replace("/", "").Replace("=", "");

            user.PasswordHash = $"RESET:{resetToken}:{user.PasswordHash}";
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var resetLink = $"{_configuration["App:BaseUrl"]}/reset-password?token={resetToken}";
            await _notificationService.SendEmailAsync(
                user.Email,
                "Password Reset Request",
                $"<p>Hello {user.FirstName},</p>" +
                $"<p>Click the link below to reset your password:</p>" +
                $"<p><a href=\"{resetLink}\">{resetLink}</a></p>" +
                $"<p>This link will expire shortly.</p>" +
                $"<p>If you did not request this, please ignore this email.</p>");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new ValidationException("New password and confirmation do not match.");

            var allUsers = await _userRepository.GetAllAsync();
            var user = allUsers.FirstOrDefault(u =>
                u.PasswordHash.StartsWith("RESET:") &&
                u.PasswordHash.Split(':')[1] == dto.Token);

            if (user == null)
                throw new ValidationException("Invalid or expired reset token.");

            user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}
