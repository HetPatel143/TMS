namespace TMS.Core.DTOs.Dashboard
{
public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalRequestTypes { get; set; }
    public int TotalRequests { get; set; }
    public int PendingApprovalsCount { get; set; }
    public Dictionary<string, int> RequestsByStatus { get; set; } = new();
    public List<RequestTypeStatsDto> RequestTypeStats { get; set; } = new();
    public List<RecentActivityDto> RecentActivity { get; set; } = new();  // ← add this
}

    public class RequestTypeStatsDto
{
    public string RequestTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TotalRequests { get; set; }
}

public class RecentActivityDto
{
    public int RequestId { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string RequestTypeName { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
}
