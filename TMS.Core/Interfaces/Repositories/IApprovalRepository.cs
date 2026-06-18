using TMS.Core.Entities;

namespace TMS.Core.Interfaces.Repositories
{
    public interface IApprovalRepository : IGenericRepository<RequestApproval>
    {
        Task<IEnumerable<RequestApproval>> GetPendingByRoleAsync(int roleId);
        Task<IEnumerable<RequestApproval>> GetByRequestIdAsync(int requestId);
        Task<RequestApproval?> GetCurrentPendingAsync(int requestId, int approvalOrder);
    }
}
