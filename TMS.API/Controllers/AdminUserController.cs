using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.API.Models;
using TMS.Core.DTOs.Users;
using TMS.Core.Interfaces.Services;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = result.UserId },
                ApiResponse<UserResponseDto>.SuccessResponse(result, "User created successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserResponseDto>>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result, "User updated successfully."));
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] ChangeRoleRequest request)
        {
            await _userService.ChangeUserRoleAsync(id, request.RoleId);
            return Ok(ApiResponse.SuccessResponse("User role updated successfully."));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            await _userService.ToggleUserStatusAsync(id);
            return Ok(ApiResponse.SuccessResponse("User status toggled successfully."));
        }
    }

    public class ChangeRoleRequest
    {
        public int RoleId { get; set; }
    }
}
