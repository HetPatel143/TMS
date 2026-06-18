namespace TMS.Core.Entities
{
    public class RequestApproval
    {
        public int ApprovalId { get; set; }
        public int RequestId { get; set; }
        public int ApprovalOrder { get; set; }
        public int RoleId { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public string? Comments { get; set; }
        public DateTime? ActionAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] RowVersion { get; set; } = null!;

        public virtual Request Request { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
        public virtual User? ApprovedByUser { get; set; }
    }
}
