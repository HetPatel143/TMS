namespace TMS.Core.DTOs.Approvals
{
    public class ApprovalHistoryDto
    {
        public int ApprovalId { get; set; }
        public int RequestId { get; set; }
        public string RequestNumber { get; set; } = null!;
        public string RequestTypeName { get; set; } = null!;
        public string RequesterName { get; set; } = null!;
        public int ApprovalOrder { get; set; }
        public string RoleName { get; set; } = null!;
        public string ApprovalStatus { get; set; } = null!;
        public string? ApprovedByName { get; set; }
        public string? Comments { get; set; }
        public DateTime? ActionAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
