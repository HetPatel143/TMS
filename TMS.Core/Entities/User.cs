namespace TMS.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Request> CreatedRequests { get; set; } = new List<Request>();
        public virtual ICollection<RequestType> CreatedRequestTypes { get; set; } = new List<RequestType>();
        public virtual ICollection<RequestApproval> RequestApprovals { get; set; } = new List<RequestApproval>();
        public virtual ICollection<RequestStatusHistory> StatusHistoryChanges { get; set; } = new List<RequestStatusHistory>();
        public virtual ICollection<RequestAttachment> UploadedAttachments { get; set; } = new List<RequestAttachment>();
    }
}
