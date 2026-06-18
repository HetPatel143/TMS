using TMS.Core.Entities;

namespace TMS.Core.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);
        Task<User?> GetWithRoleAsync(int userId);
    }
}
