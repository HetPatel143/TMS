using TMS.Core.DTOs.Approvals;

namespace TMS.Core.Interfaces.Services
{
    public interface IApprovalService
    {
        Task<IEnumerable<ApprovalHistoryDto>> GetPendingApprovalsForRoleAsync(int roleId);
        Task ApproveRequestAsync(int requestId, int userId, string? comment);
        Task RejectRequestAsync(int requestId, int userId, string comment);
        Task<IEnumerable<ApprovalHistoryDto>> GetApprovalHistoryByRequestAsync(int requestId);
    }
}
