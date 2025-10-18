using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;

namespace WashBooking.Domain.Interfaces.Repositories
{
    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {
        Task<UserProfile?> GetByEmaiAsync(string email);
        Task<UserProfile?> GetByPhoneAsync(string phone);
        Task<List<UserProfile>> GetByRoleAsync(string role);
        Task<PagedResult<UserProfile>> GetPagedWithAccountsAsync(int pageIndex, int pageSize, Expression<Func<UserProfile, bool>>? filter);
        Task<UserProfile?> GetUserByIdAsync(Guid id);
    }
}
