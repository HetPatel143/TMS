namespace TMS.Core.DTOs.RequestTypes
{
    public class CreateRequestTypeDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<CreateFieldDto> Fields { get; set; } = new();
        public List<WorkflowStepDto> WorkflowSteps { get; set; } = new();
    }
}
