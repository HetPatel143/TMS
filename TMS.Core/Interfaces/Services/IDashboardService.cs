using TMS.Core.DTOs.Dashboard;

namespace TMS.Core.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<EmployeeDashboardDto> GetEmployeeDashboardAsync(int userId);
       Task<ApproverDashboardDto> GetApproverDashboardAsync(int userId, int roleId);

        Task<AdminDashboardDto> GetAdminDashboardAsync();
    }
}
