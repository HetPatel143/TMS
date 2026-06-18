using TMS.Core.DTOs.Reports;
using TMS.Core.DTOs.Requests;

namespace TMS.Core.Interfaces.Services
{
    public interface IReportService
    {
        Task<IEnumerable<RequestResponseDto>> GetReportsAsync(ReportFilterDto filter);
        Task<byte[]> ExportToExcelAsync(ReportFilterDto filter);
        Task<string> ExportToCsvAsync(ReportFilterDto filter);
    }
}
