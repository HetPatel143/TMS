namespace TMS.Core.Entities
{
    public class Request
    {
        public int RequestId { get; set; }
        public string RequestNumber { get; set; } = null!;
        public int RequestTypeId { get; set; }
        public int CreatedByUserId { get; set; }
        public string CurrentStatus { get; set; } = null!;
        public int? CurrentApprovalOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual RequestType RequestType { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<RequestFieldValue> FieldValues { get; set; } = new List<RequestFieldValue>();
        public virtual ICollection<RequestApproval> Approvals { get; set; } = new List<RequestApproval>();
        public virtual ICollection<RequestStatusHistory> StatusHistories { get; set; } = new List<RequestStatusHistory>();
        public virtual ICollection<RequestAttachment> Attachments { get; set; } = new List<RequestAttachment>();
    }
}
