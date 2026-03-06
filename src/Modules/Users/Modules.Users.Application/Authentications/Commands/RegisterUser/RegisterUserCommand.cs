using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Domain;

namespace Modules.Users.Application.Authentications.Commands.RegisterUser;

public sealed record RegisterUserCommand : ICommand<Guid>
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}