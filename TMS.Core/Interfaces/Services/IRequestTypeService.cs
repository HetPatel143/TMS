using TMS.Core.DTOs.RequestTypes;

namespace TMS.Core.Interfaces.Services
{
    public interface IRequestTypeService
    {
        Task<RequestTypeResponseDto> CreateRequestTypeAsync(int createdByUserId, CreateRequestTypeDto dto);
        Task<RequestTypeResponseDto> UpdateRequestTypeAsync(int requestTypeId, UpdateRequestTypeDto dto);
        Task<FieldResponseDto> AddFieldAsync(int requestTypeId, CreateFieldDto dto);
        Task<FieldResponseDto> UpdateFieldAsync(int requestTypeId, int fieldId, CreateFieldDto dto);
        Task SetWorkflowAsync(int requestTypeId, List<WorkflowStepDto> steps);
        Task<RequestTypeResponseDto> GetRequestTypeWithFieldsAndWorkflowAsync(int requestTypeId);
        Task<IEnumerable<RequestTypeResponseDto>> GetAllRequestTypesAsync();
        Task DeleteFieldAsync(int requestTypeId, int fieldId); 
    }
}
