using TMS.Core.DTOs.Requests;
using TMS.Core.Entities;

namespace TMS.Core.Interfaces.Repositories
{
    public interface IRequestRepository : IGenericRepository<Request>
    {
        Task<Request?> GetWithDetailsAsync(int requestId);
        Task<IEnumerable<Request>> GetByUserIdAsync(int userId);
        Task<(IEnumerable<Request> Items, int TotalCount)> GetFilteredRequestsAsync(int userId, RequestFilterDto filter);
        Task<IEnumerable<Request>> GetAllFilteredAsync(RequestFilterDto filter);
    }
}
