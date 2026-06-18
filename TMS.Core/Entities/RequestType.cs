namespace TMS.Core.Entities
{
    public class RequestType
    {
        public int RequestTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<RequestTypeField> Fields { get; set; } = new List<RequestTypeField>();
        public virtual ICollection<ApprovalWorkflow> Workflows { get; set; } = new List<ApprovalWorkflow>();
        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}
