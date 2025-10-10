using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Application.DTOs.UserProfileDTO;
using WashBooking.Domain.Entities;

namespace WashBooking.Application.Common
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<UserProfile, UserProfileDTO>();
            
        }
    }
}
