using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Domain.Entities;

namespace WashBooking.Domain.Interfaces.Repositories
{
    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {
        Task<UserProfile?> GetByEmaiAsync(string email);
        Task<UserProfile?> GetByPhoneAsync(string phone);
        Task<List<UserProfile>> GetByRoleAsync(string role);
    }
}
