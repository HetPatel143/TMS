using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TMS.Core.DTOs.Approvals;
using TMS.Core.Entities;
using TMS.Core.Exceptions;
using TMS.Core.Interfaces.Repositories;
using TMS.Core.Interfaces.Services;

namespace TMS.Infrastructure.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGenericRepository<RequestStatusHistory> _statusHistoryRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public ApprovalService(
            IApprovalRepository approvalRepository,
            IRequestRepository requestRepository,
            IUserRepository userRepository,
            IGenericRepository<RequestStatusHistory> statusHistoryRepository,
            INotificationService notificationService,
            IMapper mapper)
        {
            _approvalRepository = approvalRepository;
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApprovalHistoryDto>> GetPendingApprovalsForRoleAsync(int roleId)
        {
            var approvals = await _approvalRepository.GetPendingByRoleAsync(roleId);
            return approvals.Select(a => new ApprovalHistoryDto
            {
                ApprovalId = a.ApprovalId,
                RequestId = a.RequestId,
                RequestNumber = a.Request.RequestNumber,
                RequestTypeName = a.Request.RequestType.Name,
                RequesterName = $"{a.Request.CreatedByUser.FirstName} {a.Request.CreatedByUser.LastName}",
                ApprovalOrder = a.ApprovalOrder,
                RoleName = a.Role.RoleName,
                ApprovalStatus = a.ApprovalStatus,
                ApprovedByName = null,
                Comments = a.Comments,
                ActionAt = a.ActionAt,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task ApproveRequestAsync(int requestId, int userId, string? comment)
        {
            var request = await _requestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                throw new NotFoundException("Request", requestId);

            if (request.CurrentStatus != "InApproval")
                throw new Core.Exceptions.ValidationException("This request is not in the approval process.");

            if (!request.CurrentApprovalOrder.HasValue)
                throw new Core.Exceptions.ValidationException("No active approval step found.");

            var user = await _userRepository.GetWithRoleAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            var currentApproval = await _approvalRepository.GetCurrentPendingAsync(
                requestId, request.CurrentApprovalOrder.Value);

            if (currentApproval == null)
                throw new Core.Exceptions.ValidationException("No pending approval found for the current step.");

            if (currentApproval.RoleId != user.RoleId)
                throw new UnauthorizedException("You do not have the required role to approve this step.");

            currentApproval.ApprovalStatus = "Approved";
            currentApproval.ApprovedByUserId = userId;
            currentApproval.Comments = comment;
            currentApproval.ActionAt = DateTime.UtcNow;
            _approvalRepository.Update(currentApproval);

            var allApprovals = await _approvalRepository.GetByRequestIdAsync(requestId);
            var nextPending = allApprovals
                .Where(a => a.ApprovalStatus == "Pending" && a.ApprovalOrder > request.CurrentApprovalOrder)
                .OrderBy(a => a.ApprovalOrder)
                .FirstOrDefault();

            var isFullyApproved = false;

            if (nextPending != null)
            {
                request.CurrentApprovalOrder = nextPending.ApprovalOrder;
                request.UpdatedAt = DateTime.UtcNow;
                _requestRepository.Update(request);
            }
            else
            {
                var oldStatus = request.CurrentStatus;
                request.CurrentStatus = "Approved";
                request.CurrentApprovalOrder = null;
                request.UpdatedAt = DateTime.UtcNow;
                _requestRepository.Update(request);

                await LogStatusChangeAsync(requestId, oldStatus, "Approved", userId, "All approval steps completed");
                isFullyApproved = true;
            }

            try
            {
                await _approvalRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Core.Exceptions.ValidationException("The approval was already processed by another user. Please refresh and try again.");
            }

            if (isFullyApproved)
                await _notificationService.NotifyRequestApprovedAsync(request);
        }

        public async Task RejectRequestAsync(int requestId, int userId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new Core.Exceptions.ValidationException("Comment is mandatory when rejecting a request.");

            var request = await _requestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                throw new NotFoundException("Request", requestId);

            if (request.CurrentStatus != "InApproval")
                throw new Core.Exceptions.ValidationException("This request is not in the approval process.");

            if (!request.CurrentApprovalOrder.HasValue)
                throw new Core.Exceptions.ValidationException("No active approval step found.");

            var user = await _userRepository.GetWithRoleAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            var currentApproval = await _approvalRepository.GetCurrentPendingAsync(
                requestId, request.CurrentApprovalOrder.Value);

            if (currentApproval == null)
                throw new Core.Exceptions.ValidationException("No pending approval found for the current step.");

            if (currentApproval.RoleId != user.RoleId)
                throw new UnauthorizedException("You do not have the required role to reject this step.");

            currentApproval.ApprovalStatus = "Rejected";
            currentApproval.ApprovedByUserId = userId;
            currentApproval.Comments = comment;
            currentApproval.ActionAt = DateTime.UtcNow;
            _approvalRepository.Update(currentApproval);

            var oldStatus = request.CurrentStatus;
            request.CurrentStatus = "Rejected";
            request.CurrentApprovalOrder = null;
            request.UpdatedAt = DateTime.UtcNow;
            _requestRepository.Update(request);

            await LogStatusChangeAsync(requestId, oldStatus, "Rejected", userId, comment);

            try
            {
                await _approvalRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Core.Exceptions.ValidationException("The approval was already processed by another user. Please refresh and try again.");
            }

            await _notificationService.NotifyRequestRejectedAsync(request, comment);
        }

        public async Task<IEnumerable<ApprovalHistoryDto>> GetApprovalHistoryByRequestAsync(int requestId)
        {
            var approvals = await _approvalRepository.GetByRequestIdAsync(requestId);
            return approvals.Select(a => new ApprovalHistoryDto
            {
                ApprovalId = a.ApprovalId,
                RequestId = a.RequestId,
                RequestNumber = a.Request.RequestNumber,
                RequestTypeName = string.Empty,
                RequesterName = string.Empty,
                ApprovalOrder = a.ApprovalOrder,
                RoleName = a.Role.RoleName,
                ApprovalStatus = a.ApprovalStatus,
                ApprovedByName = a.ApprovedByUser != null
                    ? $"{a.ApprovedByUser.FirstName} {a.ApprovedByUser.LastName}"
                    : null,
                Comments = a.Comments,
                ActionAt = a.ActionAt,
                CreatedAt = a.CreatedAt
            });
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
    }
}
