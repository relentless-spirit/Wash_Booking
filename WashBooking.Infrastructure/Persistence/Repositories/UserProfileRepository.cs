using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Enums;
using WashBooking.Domain.Interfaces.Repositories;

namespace WashBooking.Infrastructure.Persistence.Repositories
{
    public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(MotoBikeWashingBookingContext context) : base(context)
        {
        }

        public async Task<UserProfile?> GetByEmaiAsync(string email)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(profile => profile.Email.Equals(email));
        }

        public async Task<UserProfile?> GetByPhoneAsync(string phone)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(profile => profile.Phone.Equals(phone));
        }

        public async Task<List<UserProfile>> GetByRoleAsync(string role)
        {
            if (Enum.TryParse<Role>(role, true, out var roleEnum))
            {
                return await _dbSet.Where(profile => profile.Role == roleEnum).ToListAsync();
            }

            return new List<UserProfile>();
        }

        public async Task<PagedResult<UserProfile>> GetPagedWithAccountsAsync(int pageIndex, int pageSize, Expression<Func<UserProfile, bool>>? filter)
        {
            IQueryable<UserProfile> query = _dbSet
                .Include(up => up.Accounts) 
                .AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }
    
            var totalCount = await query.CountAsync();
    
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<UserProfile>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<UserProfile?> GetUserByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(up => up.Accounts)
                .IgnoreQueryFilters()   
                .SingleOrDefaultAsync(up => up.Id.Equals(id));
        }
    }
}
