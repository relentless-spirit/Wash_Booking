using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Domain.Entities;

namespace WashBooking.Domain.Interfaces.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        // Add any account-specific methods here, for example:
        Task<Account?> GetByUsernameAsync(string username);
        Task<Account?> FindByUsernameOrEmailAsync(string loginIdentifier);
    }
}
