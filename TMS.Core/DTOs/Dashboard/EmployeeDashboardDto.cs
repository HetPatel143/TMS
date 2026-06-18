namespace TMS.Core.DTOs.Dashboard
{
    public class EmployeeDashboardDto
    {
        public int TotalRequests { get; set; }
        public int DraftCount { get; set; }
        public int SubmittedCount { get; set; }
        public int InApprovalCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int CancelledCount { get; set; }
        public int ClosedCount { get; set; }
        public List<RecentRequestDto> RecentRequests { get; set; } = new();
    }

    public class RecentRequestDto
    {
        public int RequestId { get; set; }
        public string RequestNumber { get; set; } = null!;
        public string RequestTypeName { get; set; } = null!;
        public string CurrentStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
