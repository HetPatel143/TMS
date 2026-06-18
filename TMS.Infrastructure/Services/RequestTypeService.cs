using AutoMapper;
using TMS.Core.DTOs.RequestTypes;
using TMS.Core.Entities;
using TMS.Core.Exceptions;
using TMS.Core.Interfaces.Repositories;
using TMS.Core.Interfaces.Services;

namespace TMS.Infrastructure.Services
{
    public class RequestTypeService : IRequestTypeService
    {
        private readonly IRequestTypeRepository _requestTypeRepository;
        private readonly IGenericRepository<RequestTypeField> _fieldRepository;
        private readonly IGenericRepository<ApprovalWorkflow> _workflowRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public RequestTypeService(
            IRequestTypeRepository requestTypeRepository,
            IGenericRepository<RequestTypeField> fieldRepository,
            IGenericRepository<ApprovalWorkflow> workflowRepository,
            IGenericRepository<Role> roleRepository,
            IMapper mapper)
        {
            _requestTypeRepository = requestTypeRepository;
            _fieldRepository = fieldRepository;
            _workflowRepository = workflowRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RequestTypeResponseDto> CreateRequestTypeAsync(int createdByUserId, CreateRequestTypeDto dto)
        {
            var existingTypes = await _requestTypeRepository.FindAsync(rt => rt.Name == dto.Name);
            if (existingTypes.Any())
                throw new Core.Exceptions.ValidationException("A request type with this name already exists.");

            var requestType = new RequestType
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _requestTypeRepository.AddAsync(requestType);
            await _requestTypeRepository.SaveChangesAsync();

            foreach (var fieldDto in dto.Fields)
            {
                var field = new RequestTypeField
                {
                    RequestTypeId = requestType.RequestTypeId,
                    FieldName = fieldDto.FieldName,
                    FieldLabel = fieldDto.FieldLabel,
                    FieldType = fieldDto.FieldType,
                    IsRequired = fieldDto.IsRequired,
                    DisplayOrder = fieldDto.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _fieldRepository.AddAsync(field);
            }

            foreach (var stepDto in dto.WorkflowSteps)
            {
                var workflow = new ApprovalWorkflow
                {
                    RequestTypeId = requestType.RequestTypeId,
                    ApprovalOrder = stepDto.ApprovalOrder,
                    RoleId = stepDto.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _workflowRepository.AddAsync(workflow);
            }

            await _requestTypeRepository.SaveChangesAsync();

            return await GetRequestTypeWithFieldsAndWorkflowAsync(requestType.RequestTypeId);
        }

        public async Task<RequestTypeResponseDto> UpdateRequestTypeAsync(int requestTypeId, UpdateRequestTypeDto dto)
        {
            var requestType = await _requestTypeRepository.GetByIdAsync(requestTypeId);
            if (requestType == null)
                throw new NotFoundException("RequestType", requestTypeId);

            var existingTypes = await _requestTypeRepository.FindAsync(
                rt => rt.Name == dto.Name && rt.RequestTypeId != requestTypeId);
            if (existingTypes.Any())
                throw new Core.Exceptions.ValidationException("A request type with this name already exists.");

            requestType.Name = dto.Name;
            requestType.Description = dto.Description;
            requestType.IsActive = dto.IsActive;
            requestType.UpdatedAt = DateTime.UtcNow;

            _requestTypeRepository.Update(requestType);
            await _requestTypeRepository.SaveChangesAsync();

            return await GetRequestTypeWithFieldsAndWorkflowAsync(requestTypeId);
        }

        public async Task<FieldResponseDto> AddFieldAsync(int requestTypeId, CreateFieldDto dto)
        {
            var requestType = await _requestTypeRepository.GetByIdAsync(requestTypeId);
            if (requestType == null)
                throw new NotFoundException("RequestType", requestTypeId);

            var field = new RequestTypeField
            {
                RequestTypeId = requestTypeId,
                FieldName = dto.FieldName,
                FieldLabel = dto.FieldLabel,
                FieldType = dto.FieldType,
                Options = dto.Options,
                IsRequired = dto.IsRequired,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _fieldRepository.AddAsync(field);
            await _fieldRepository.SaveChangesAsync();

            return _mapper.Map<FieldResponseDto>(field);
        }
public async Task DeleteFieldAsync(int requestTypeId, int fieldId)
{
    var field = await _requestTypeRepository.GetFieldAsync(requestTypeId, fieldId);
    if (field == null)
        throw new NotFoundException("Field", fieldId);

    _fieldRepository.Delete(field);
    await _fieldRepository.SaveChangesAsync();
}

        public async Task<FieldResponseDto> UpdateFieldAsync(int requestTypeId, int fieldId, CreateFieldDto dto)
        {
            var field = await _fieldRepository.GetByIdAsync(fieldId);
            if (field == null || field.RequestTypeId != requestTypeId)
                throw new NotFoundException("Field", fieldId);

            field.FieldName = dto.FieldName;
            field.FieldLabel = dto.FieldLabel;
            field.FieldType = dto.FieldType;
            field.IsRequired = dto.IsRequired;
            field.DisplayOrder = dto.DisplayOrder;
            field.Options = dto.Options; 

            _fieldRepository.Update(field);
            await _fieldRepository.SaveChangesAsync();

            return _mapper.Map<FieldResponseDto>(field);
        }

        public async Task SetWorkflowAsync(int requestTypeId, List<WorkflowStepDto> steps)
        {
            var requestType = await _requestTypeRepository.GetWithFieldsAndWorkflowAsync(requestTypeId);
            if (requestType == null)
                throw new NotFoundException("RequestType", requestTypeId);

            foreach (var existingWorkflow in requestType.Workflows.ToList())
            {
                _workflowRepository.Delete(existingWorkflow);
            }
            await _workflowRepository.SaveChangesAsync();

            foreach (var step in steps.OrderBy(s => s.ApprovalOrder))
            {
                var role = await _roleRepository.GetByIdAsync(step.RoleId);
                if (role == null)
                    throw new NotFoundException("Role", step.RoleId);

                var workflow = new ApprovalWorkflow
                {
                    RequestTypeId = requestTypeId,
                    ApprovalOrder = step.ApprovalOrder,
                    RoleId = step.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _workflowRepository.AddAsync(workflow);
            }

            await _workflowRepository.SaveChangesAsync();
        }

        public async Task<RequestTypeResponseDto> GetRequestTypeWithFieldsAndWorkflowAsync(int requestTypeId)
        {
            var requestType = await _requestTypeRepository.GetWithFieldsAndWorkflowAsync(requestTypeId);
            if (requestType == null)
                throw new NotFoundException("RequestType", requestTypeId);

            return _mapper.Map<RequestTypeResponseDto>(requestType);
        }

        public async Task<IEnumerable<RequestTypeResponseDto>> GetAllRequestTypesAsync()
        {
            var requestTypes = await _requestTypeRepository.GetAllWithFieldsAndWorkflowAsync();
            return _mapper.Map<IEnumerable<RequestTypeResponseDto>>(requestTypes);
        }
    }
}
