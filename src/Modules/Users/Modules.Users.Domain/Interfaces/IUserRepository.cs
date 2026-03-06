using BuildingBlocks.Domain;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User, Guid>
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct);
}