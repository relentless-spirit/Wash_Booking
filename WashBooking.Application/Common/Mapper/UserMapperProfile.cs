using AutoMapper;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Application.DTOs.UserProfileDTO;
using WashBooking.Application.DTOs.UserProfileDTO.Request;
using WashBooking.Application.DTOs.UserProfileDTO.Response;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;

namespace WashBooking.Application.Common;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserProfile, UserProfileResponse>();
        CreateMap<UpdateUserProfileRequest, UserProfile>();
        // === ÁNH XẠ CHO ADMIN ===
        CreateMap<UserProfile, AdminUserDetailResponse>()
            .ForMember(dest => dest.Username, 
                opt => opt.MapFrom(src => src.Accounts.FirstOrDefault().Username))
            .ForMember(dest => dest.AccountType, 
                opt => opt.MapFrom(src => src.Accounts.FirstOrDefault().AccountType.ToString()))
            .ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.ToString()));
        
        // Cấu hình cho PagedResult của Admin
        CreateMap<PagedResult<UserProfile>, PagedResult<AdminUserDetailResponse>>();

        // === ÁNH XẠ CHO STAFF ===
        CreateMap<UserProfile, StaffUserDetailResponse>();
        
        // Cấu hình cho PagedResult của Staff
        CreateMap<PagedResult<UserProfile>, PagedResult<StaffUserDetailResponse>>();

        CreateMap<ActivateUserRequest, UserProfile>();
        CreateMap<ActivateUserRequest, Account>();
        
        CreateMap<UpdateUserRoleRequest, UserProfile>();
    }
}