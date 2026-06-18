using TMS.Core.Entities;

namespace TMS.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task NotifyRequestSubmittedAsync(Request request);
        Task NotifyRequestApprovedAsync(Request request);
        Task NotifyRequestRejectedAsync(Request request, string comment);
    }
}
