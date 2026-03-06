using BuildingBlocks.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Persistence.DBContext;

namespace Modules.Users.Infrastructure.Persistence.Repositories;

internal sealed class UserReposity : GenericRepository<User, Guid>, IUserRepository
{
    private readonly UserDbContext _context;
    
    public UserReposity(UserDbContext context) : base(context)
    {
        _context = context;
    }
    
    
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        var isEmailExists  = await _context.Users.AnyAsync(x => x.Email == email, ct);
        return isEmailExists ;
    }

    public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct)
    {
        var isUserNameExists  = _context.Users.AnyAsync(x => x.Username == userName, ct);
        return isUserNameExists ;
    }
}