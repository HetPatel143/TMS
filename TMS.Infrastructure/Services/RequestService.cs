using AutoMapper;
using TMS.Core.DTOs.Requests;
using TMS.Core.Entities;
using TMS.Core.Exceptions;
using TMS.Core.Interfaces.Repositories;
using TMS.Core.Interfaces.Services;

namespace TMS.Infrastructure.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IRequestTypeRepository _requestTypeRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IGenericRepository<RequestFieldValue> _fieldValueRepository;
        private readonly IGenericRepository<RequestStatusHistory> _statusHistoryRepository;
        private readonly IGenericRepository<RequestAttachment> _attachmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public RequestService(
            IRequestRepository requestRepository,
            IRequestTypeRepository requestTypeRepository,
            IApprovalRepository approvalRepository,
            IGenericRepository<RequestFieldValue> fieldValueRepository,
            IGenericRepository<RequestStatusHistory> statusHistoryRepository,
            IGenericRepository<RequestAttachment> attachmentRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IMapper mapper)
        {
            _requestRepository = requestRepository;
            _requestTypeRepository = requestTypeRepository;
            _approvalRepository = approvalRepository;
            _fieldValueRepository = fieldValueRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _attachmentRepository = attachmentRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        // public async Task<RequestResponseDto> CreateRequestAsync(int userId, CreateRequestDto dto)
        // {
        //     var requestType = await _requestTypeRepository.GetWithFieldsAndWorkflowAsync(dto.RequestTypeId);
        //     if (requestType == null)
        //         throw new NotFoundException("RequestType", dto.RequestTypeId);

        //     ValidateRequiredFields(requestType, dto.FieldValues);

        //     var request = new Request
        //     {
        //         RequestTypeId = dto.RequestTypeId,
        //         CreatedByUserId = userId,
        //         CurrentStatus = "Draft",
        //         CreatedAt = DateTime.UtcNow,
        //         UpdatedAt = DateTime.UtcNow
        //     };

        //     await _requestRepository.AddAsync(request);
        //     await _requestRepository.SaveChangesAsync();

        //     foreach (var fv in dto.FieldValues)
        //     {
        //         var fieldValue = new RequestFieldValue
        //         {
        //             RequestId = request.RequestId,
        //             FieldId = fv.FieldId,
        //             FieldValue = fv.FieldValue,
        //             CreatedAt = DateTime.UtcNow
        //         };
        //         await _fieldValueRepository.AddAsync(fieldValue);
        //     }

        //     await _fieldValueRepository.SaveChangesAsync();

        //     var result = await _requestRepository.GetWithDetailsAsync(request.RequestId);
        //     return _mapper.Map<RequestResponseDto>(result);
        // }
public async Task<RequestResponseDto> CreateRequestAsync(int userId, CreateRequestDto dto)
{
    var requestType = await _requestTypeRepository.GetWithFieldsAndWorkflowAsync(dto.RequestTypeId);
    if (requestType == null)
        throw new NotFoundException("RequestType", dto.RequestTypeId);

    ValidateRequiredFields(requestType, dto.FieldValues);
    var userRequestCount = (await _requestRepository.FindAsync(r => r.CreatedByUserId == userId)).Count();

    var request = new Request
    {
        RequestNumber = FormatRequestNumber(userId, userRequestCount + 1),
        RequestTypeId = dto.RequestTypeId,
        CreatedByUserId = userId,
        CurrentStatus = "Draft",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        // Add field values directly — no second SaveChanges needed
        FieldValues = dto.FieldValues.Select(fv => new RequestFieldValue
        {
            FieldId = fv.FieldId,
            FieldValue = fv.FieldValue,
            CreatedAt = DateTime.UtcNow
        }).ToList()
    };

    await _requestRepository.AddAsync(request);
    await _requestRepository.SaveChangesAsync(); // single save for both request + fields

    var result = await _requestRepository.GetWithDetailsAsync(request.RequestId);
    return _mapper.Map<RequestResponseDto>(result);
}
        public async Task<RequestResponseDto> SubmitRequestAsync(int requestId, int userId)
{
    var request = await _requestRepository.GetByIdAsync(requestId);
    if (request == null)
        throw new NotFoundException("Request", requestId);

    if (request.CreatedByUserId != userId)
        throw new UnauthorizedException("You can only submit your own requests.");

    if (request.CurrentStatus != "Draft")
        throw new Core.Exceptions.ValidationException("Only draft requests can be submitted.");

    var requestType = await _requestTypeRepository.GetWithFieldsAndWorkflowAsync(request.RequestTypeId);
    if (requestType == null)
        throw new NotFoundException("RequestType", request.RequestTypeId);

    var workflows = requestType.Workflows
        .Where(w => w.IsActive)
        .OrderBy(w => w.ApprovalOrder)
        .ToList();

    if (!workflows.Any())
        throw new Core.Exceptions.ValidationException("No approval workflow configured for this request type.");

    var oldStatus = request.CurrentStatus;
    request.CurrentStatus = "InApproval";
    request.CurrentApprovalOrder = workflows.First().ApprovalOrder;
    request.UpdatedAt = DateTime.UtcNow;

    var approvals = workflows.Select(workflow => new RequestApproval
    {
        RequestId = requestId,
        ApprovalOrder = workflow.ApprovalOrder,
        RoleId = workflow.RoleId,
        ApprovalStatus = "Pending",
        CreatedAt = DateTime.UtcNow
    }).ToList();

    foreach (var approval in approvals)
        await _approvalRepository.AddAsync(approval);

    var history = new RequestStatusHistory
    {
        RequestId = requestId,
        OldStatus = oldStatus,
        NewStatus = "InApproval",
        ChangedByUserId = userId,
        ChangeReason = "Request submitted for approval",
        ChangedAt = DateTime.UtcNow
    };
    await _statusHistoryRepository.AddAsync(history);

    _requestRepository.Update(request);

    await _requestRepository.SaveChangesAsync();

    

    // var result = await _requestRepository.GetWithDetailsAsync(requestId);
    // return _mapper.Map<RequestResponseDto>(result);
    return new RequestResponseDto
{
    RequestId = requestId,
    RequestNumber = request.RequestNumber,
    CurrentStatus = "InApproval"
};
}
        public async Task<RequestResponseDto> SaveDraftAsync(int userId, CreateRequestDto dto)
        {
            return await CreateRequestAsync(userId, dto);
        }

        public async Task<RequestResponseDto> EditRequestAsync(int requestId, int userId, CreateRequestDto dto)
        {
            var request = await _requestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                throw new NotFoundException("Request", requestId);

            if (request.CreatedByUserId != userId)
                throw new UnauthorizedException("You can only edit your own requests.");

            if (request.CurrentStatus != "Draft")
            {
                var hasApprovalAction = request.Approvals.Any(
                    a => a.ApprovalStatus != "Pending");
                if (hasApprovalAction)
                    throw new Core.Exceptions.ValidationException("Cannot edit a request after an approval action has been taken.");
            }

            var existingFieldValues = await _fieldValueRepository.FindAsync(fv => fv.RequestId == requestId);
            foreach (var existing in existingFieldValues)
            {
                _fieldValueRepository.Delete(existing);
            }
            await _fieldValueRepository.SaveChangesAsync();

            foreach (var fv in dto.FieldValues)
            {
                var fieldValue = new RequestFieldValue
                {
                    RequestId = requestId,
                    FieldId = fv.FieldId,
                    FieldValue = fv.FieldValue,
                    CreatedAt = DateTime.UtcNow
                };
                await _fieldValueRepository.AddAsync(fieldValue);
            }

            request.RequestTypeId = dto.RequestTypeId;
            request.UpdatedAt = DateTime.UtcNow;
            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            var result = await _requestRepository.GetWithDetailsAsync(requestId);
            return _mapper.Map<RequestResponseDto>(result);
        }

        public async Task CancelRequestAsync(int requestId, int userId)
        {
            var request = await _requestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                throw new NotFoundException("Request", requestId);

            if (request.CreatedByUserId != userId)
                throw new UnauthorizedException("You can only cancel your own requests.");

            var hasApprovalAction = request.Approvals.Any(
                a => a.ApprovalStatus != "Pending");
            if (hasApprovalAction)
                throw new Core.Exceptions.ValidationException("Cannot cancel a request after an approval action has been taken.");

            var oldStatus = request.CurrentStatus;
            request.CurrentStatus = "Cancelled";
            request.UpdatedAt = DateTime.UtcNow;

            await LogStatusChangeAsync(requestId, oldStatus, "Cancelled", userId, "Request cancelled by user");

            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();
        }

        public async Task<(IEnumerable<RequestResponseDto> Items, int TotalCount)> GetMyRequestsAsync(
            int userId, RequestFilterDto filter)
        {
            var (items, totalCount) = await _requestRepository.GetFilteredRequestsAsync(userId, filter);
            var dtos = _mapper.Map<IEnumerable<RequestResponseDto>>(items);
            return (dtos, totalCount);
        }

        public async Task<RequestResponseDto> GetRequestByIdAsync(int requestId, int userId)
        {
            var request = await _requestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                throw new NotFoundException("Request", requestId);

            return _mapper.Map<RequestResponseDto>(request);
        }

        public async Task<AttachmentResponseDto> UploadAttachmentAsync(
            int requestId, int userId, string fileName, string filePath, string? fileType)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new NotFoundException("Request", requestId);

            var attachment = new RequestAttachment
            {
                RequestId = requestId,
                FileName = fileName,
                FilePath = filePath,
                FileType = fileType,
                UploadedByUserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            await _attachmentRepository.AddAsync(attachment);
            await _attachmentRepository.SaveChangesAsync();

            var user = await _userRepository.GetByIdAsync(userId);

            return new AttachmentResponseDto
            {
                AttachmentId = attachment.AttachmentId,
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                FileType = attachment.FileType,
                UploadedByName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                UploadedAt = attachment.UploadedAt
            };
        }

        private void ValidateRequiredFields(RequestType requestType, List<RequestFieldValueDto> fieldValues)
        {
            var requiredFields = requestType.Fields
                .Where(f => f.IsRequired && f.IsActive)
                .ToList();

            var missingFields = requiredFields
                .Where(rf => !fieldValues.Any(fv => fv.FieldId == rf.FieldId && !string.IsNullOrWhiteSpace(fv.FieldValue)))
                .Select(rf => rf.FieldLabel)
                .ToList();

            if (missingFields.Any())
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "RequiredFields", missingFields.ToArray() }
                };
                throw new Core.Exceptions.ValidationException(
                    $"Missing required fields: {string.Join(", ", missingFields)}", errors);
            }
        }

        private async Task LogStatusChangeAsync(int requestId, string oldStatus, string newStatus, int userId, string? reason)
        {
            var history = new RequestStatusHistory
            {
                RequestId = requestId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedByUserId = userId,
                ChangeReason = reason,
                ChangedAt = DateTime.UtcNow
            };

            await _statusHistoryRepository.AddAsync(history);
        }

        private static string FormatRequestNumber(int userId, int userSequence)
        {
            return $"USR-{userId}-{userSequence:0000}";
        }
    }
}
