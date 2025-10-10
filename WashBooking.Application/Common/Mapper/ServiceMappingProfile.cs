using AutoMapper;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;

namespace WashBooking.Application.Common;

public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        CreateMap<CreateServiceRequest, Service>();
        CreateMap<Service, GetAllResponse>();
        CreateMap<Service, GetPagedResponse>();
        
        CreateMap<PagedResult<Service>, PagedResult<GetPagedResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        
        CreateMap<UpdateServiceRequest, Service>();
    }
}