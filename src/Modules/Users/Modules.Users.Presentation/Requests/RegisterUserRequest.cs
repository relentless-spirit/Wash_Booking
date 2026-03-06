namespace Modules.Users.Presentation.Requests;

public sealed record RegisterUserRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}