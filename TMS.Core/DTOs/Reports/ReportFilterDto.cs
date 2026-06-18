namespace TMS.Core.DTOs.Reports
{
    public class ReportFilterDto
    {
        public int? RequestTypeId { get; set; }
        public string? Status { get; set; }
        public int? ApprovalRoleId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
