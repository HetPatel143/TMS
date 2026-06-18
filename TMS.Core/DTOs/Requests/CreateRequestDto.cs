namespace TMS.Core.DTOs.Requests
{
    public class CreateRequestDto
    {
        public int RequestTypeId { get; set; }
        public List<RequestFieldValueDto> FieldValues { get; set; } = new();
    }
}
