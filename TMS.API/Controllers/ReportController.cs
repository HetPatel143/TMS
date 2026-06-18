using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.API.Models;
using TMS.Core.DTOs.Reports;
using TMS.Core.DTOs.Requests;
using TMS.Core.Interfaces.Services;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReports([FromQuery] ReportFilterDto filter)
        {
            var result = await _reportService.GetReportsAsync(filter);
            return Ok(ApiResponse<IEnumerable<RequestResponseDto>>.SuccessResponse(result));
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportReports([FromQuery] ReportFilterDto filter, [FromQuery] string format = "excel")
        {
            switch (format.ToLower())
            {
                case "excel":
                    var excelBytes = await _reportService.ExportToExcelAsync(filter);
                    return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"TMS_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");

                case "csv":
                    var csvContent = await _reportService.ExportToCsvAsync(filter);
                    var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                    return File(csvBytes,
                        "text/csv",
                        $"TMS_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");

                default:
                    return BadRequest(ApiResponse.FailResponse("Invalid export format. Use 'excel' or 'csv'."));
            }
        }
    }
}
