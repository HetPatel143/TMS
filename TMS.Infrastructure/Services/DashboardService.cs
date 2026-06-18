using Microsoft.EntityFrameworkCore;
using TMS.Core.DTOs.Dashboard;
using TMS.Core.Entities;
using TMS.Core.Interfaces.Repositories;
using TMS.Core.Interfaces.Services;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly TMSDbContext _context;
        private readonly IRequestRepository _requestRepository;
        private readonly IApprovalRepository _approvalRepository;

        public DashboardService(
            TMSDbContext context,
            IRequestRepository requestRepository,
            IApprovalRepository approvalRepository)
        {
            _context = context;
            _requestRepository = requestRepository;
            _approvalRepository = approvalRepository;
        }

        public async Task<EmployeeDashboardDto> GetEmployeeDashboardAsync(int userId)
        {
            var requests = await _context.Requests
                .Include(r => r.RequestType)
                .Where(r => r.CreatedByUserId == userId)
                .ToListAsync();

            var dashboard = new EmployeeDashboardDto
            {
                TotalRequests = requests.Count,
                DraftCount = requests.Count(r => r.CurrentStatus == "Draft"),
                SubmittedCount = requests.Count(r => r.CurrentStatus == "Submitted"),
                InApprovalCount = requests.Count(r => r.CurrentStatus == "InApproval"),
                ApprovedCount = requests.Count(r => r.CurrentStatus == "Approved"),
                RejectedCount = requests.Count(r => r.CurrentStatus == "Rejected"),
                CancelledCount = requests.Count(r => r.CurrentStatus == "Cancelled"),
                ClosedCount = requests.Count(r => r.CurrentStatus == "Closed"),
                RecentRequests = requests
                    .OrderByDescending(r => r.UpdatedAt)
                    .Take(5)
                    .Select(r => new RecentRequestDto
                    {
                        RequestId = r.RequestId,
                        RequestNumber = r.RequestNumber,
                        RequestTypeName = r.RequestType.Name,
                        CurrentStatus = r.CurrentStatus,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    })
                    .ToList()
            };

            return dashboard;
        }

       public async Task<ApproverDashboardDto> GetApproverDashboardAsync(int userId, int roleId)
        {
            var pendingApprovals = await _context.RequestApprovals
                .Include(a => a.Request)
                    .ThenInclude(r => r.RequestType)
                .Include(a => a.Request)
                    .ThenInclude(r => r.CreatedByUser)
                .Where(a => a.RoleId == roleId
                    && a.ApprovalStatus == "Pending"
                    && a.Request.CurrentStatus == "InApproval"
                    && a.ApprovalOrder == a.Request.CurrentApprovalOrder)
                .ToListAsync();

            var allApprovals = await _context.RequestApprovals
                .Where(a => a.RoleId == roleId && a.ApprovalStatus != "Pending")
                .ToListAsync();

                var totalMyRequests = await _context.Requests
    .CountAsync(r => r.CreatedByUserId == userId);

var myApprovedCount = await _context.RequestApprovals
    .CountAsync(a => a.ApprovedByUserId == userId && a.ApprovalStatus == "Approved");

    Console.WriteLine($"Total My Requests: {totalMyRequests}, My Approved Count: {myApprovedCount}");

            var dashboard = new ApproverDashboardDto
            {
                TotalPendingApprovals = pendingApprovals.Count,
                ApprovedCount = allApprovals.Count(a => a.ApprovalStatus == "Approved"),
                RejectedCount = allApprovals.Count(a => a.ApprovalStatus == "Rejected"),
                TotalMyRequests = totalMyRequests,
MyApprovedCount = myApprovedCount,
                
                PendingApprovals = pendingApprovals.Select(a => new PendingApprovalDto
                {
                    
                    RequestId = a.RequestId,
                    RequestNumber = a.Request.RequestNumber,
                    RequestTypeName = a.Request.RequestType.Name,
                    RequesterName = $"{a.Request.CreatedByUser.FirstName} {a.Request.CreatedByUser.LastName}",
                    ApprovalOrder = a.ApprovalOrder,
                    SubmittedAt = a.CreatedAt
                }).ToList()
            };

            return dashboard;
        }

             public async Task<AdminDashboardDto> GetAdminDashboardAsync()
{
    var totalUsers = await _context.Users.CountAsync();
    var totalRequestTypes = await _context.RequestTypes.CountAsync();
    var totalRequests = await _context.Requests.CountAsync();
    var pendingApprovals = await _context.RequestApprovals
        .CountAsync(a => a.ApprovalStatus == "Pending");

    var requestsByStatus = await _context.Requests
        .GroupBy(r => r.CurrentStatus)
        .Select(g => new { Status = g.Key, Count = g.Count() })
        .ToDictionaryAsync(x => x.Status, x => x.Count);

    // Request types with description + count
    var requestTypeStats = await _context.RequestTypes
        .Include(rt => rt.Requests)
        .Select(rt => new RequestTypeStatsDto
        {
            RequestTypeName = rt.Name,
            Description = rt.Description,
            TotalRequests = rt.Requests.Count
        })
        .ToListAsync();

    // Recent activity — latest 5 requests system-wide
    var recentActivity = await _context.Requests
        .Include(r => r.RequestType)
        .Include(r => r.CreatedByUser)
        .OrderByDescending(r => r.UpdatedAt)
        .Take(5)
        .Select(r => new RecentActivityDto
        {
            RequestId = r.RequestId,
            RequestNumber = r.RequestNumber,
            RequestTypeName = r.RequestType.Name,
            RequesterName = r.CreatedByUser.FirstName + " " + r.CreatedByUser.LastName,
            CurrentStatus = r.CurrentStatus,
            CreatedAt = r.CreatedAt
        })
        .ToListAsync();

    return new AdminDashboardDto
    {
        TotalUsers = totalUsers,
        TotalRequestTypes = totalRequestTypes,
        TotalRequests = totalRequests,
        PendingApprovalsCount = pendingApprovals,
        RequestsByStatus = requestsByStatus,
        RequestTypeStats = requestTypeStats,
        RecentActivity = recentActivity
    };
}
    }
}
