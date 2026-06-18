using AutoMapper;
using TMS.Core.DTOs.Users;
using TMS.Core.Entities;
using TMS.Core.Exceptions;
using TMS.Core.Interfaces.Repositories;
using TMS.Core.Interfaces.Services;
using TMS.Infrastructure.Helpers;

namespace TMS.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IGenericRepository<Role> roleRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Core.Exceptions.ValidationException("A user with this email already exists.");

            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null)
                throw new NotFoundException("Role", dto.RoleId);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var createdUser = await _userRepository.GetWithRoleAsync(user.UserId);
            return _mapper.Map<UserResponseDto>(createdUser);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var usersWithRoles = new List<UserResponseDto>();
            foreach (var user in users)
            {
                var userWithRole = await _userRepository.GetWithRoleAsync(user.UserId);
                if (userWithRole != null)
                    usersWithRoles.Add(_mapper.Map<UserResponseDto>(userWithRole));
            }
            return usersWithRoles;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetWithRoleAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.UserId != userId)
                throw new Core.Exceptions.ValidationException("A user with this email already exists.");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var updatedUser = await _userRepository.GetWithRoleAsync(userId);
            return _mapper.Map<UserResponseDto>(updatedUser);
        }

        public async Task ChangeUserRoleAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new NotFoundException("Role", roleId);

            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task ToggleUserStatusAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}
