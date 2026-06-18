using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.API.Models;
using TMS.Core.DTOs.Approvals;
using TMS.Core.Interfaces.Services;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/approvals")]
    [Authorize(Roles = "Manager,Finance,IT,HR")]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;

        public ApprovalController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var roleId = int.Parse(User.FindFirstValue("RoleId")!);
            var result = await _approvalService.GetPendingApprovalsForRoleAsync(roleId);
            return Ok(ApiResponse<IEnumerable<ApprovalHistoryDto>>.SuccessResponse(result));
        }

        [HttpPost("{requestId}/approve")]
        public async Task<IActionResult> ApproveRequest(int requestId, [FromBody] ApprovalActionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _approvalService.ApproveRequestAsync(requestId, userId, dto.Comments);
            return Ok(ApiResponse.SuccessResponse("Request approved successfully."));
        }

        [HttpPost("{requestId}/reject")]
        public async Task<IActionResult> RejectRequest(int requestId, [FromBody] ApprovalActionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comments))
                return BadRequest(ApiResponse.FailResponse("Comment is mandatory when rejecting a request."));

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _approvalService.RejectRequestAsync(requestId, userId, dto.Comments!);
            return Ok(ApiResponse.SuccessResponse("Request rejected successfully."));
        }

        [HttpGet("{requestId}/history")]
        public async Task<IActionResult> GetApprovalHistory(int requestId)
        {
            var result = await _approvalService.GetApprovalHistoryByRequestAsync(requestId);
            return Ok(ApiResponse<IEnumerable<ApprovalHistoryDto>>.SuccessResponse(result));
        }
    }
}
