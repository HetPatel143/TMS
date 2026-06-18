using AutoMapper;
using TMS.Core.DTOs.Approvals;
using TMS.Core.DTOs.Requests;
using TMS.Core.DTOs.RequestTypes;
using TMS.Core.DTOs.Users;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestTypeField, FieldResponseDto>()
    .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            CreateMap<RequestType, RequestTypeResponseDto>()
                .ForMember(dest => dest.CreatedByName,
                    opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"))
                .ForMember(dest => dest.Fields,
                    opt => opt.MapFrom(src => src.Fields))
                .ForMember(dest => dest.WorkflowSteps,
                    opt => opt.MapFrom(src => src.Workflows));

            CreateMap<RequestTypeField, FieldResponseDto>();

            CreateMap<ApprovalWorkflow, WorkflowStepResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            CreateMap<Request, RequestResponseDto>()
                .ForMember(dest => dest.RequestTypeName,
                    opt => opt.MapFrom(src => src.RequestType.Name))
                .ForMember(dest => dest.CreatedByName,
                    opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"))
                .ForMember(dest => dest.FieldValues,
                    opt => opt.MapFrom(src => src.FieldValues))
                .ForMember(dest => dest.Approvals,
                    opt => opt.MapFrom(src => src.Approvals.OrderBy(a => a.ApprovalOrder)))
                .ForMember(dest => dest.StatusHistory,
                    opt => opt.MapFrom(src => src.StatusHistories.OrderByDescending(sh => sh.ChangedAt)))
                .ForMember(dest => dest.Attachments,
                    opt => opt.MapFrom(src => src.Attachments));

            CreateMap<RequestFieldValue, RequestFieldValueResponseDto>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.FieldName))
                .ForMember(dest => dest.FieldLabel, opt => opt.MapFrom(src => src.Field.FieldLabel));

            CreateMap<RequestApproval, ApprovalStepResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.ApprovedByName,
                    opt => opt.MapFrom(src => src.ApprovedByUser != null
                        ? $"{src.ApprovedByUser.FirstName} {src.ApprovedByUser.LastName}"
                        : null));

            CreateMap<RequestStatusHistory, StatusHistoryResponseDto>()
                .ForMember(dest => dest.ChangedByName,
                    opt => opt.MapFrom(src => $"{src.ChangedByUser.FirstName} {src.ChangedByUser.LastName}"));

            CreateMap<RequestAttachment, AttachmentResponseDto>()
                .ForMember(dest => dest.UploadedByName,
                    opt => opt.MapFrom(src => $"{src.UploadedByUser.FirstName} {src.UploadedByUser.LastName}"));

            CreateMap<RequestApproval, ApprovalHistoryDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.RequestNumber, opt => opt.MapFrom(src => src.Request.RequestNumber))
                .ForMember(dest => dest.RequestTypeName, opt => opt.MapFrom(src => src.Request.RequestType.Name))
                .ForMember(dest => dest.RequesterName,
                    opt => opt.MapFrom(src => $"{src.Request.CreatedByUser.FirstName} {src.Request.CreatedByUser.LastName}"))
                .ForMember(dest => dest.ApprovedByName,
                    opt => opt.MapFrom(src => src.ApprovedByUser != null
                        ? $"{src.ApprovedByUser.FirstName} {src.ApprovedByUser.LastName}"
                        : null));
        }
    }
}
