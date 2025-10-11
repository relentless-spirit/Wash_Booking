using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Domain.Entities;
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
            return await _dbSet.Where(profile => profile.Role.Equals(role)).ToListAsync();
        }
    }
}
