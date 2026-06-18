using TMS.Core.Entities;

namespace TMS.Core.Interfaces.Repositories
{
    public interface IRequestTypeRepository : IGenericRepository<RequestType>
    {
        Task<RequestType?> GetWithFieldsAndWorkflowAsync(int requestTypeId);
        Task<IEnumerable<RequestType>> GetAllWithFieldsAndWorkflowAsync();
        Task<IEnumerable<RequestType>> GetAllActiveAsync();
         Task<RequestTypeField?> GetFieldAsync(int requestTypeId, int fieldId);
    }
}
