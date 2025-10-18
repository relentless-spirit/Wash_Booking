using AutoMapper;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Response;
using WashBooking.Domain.Entities;
using ProgressStepDto = WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Response.ProgressStepDto;

namespace WashBooking.Application.Common;

public class BookingDetailMappingProfile : Profile
{
    public BookingDetailMappingProfile()
    {
        CreateMap<UpdateBookingDetailStatusRequest, BookingDetail>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.NewStatus));
        CreateMap<BookingDetailProgress, ProgressStepDto>()
            .ForMember(dest => dest.Timestamp, otp => otp.MapFrom(src => src.CreatedAt));
        CreateMap<BookingDetail, BookingDetailProgressResponse>()
            // Hướng dẫn đặc biệt cho các thuộc tính không trùng tên hoặc lồng nhau
            .ForMember(dest => dest.BookingDetailId,
                opt => opt.MapFrom(src => src.Id)) // Map Id của Entity sang BookingDetailId của DTO
            .ForMember(
                dest => dest.ServiceName,
                opt => opt.MapFrom(src => src.Service.Name) // Lấy Name từ Service liên quan
            )
            .ForMember(
                dest => dest.CurrentStatus,
                opt => opt.MapFrom(src => src.Status) // Map Status của Entity sang CurrentJobStatus của DTO
            )
            .ForMember(
                dest => dest.Timeline,
                opt => opt.MapFrom(src => src.BookingDetailProgresses) // Map danh sách ProgressHistory sang Timeline
            );
        CreateMap<AssignStaffRequest, BookingDetail>()
            .ForMember(dest => dest.AssigneeId, opt => opt.MapFrom(src => src.NewAssigneeId));
        CreateMap<BookingDetail, MyTaskResponse>()
            .ForMember(dest => dest.DetailId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Booking.CustomerName));

    }
}