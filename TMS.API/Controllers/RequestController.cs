using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.API.Models;
using TMS.Core.DTOs.Requests;
using TMS.Core.Interfaces.Services;
using TMS.Infrastructure.Helpers;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/requests")]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly FileUploadHelper _fileUploadHelper;

        public RequestController(IRequestService requestService, FileUploadHelper fileUploadHelper)
        {
            _requestService = requestService;
            _fileUploadHelper = fileUploadHelper;
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Manager,Finance,IT,HR")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _requestService.CreateRequestAsync(userId, dto);
            return CreatedAtAction(nameof(GetRequestById), new { id = result.RequestId },
                ApiResponse<RequestResponseDto>.SuccessResponse(result, "Request created successfully."));
        }

        [HttpPost("draft")]
        [Authorize(Roles = "Employee,Manager,Finance,IT,HR")]
        public async Task<IActionResult> SaveDraft([FromBody] CreateRequestDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _requestService.SaveDraftAsync(userId, dto);
            return CreatedAtAction(nameof(GetRequestById), new { id = result.RequestId },
                ApiResponse<RequestResponseDto>.SuccessResponse(result, "Draft saved successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRequests([FromQuery] RequestFilterDto filter)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (items, totalCount) = await _requestService.GetMyRequestsAsync(userId, filter);

            var pagedResult = new
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };

            return Ok(ApiResponse<object>.SuccessResponse(pagedResult));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _requestService.GetRequestByIdAsync(id, userId);
            return Ok(ApiResponse<RequestResponseDto>.SuccessResponse(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Manager,Finance,IT,HR")]
        public async Task<IActionResult> EditRequest(int id, [FromBody] CreateRequestDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _requestService.EditRequestAsync(id, userId, dto);
            return Ok(ApiResponse<RequestResponseDto>.SuccessResponse(result, "Request updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Manager,Finance,IT,HR")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _requestService.CancelRequestAsync(id, userId);
            return Ok(ApiResponse.SuccessResponse("Request cancelled successfully."));
        }

        // [HttpPost("{id}/submit")]
        //  [Authorize(Roles = "Employee,Manager,Finance,IT,HR")]
        // public async Task<IActionResult> SubmitRequest(int id)
        // {
        //     var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //     var result = await _requestService.SubmitRequestAsync(id, userId);
        //     return Ok(ApiResponse<RequestResponseDto>.SuccessResponse(result, "Request submitted for approval."));
        // }
        [HttpPost("{id}/submit")]
[Authorize(Roles = "Employee,Manager,Finance,IT,HR")]
public async Task<IActionResult> SubmitRequest(int id)
{
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    await _requestService.SubmitRequestAsync(id, userId);
    return Ok(ApiResponse<object>.SuccessResponse(new { requestId = id }, "Request submitted for approval."));
}

        [HttpPost("{id}/attachments")]
        [Authorize(Roles = "Employee")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var (filePath, fileName, fileType) = await _fileUploadHelper.SaveFileAsync(file, id.ToString());

            var result = await _requestService.UploadAttachmentAsync(
                id, userId, fileName, filePath, fileType);

            return CreatedAtAction(nameof(GetRequestById), new { id },
                ApiResponse<AttachmentResponseDto>.SuccessResponse(result, "Attachment uploaded successfully."));
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetStatusHistory(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var request = await _requestService.GetRequestByIdAsync(id, userId);
            return Ok(ApiResponse<List<StatusHistoryResponseDto>>.SuccessResponse(request.StatusHistory));
        }
    }
}
