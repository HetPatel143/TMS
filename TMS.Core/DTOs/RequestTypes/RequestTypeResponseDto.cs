namespace TMS.Core.DTOs.RequestTypes
{
    public class RequestTypeResponseDto
    {
        public int RequestTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<FieldResponseDto> Fields { get; set; } = new();
        public List<WorkflowStepResponseDto> WorkflowSteps { get; set; } = new();
    }

    public class FieldResponseDto
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public string FieldLabel { get; set; } = null!;
        public string FieldType { get; set; } = null!;
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
           public string? Options { get; set; } 
    }

    public class WorkflowStepResponseDto
    {
        public int WorkflowId { get; set; }
        public int ApprovalOrder { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
