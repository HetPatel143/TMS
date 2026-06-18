namespace TMS.Core.DTOs.Requests
{
    public class RequestResponseDto
    {
        public int RequestId { get; set; }
        public string RequestNumber { get; set; } = null!;
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; } = null!;
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = null!;
        public string CurrentStatus { get; set; } = null!;
        public int? CurrentApprovalOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<RequestFieldValueResponseDto> FieldValues { get; set; } = new();
        public List<ApprovalStepResponseDto> Approvals { get; set; } = new();
        public List<StatusHistoryResponseDto> StatusHistory { get; set; } = new();
        public List<AttachmentResponseDto> Attachments { get; set; } = new();
    }

    public class RequestFieldValueResponseDto
    {
        public int FieldValueId { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public string FieldLabel { get; set; } = null!;
        public string FieldValue { get; set; } = null!;
    }

    public class ApprovalStepResponseDto
    {
        public int ApprovalId { get; set; }
        public int ApprovalOrder { get; set; }
        public string RoleName { get; set; } = null!;
        public string ApprovalStatus { get; set; } = null!;
        public string? ApprovedByName { get; set; }
        public string? Comments { get; set; }
        public DateTime? ActionAt { get; set; }
    }

    public class StatusHistoryResponseDto
    {
        public string OldStatus { get; set; } = null!;
        public string NewStatus { get; set; } = null!;
        public string ChangedByName { get; set; } = null!;
        public string? ChangeReason { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    public class AttachmentResponseDto
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string? FileType { get; set; }
        public string UploadedByName { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
