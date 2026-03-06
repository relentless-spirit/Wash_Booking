using BuildingBlocks.Presentation;
using BuildingBlocks.Presentation.Infrastructures;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Users.Application.Authentications.Commands.RegisterUser;
using Modules.Users.Presentation.Requests;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Modules.Users.Presentation.Controllers.v1;

[ApiController]
[Route("api/v1/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _sender;
    
    public AuthenticationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var command = new RegisterUserCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        
        var result = await _sender.Send(command, cancellationToken);
        return result.Match<Guid, IResult>(
            onSuccess: userId => Results.Created($"/api/v1/users/{userId}", userId),
            onFailure: r => CustomResults.Problem(r)
        );
    }
}