namespace TMS.Core.DTOs.Requests
{
    public class RequestFilterDto
    {
        public string? Status { get; set; }
        public int? RequestTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
