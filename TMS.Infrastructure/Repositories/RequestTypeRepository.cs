using Microsoft.EntityFrameworkCore;
using TMS.Core.Entities;
using TMS.Core.Interfaces.Repositories;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories
{
    public class RequestTypeRepository : GenericRepository<RequestType>, IRequestTypeRepository
    {
        public RequestTypeRepository(TMSDbContext context) : base(context) { }

        public async Task<RequestType?> GetWithFieldsAndWorkflowAsync(int requestTypeId)
        {
            return await _dbSet
                .AsSplitQuery()
                .Include(rt => rt.CreatedByUser)
                .Include(rt => rt.Fields.OrderBy(f => f.DisplayOrder))
                .Include(rt => rt.Workflows.OrderBy(w => w.ApprovalOrder))
                    .ThenInclude(w => w.Role)
                .FirstOrDefaultAsync(rt => rt.RequestTypeId == requestTypeId);
        }

        public async Task<IEnumerable<RequestType>> GetAllWithFieldsAndWorkflowAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .Include(rt => rt.CreatedByUser)
                .Include(rt => rt.Fields.OrderBy(f => f.DisplayOrder))
                .Include(rt => rt.Workflows.OrderBy(w => w.ApprovalOrder))
                    .ThenInclude(w => w.Role)
                .OrderBy(rt => rt.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestType>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(rt => rt.CreatedByUser)
                .Where(rt => rt.IsActive)
                .OrderBy(rt => rt.Name)
                .ToListAsync();
        }
        public async Task<RequestTypeField?> GetFieldAsync(int requestTypeId, int fieldId)
{
    return await _context.RequestTypeFields
        .FirstOrDefaultAsync(f => f.FieldId == fieldId 
                               && f.RequestTypeId == requestTypeId);
}
    }
}
