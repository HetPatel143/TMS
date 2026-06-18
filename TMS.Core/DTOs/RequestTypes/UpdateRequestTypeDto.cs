namespace TMS.Core.DTOs.RequestTypes
{
    public class UpdateRequestTypeDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
