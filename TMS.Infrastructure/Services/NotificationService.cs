// using Microsoft.EntityFrameworkCore;
// using TMS.Core.Entities;
// using TMS.Core.Interfaces.Services;
// using TMS.Infrastructure.Data;
// using TMS.Infrastructure.Helpers;

// namespace TMS.Infrastructure.Services
// {
//     public class NotificationService : INotificationService
//     {
//         private readonly EmailHelper _emailHelper;
//         private readonly TMSDbContext _context;

//         public NotificationService(EmailHelper emailHelper, TMSDbContext context)
//         {
//             _emailHelper = emailHelper;
//             _context = context;
//         }

//         public async Task SendEmailAsync(string to, string subject, string body)
//         {
//             await _emailHelper.SendEmailAsync(to, subject, body);
//         }

//         public async Task NotifyRequestSubmittedAsync(Request request)
//         {
//             var fullRequest = await _context.Requests
//                 .Include(r => r.RequestType)
//                     .ThenInclude(rt => rt.Workflows)
//                         .ThenInclude(w => w.Role)
//                             .ThenInclude(role => role.Users)
//                 .Include(r => r.CreatedByUser)
//                 .FirstOrDefaultAsync(r => r.RequestId == request.RequestId);

//             if (fullRequest == null) return;

//             var firstWorkflow = fullRequest.RequestType.Workflows
//                 .Where(w => w.IsActive)
//                 .OrderBy(w => w.ApprovalOrder)
//                 .FirstOrDefault();

//             if (firstWorkflow == null) return;

//             var approvers = firstWorkflow.Role.Users.Where(u => u.IsActive).ToList();

//             foreach (var approver in approvers)
//             {
//                 await _emailHelper.SendEmailAsync(
//                     approver.Email,
//                     $"New Request Pending Your Approval - #{fullRequest.RequestId}",
//                     $"<p>Hello {approver.FirstName},</p>" +
//                     $"<p>A new <strong>{fullRequest.RequestType.Name}</strong> request (#{fullRequest.RequestId}) " +
//                     $"has been submitted by {fullRequest.CreatedByUser.FirstName} {fullRequest.CreatedByUser.LastName} " +
//                     $"and is pending your approval.</p>" +
//                     $"<p>Please login to the system to review and take action.</p>");
//             }
//         }

//         public async Task NotifyRequestApprovedAsync(Request request)
//         {
//             var fullRequest = await _context.Requests
//                 .Include(r => r.RequestType)
//                 .Include(r => r.CreatedByUser)
//                 .FirstOrDefaultAsync(r => r.RequestId == request.RequestId);

//             if (fullRequest == null) return;

//             await _emailHelper.SendEmailAsync(
//                 fullRequest.CreatedByUser.Email,
//                 $"Your Request #{fullRequest.RequestId} Has Been Approved",
//                 $"<p>Hello {fullRequest.CreatedByUser.FirstName},</p>" +
//                 $"<p>Your <strong>{fullRequest.RequestType.Name}</strong> request (#{fullRequest.RequestId}) " +
//                 $"has been fully approved.</p>" +
//                 $"<p>Thank you.</p>");
//         }

//         public async Task NotifyRequestRejectedAsync(Request request, string comment)
//         {
//             var fullRequest = await _context.Requests
//                 .Include(r => r.RequestType)
//                 .Include(r => r.CreatedByUser)
//                 .FirstOrDefaultAsync(r => r.RequestId == request.RequestId);

//             if (fullRequest == null) return;

//             await _emailHelper.SendEmailAsync(
//                 fullRequest.CreatedByUser.Email,
//                 $"Your Request #{fullRequest.RequestId} Has Been Rejected",
//                 $"<p>Hello {fullRequest.CreatedByUser.FirstName},</p>" +
//                 $"<p>Your <strong>{fullRequest.RequestType.Name}</strong> request (#{fullRequest.RequestId}) " +
//                 $"has been rejected.</p>" +
//                 $"<p><strong>Reason:</strong> {comment}</p>" +
//                 $"<p>Please review and resubmit if needed.</p>");
//         }
//     }
// }

using Microsoft.EntityFrameworkCore;
using TMS.Core.Entities;
using TMS.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using TMS.Infrastructure.Data;
using TMS.Infrastructure.Helpers;

