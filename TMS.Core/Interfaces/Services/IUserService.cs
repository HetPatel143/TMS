using TMS.Core.DTOs.Users;

namespace TMS.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserDto dto);
        Task ChangeUserRoleAsync(int userId, int roleId);
        Task ToggleUserStatusAsync(int userId);
    }
}
