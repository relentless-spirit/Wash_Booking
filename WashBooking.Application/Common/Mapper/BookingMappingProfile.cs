using AutoMapper;
using WashBooking.Application.DTOs.BookingDTO;
using WashBooking.Application.DTOs.BookingDTO.Response;
using WashBooking.Domain.Entities;

namespace WashBooking.Application.Common;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, AdminBookingDetailResponse>()
            .ForMember(
                dest => dest.Details,
                opt => opt.MapFrom(src => src.BookingDetails));
        CreateMap<Booking, GuestBookingStatusResponse>()
            .ForMember(
                dest => dest.OverallStatus,
                opt => opt.MapFrom(src => src.Status))
            .ForMember(
                dest => dest.Jobs,
                opt => opt.MapFrom(src => src.BookingDetails));
        CreateMap<Booking, StaffBookingDetailResponse>()
            .ForMember(
                dest => dest.Details,
                opt => opt.MapFrom(src => src.BookingDetails));
        CreateMap<Booking, CustomerBookingDetailResponse>()
            .ForMember(
                dest => dest.Details,
                opt => opt.MapFrom(src => src.BookingDetails));
        
        CreateMap<BookingDetail, AdminBookingDetailItemDto>()
            .ForMember(
                dest => dest.AssigneeName
                , opt => opt.MapFrom(src => src.Assignee.FullName));
        CreateMap<BookingDetail, StaffBookingDetailItemDto>()
            .ForMember(
                dest => dest.AssigneeName
                , opt => opt.MapFrom(src => src.Assignee.FullName));
        CreateMap<BookingDetail, CustomerBookingDetailItemDto>()
            .ForMember(
                dest => dest.AssigneeName
                , opt => opt.MapFrom(src => src.Assignee.FullName));

        CreateMap<BookingDetailProgress, ProgressStepDto>()
            .ForMember(
                dest => dest.Timestamp,
                opt => opt.MapFrom(src => src.CreatedAt)
            );
        CreateMap<BookingDetail, BookingJobTimelineDto>()
            // Hướng dẫn đặc biệt cho các thuộc tính không trùng tên hoặc lồng nhau
            .ForMember(
                dest => dest.ServiceName,
                opt => opt.MapFrom(src => src.Service.Name) // Lấy Name từ Service liên quan
            )
            .ForMember(
                dest => dest.CurrentJobStatus,
                opt => opt.MapFrom(src => src.Status) // Map Status của Entity sang CurrentJobStatus của DTO
            )
            .ForMember(
                dest => dest.Timeline,
                opt => opt.MapFrom(src => src.BookingDetailProgresses) // Map danh sách ProgressHistory sang Timeline
            );

        CreateMap<UpdateBookingRequest, Booking>()
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<BookingItemRequest, BookingDetail>();
    }
}