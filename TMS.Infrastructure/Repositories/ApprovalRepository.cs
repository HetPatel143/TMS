using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TMS.Core.Entities;
using TMS.Core.Interfaces.Repositories;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories
{
    public class ApprovalRepository : GenericRepository<RequestApproval>, IApprovalRepository
    {
        public ApprovalRepository(TMSDbContext context) : base(context) { }

        public async Task<IEnumerable<RequestApproval>> GetPendingByRoleAsync(int roleId)
        {
            return await _dbSet
                .Include(a => a.Request)
                    .ThenInclude(r => r.RequestType)
                .Include(a => a.Request)
                    .ThenInclude(r => r.CreatedByUser)
                .Include(a => a.Role)
                .Where(a => a.RoleId == roleId
                    && a.ApprovalStatus == "Pending"
                    && a.Request.CurrentStatus == "InApproval"
                    && a.ApprovalOrder == a.Request.CurrentApprovalOrder)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }
public async Task<bool> AnyAsync(Expression<Func<RequestApproval, bool>> predicate)
{
    return await _context.RequestApprovals.AnyAsync(predicate);
}
        public async Task<IEnumerable<RequestApproval>> GetByRequestIdAsync(int requestId)
        {
            return await _dbSet
                .Include(a => a.Request)
                .Include(a => a.Role)
                .Include(a => a.ApprovedByUser)
                .Where(a => a.RequestId == requestId)
                .OrderBy(a => a.ApprovalOrder)
                .ToListAsync();
        }

        public async Task<RequestApproval?> GetCurrentPendingAsync(int requestId, int approvalOrder)
        {
            return await _dbSet
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.RequestId == requestId
                    && a.ApprovalOrder == approvalOrder
                    && a.ApprovalStatus == "Pending");
        }
    }
}