namespace TMS.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EmailHelper _emailHelper;
        private readonly TMSDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(EmailHelper emailHelper, TMSDbContext context, ILogger<NotificationService> logger)
        {
            _emailHelper = emailHelper;
            _context = context;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                await _emailHelper.SendEmailAsync(to, subject, body);
                _logger?.LogInformation("Email sent to {Recipient} with subject {Subject}", to, subject);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to send email to {Recipient} with subject {Subject}", to, subject);
                throw;
            }
        }

        public Task NotifyRequestSubmittedAsync(Request request)
        {
            return SendSubmittedEmailAsync(request.RequestId);
        }

        public Task NotifyRequestApprovedAsync(Request request)
        {
            return SendApprovedEmailAsync(request.RequestId);
        }

        public Task NotifyRequestRejectedAsync(Request request, string comment)
        {
            return SendRejectedEmailAsync(request.RequestId, comment);
        }

        private async Task SendSubmittedEmailAsync(int requestId)
        {
            try
            {
                var fullRequest = await _context.Requests
                    .AsNoTracking()
                    .Include(r => r.RequestType)
                        .ThenInclude(rt => rt.Workflows)
                            .ThenInclude(w => w.Role)
                                .ThenInclude(role => role.Users)
                    .Include(r => r.CreatedByUser)
                    .FirstOrDefaultAsync(r => r.RequestId == requestId);

                if (fullRequest == null) return;

                var firstWorkflow = fullRequest.RequestType.Workflows
                    .Where(w => w.IsActive)
                    .OrderBy(w => w.ApprovalOrder)
                    .FirstOrDefault();

                if (firstWorkflow == null) return;

                var approvers = firstWorkflow.Role.Users.Where(u => u.IsActive).ToList();

                foreach (var approver in approvers)
                {
                    await _emailHelper.SendEmailAsync(
                        approver.Email,
                        $"New Request Pending Your Approval - #{fullRequest.RequestId}",
                        $"<p>Hello {approver.FirstName},</p>" +
                        $"<p>A new <strong>{fullRequest.RequestType.Name}</strong> request (#{fullRequest.RequestId}) " +
                        $"has been submitted by {fullRequest.CreatedByUser.FirstName} {fullRequest.CreatedByUser.LastName} " +
                        $"and is pending your approval.</p>" +
                        $"<p>Please login to the system to review and take action.</p>");
                }
            }
            catch { /* swallow — don't fail the request */ }
        }

        private async Task SendApprovedEmailAsync(int requestId)
        {
            try
            {
                var fullRequest = await _context.Requests
                    .AsNoTracking()
                    .Include(r => r.RequestType)
                    .Include(r => r.CreatedByUser)
                    .FirstOrDefaultAsync(r => r.RequestId == requestId);

                if (fullRequest == null) return;

                await _emailHelper.SendEmailAsync(
                    fullRequest.CreatedByUser.Email,
                    $"Your Request #{fullRequest.RequestId} Has Been Approved",
                    $"<p>Hello {fullRequest.CreatedByUser.FirstName},</p>" +
                    $"<p>Your <strong>{fullRequest.RequestType.Name}</strong> request (#{fullRequest.RequestId}) " +
                    $"has been fully approved.</p>" +
                    $"<p>Thank you.</p>");
            }
            catch { }
        }

        private async Task SendRejectedEmailAsync(int requestId, string comment)
        {
            try
            {
                var fullRequest = await _context.Requests
                    .AsNoTracking()
                    .Include(r => r.RequestType)
                    .Include(r => r.CreatedByUser)
                    .FirstOrDefaultAsync(r => r.RequestId == requestId);

                if (fullRequest == null) return;

                await _emailHelper.SendEmailAsync(
                    fullRequest.CreatedByUser.Email,
                    $"Your Request #{fullRequest.RequestId} Has Been Rejected",
                    $"<p>Hello {fullRequest.CreatedByUser.FirstName},</p>" +
                    $"<p>Your <strong>{fullRequest.RequestType.Name}</strong> request (#{fullRequest.RequestId}) " +
                    $"has been rejected.</p>" +
                    $"<p><strong>Reason:</strong> {comment}</p>" +
                    $"<p>Please review and resubmit if needed.</p>");
            }
            catch { }
        }
    }
}
