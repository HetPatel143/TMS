namespace TMS.Core.DTOs.Dashboard
{
    public class ApproverDashboardDto
    {
        public int TotalPendingApprovals { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public List<PendingApprovalDto> PendingApprovals { get; set; } = new();
        public int TotalMyRequests { get; set; }      
public int MyApprovedCount { get; set; } 
    }

    public class PendingApprovalDto
    {
        public int RequestId { get; set; }
        public string RequestNumber { get; set; } = null!;
        public string RequestTypeName { get; set; } = null!;
        public string RequesterName { get; set; } = null!;
        public int ApprovalOrder { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
