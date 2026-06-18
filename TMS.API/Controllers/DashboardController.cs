using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.API.Models;
using TMS.Core.DTOs.Dashboard;
using TMS.Core.Interfaces.Services;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("employee")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetEmployeeDashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _dashboardService.GetEmployeeDashboardAsync(userId);
            return Ok(ApiResponse<EmployeeDashboardDto>.SuccessResponse(result));
        }

       [HttpGet("approver")]
[Authorize(Roles = "Manager,Finance,IT,HR")]
public async Task<IActionResult> GetApproverDashboard()
{
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var roleId = int.Parse(User.FindFirstValue("RoleId")!);
    var result = await _dashboardService.GetApproverDashboardAsync(userId, roleId);
    return Ok(ApiResponse<ApproverDashboardDto>.SuccessResponse(result));
}

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _dashboardService.GetAdminDashboardAsync();
            return Ok(ApiResponse<AdminDashboardDto>.SuccessResponse(result));
        }
    }
}
