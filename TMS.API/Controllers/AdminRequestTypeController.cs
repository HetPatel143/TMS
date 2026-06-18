using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.API.Models;
using TMS.Core.DTOs.RequestTypes;
using TMS.Core.Interfaces.Services;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/admin/request-types")]
    [Authorize]
    public class AdminRequestTypeController : ControllerBase
    {
        private readonly IRequestTypeService _requestTypeService;

        public AdminRequestTypeController(IRequestTypeService requestTypeService)
        {
            _requestTypeService = requestTypeService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRequestType([FromBody] CreateRequestTypeDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _requestTypeService.CreateRequestTypeAsync(userId, dto);
            return CreatedAtAction(nameof(GetRequestTypeById), new { id = result.RequestTypeId },
                ApiResponse<RequestTypeResponseDto>.SuccessResponse(result, "Request type created successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequestTypes()
        {
            var result = await _requestTypeService.GetAllRequestTypesAsync();
            return Ok(ApiResponse<IEnumerable<RequestTypeResponseDto>>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestTypeById(int id)
        {
            var result = await _requestTypeService.GetRequestTypeWithFieldsAndWorkflowAsync(id);
            return Ok(ApiResponse<RequestTypeResponseDto>.SuccessResponse(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRequestType(int id, [FromBody] UpdateRequestTypeDto dto)
        {
            var result = await _requestTypeService.UpdateRequestTypeAsync(id, dto);
            return Ok(ApiResponse<RequestTypeResponseDto>.SuccessResponse(result, "Request type updated successfully."));
        }

        [HttpPost("{id}/fields")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddField(int id, [FromBody] CreateFieldDto dto)
        {
            var result = await _requestTypeService.AddFieldAsync(id, dto);
            return CreatedAtAction(nameof(GetRequestTypeById), new { id },
                ApiResponse<FieldResponseDto>.SuccessResponse(result, "Field added successfully."));
        }
[HttpDelete("{id}/fields/{fieldId}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteField(int id, int fieldId)
{
    await _requestTypeService.DeleteFieldAsync(id, fieldId);
    return Ok(ApiResponse.SuccessResponse("Field deleted successfully."));
}
        [HttpPut("{id}/fields/{fieldId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateField(int id, int fieldId, [FromBody] CreateFieldDto dto)
        {
            var result = await _requestTypeService.UpdateFieldAsync(id, fieldId, dto);
            return Ok(ApiResponse<FieldResponseDto>.SuccessResponse(result, "Field updated successfully."));
        }

        [HttpPost("{id}/workflows")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetWorkflow(int id, [FromBody] List<WorkflowStepDto> steps)
        {
            await _requestTypeService.SetWorkflowAsync(id, steps);
            return Ok(ApiResponse.SuccessResponse("Workflow configured successfully."));
        }

        [HttpGet("{id}/workflows")]
        public async Task<IActionResult> GetWorkflow(int id)
        {
            var result = await _requestTypeService.GetRequestTypeWithFieldsAndWorkflowAsync(id);
            return Ok(ApiResponse<List<WorkflowStepResponseDto>>.SuccessResponse(
                result.WorkflowSteps));
        }
    }
}
