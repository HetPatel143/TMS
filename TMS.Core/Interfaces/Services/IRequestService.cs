using TMS.Core.DTOs.Requests;

namespace TMS.Core.Interfaces.Services
{
    public interface IRequestService
    {
        Task<RequestResponseDto> CreateRequestAsync(int userId, CreateRequestDto dto);
        Task<RequestResponseDto> SubmitRequestAsync(int requestId, int userId);
        Task<RequestResponseDto> SaveDraftAsync(int userId, CreateRequestDto dto);
        Task<RequestResponseDto> EditRequestAsync(int requestId, int userId, CreateRequestDto dto);
        Task CancelRequestAsync(int requestId, int userId);
        Task<(IEnumerable<RequestResponseDto> Items, int TotalCount)> GetMyRequestsAsync(int userId, RequestFilterDto filter);
        Task<RequestResponseDto> GetRequestByIdAsync(int requestId, int userId);
        Task<AttachmentResponseDto> UploadAttachmentAsync(int requestId, int userId, string fileName, string filePath, string? fileType);
    }
}
