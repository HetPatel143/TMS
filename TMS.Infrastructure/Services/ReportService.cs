using System.Globalization;
using System.Text;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMS.Core.DTOs.Reports;
using TMS.Core.DTOs.Requests;
using TMS.Core.Interfaces.Services;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly TMSDbContext _context;
        private readonly IMapper _mapper;

        public ReportService(TMSDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       public async Task<IEnumerable<RequestResponseDto>> GetReportsAsync(ReportFilterDto filter)
{
    var query = _context.Requests
        .AsNoTracking()
        .Include(r => r.RequestType)
        .Include(r => r.CreatedByUser)
        .AsQueryable();

    if (filter.RequestTypeId.HasValue)
        query = query.Where(r => r.RequestTypeId == filter.RequestTypeId.Value);

    if (!string.IsNullOrEmpty(filter.Status))
        query = query.Where(r => r.CurrentStatus == filter.Status);

    if (filter.ApprovalRoleId.HasValue)
        query = query.Where(r => r.Approvals.Any(a => a.RoleId == filter.ApprovalRoleId.Value));

    if (filter.FromDate.HasValue)
        query = query.Where(r => r.CreatedAt >= filter.FromDate.Value);

    if (filter.ToDate.HasValue)
        query = query.Where(r => r.CreatedAt <= filter.ToDate.Value);

    // Project directly instead of loading full entities
    var requests = await query
        .OrderByDescending(r => r.CreatedAt)
        .Select(r => new RequestResponseDto
        {
            RequestId = r.RequestId,
            RequestNumber = r.RequestNumber,
            RequestTypeId = r.RequestTypeId,
            RequestTypeName = r.RequestType.Name,
            CreatedByUserId = r.CreatedByUserId,
            CreatedByName = r.CreatedByUser.FirstName + " " + r.CreatedByUser.LastName,
            CurrentStatus = r.CurrentStatus,
            CurrentApprovalOrder = r.CurrentApprovalOrder,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            // These are empty for list view — detail page loads them separately
            FieldValues = new List<RequestFieldValueResponseDto>(),
            Approvals = new List<ApprovalStepResponseDto>(),
            StatusHistory = new List<StatusHistoryResponseDto>(),
            Attachments = new List<AttachmentResponseDto>()
        })
        .ToListAsync();

    return requests;
}
        public async Task<byte[]> ExportToExcelAsync(ReportFilterDto filter)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var data = await GetReportsAsync(filter);
            var records = data.ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Requests Report");

            worksheet.Cells[1, 1].Value = "Request Number";
            worksheet.Cells[1, 2].Value = "Request ID";
            worksheet.Cells[1, 3].Value = "Request Type";
            worksheet.Cells[1, 4].Value = "Created By";
            worksheet.Cells[1, 5].Value = "Status";
            worksheet.Cells[1, 6].Value = "Created At";
            worksheet.Cells[1, 7].Value = "Updated At";

            using (var headerRange = worksheet.Cells[1, 1, 1, 7])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            for (int i = 0; i < records.Count; i++)
            {
                var row = i + 2;
                worksheet.Cells[row, 1].Value = records[i].RequestNumber;
                worksheet.Cells[row, 2].Value = records[i].RequestId;
                worksheet.Cells[row, 3].Value = records[i].RequestTypeName;
                worksheet.Cells[row, 4].Value = records[i].CreatedByName;
                worksheet.Cells[row, 5].Value = records[i].CurrentStatus;
                worksheet.Cells[row, 6].Value = records[i].CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[row, 7].Value = records[i].UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            }

            worksheet.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        public async Task<string> ExportToCsvAsync(ReportFilterDto filter)
        {
            var data = await GetReportsAsync(filter);

            using var stringWriter = new StringWriter();
            using var csvWriter = new CsvWriter(stringWriter, new CsvConfiguration(CultureInfo.InvariantCulture));

            csvWriter.WriteField("Request Number");
            csvWriter.WriteField("Request ID");
            csvWriter.WriteField("Request Type");
            csvWriter.WriteField("Created By");
            csvWriter.WriteField("Status");
            csvWriter.WriteField("Created At");
            csvWriter.WriteField("Updated At");
            await csvWriter.NextRecordAsync();

            foreach (var record in data)
            {
                csvWriter.WriteField(record.RequestNumber);
                csvWriter.WriteField(record.RequestId);
                csvWriter.WriteField(record.RequestTypeName);
                csvWriter.WriteField(record.CreatedByName);
                csvWriter.WriteField(record.CurrentStatus);
                csvWriter.WriteField(record.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                csvWriter.WriteField(record.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                await csvWriter.NextRecordAsync();
            }

            return stringWriter.ToString();
        }
    }
}
