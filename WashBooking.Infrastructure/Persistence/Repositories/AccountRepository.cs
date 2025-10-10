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
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(MotoBikeWashingBookingContext context) : base(context)
        {
        }

        public async Task<Account?> FindByUsernameOrEmailAsync(string loginIdentifier)
        {
             return await _dbSet
                .Include(acc => acc.UserProfile)
                .SingleOrDefaultAsync(acc => acc.Username.Equals(loginIdentifier) 
                                           || acc.UserProfile.Email.Equals(loginIdentifier));
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _dbSet.SingleOrDefaultAsync(acc => acc.Username.Equals(username));
        }
    }
}
