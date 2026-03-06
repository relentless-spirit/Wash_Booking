using BuildingBlocks.Application.Abstractions.Authentication;
using BuildingBlocks.Application.Abstractions.Data;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Domain;
using Microsoft.Extensions.Logging;
using Modules.Users.Application.Services;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;

namespace Modules.Users.Application.Authentications.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    
    public RegisterUserCommandHandler(
        IUserRepository userRepository, 
        IUsersUnitOfWork usersUnitOfWork, 
        IPasswordHasher passwordHasher,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _usersUnitOfWork = usersUnitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var isEmailTaken = await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken);
        if (isEmailTaken)
        {
            _logger.LogWarning("Email {Email} is already taken", command.Email);
            return Result<Guid>.Failure(Error.Conflict("User.Duplicate.Email", "Email is already taken"));
        }

        var isUsernameTaken = await _userRepository.ExistsByUserNameAsync(command.Username, cancellationToken);
        if (isUsernameTaken)
        {
            _logger.LogWarning("Username {Username} is already taken", command.Username);
            return Result<Guid>.Failure(Error.Conflict("User.Duplicate.Username", "Username is already taken"));
        }
        
        var passwordHash = _passwordHasher.Hash(command.Password);
        var newUser = User.Create(
            command.Email,
            command.Username,
            passwordHash,
            command.FirstName,
            command.LastName);
        
        _userRepository.Add(newUser);
        await _usersUnitOfWork.SaveChangesAsync(cancellationToken);

        return newUser.Id;
    }
}