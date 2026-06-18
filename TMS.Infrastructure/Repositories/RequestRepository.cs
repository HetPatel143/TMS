using Microsoft.EntityFrameworkCore;
using TMS.Core.DTOs.Requests;
using TMS.Core.Entities;
using TMS.Core.Interfaces.Repositories;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories
{
    public class RequestRepository : GenericRepository<Request>, IRequestRepository
    {
        public RequestRepository(TMSDbContext context) : base(context) { }

        public async Task<Request?> GetWithDetailsAsync(int requestId)
        {
            return await _dbSet
                .AsSplitQuery()
                .Include(r => r.RequestType)
                .Include(r => r.CreatedByUser)
                .Include(r => r.FieldValues)
                    .ThenInclude(fv => fv.Field)
                .Include(r => r.Approvals)
                    .ThenInclude(a => a.Role)
                .Include(r => r.Approvals)
                    .ThenInclude(a => a.ApprovedByUser)
                .Include(r => r.StatusHistories)
                    .ThenInclude(sh => sh.ChangedByUser)
                .Include(r => r.Attachments)
                    .ThenInclude(att => att.UploadedByUser)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        public async Task<IEnumerable<Request>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.RequestType)
                .Where(r => r.CreatedByUserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Request> Items, int TotalCount)> GetFilteredRequestsAsync(int userId, RequestFilterDto filter)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(r => r.RequestType)
                .Include(r => r.CreatedByUser)
                .Where(r => r.CreatedByUserId == userId);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.CurrentStatus == filter.Status);

            if (filter.RequestTypeId.HasValue)
                query = query.Where(r => r.RequestTypeId == filter.RequestTypeId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= filter.ToDate.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Request>> GetAllFilteredAsync(RequestFilterDto filter)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(r => r.RequestType)
                .Include(r => r.CreatedByUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.CurrentStatus == filter.Status);

            if (filter.RequestTypeId.HasValue)
                query = query.Where(r => r.RequestTypeId == filter.RequestTypeId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= filter.ToDate.Value);

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }
    }
}
