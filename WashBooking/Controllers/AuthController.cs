using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WashBooking.Application.DTOs.AuthDTO.LoginDTO;
using WashBooking.Application.DTOs.AuthDTO.LogoutDTO;
using WashBooking.Application.DTOs.AuthDTO.RefreshTokenDTO;
using WashBooking.Application.DTOs.AuthDTO.RegisterDTO;
using WashBooking.Application.Interfaces.Auth;
using WashBooking.Domain.Common;

namespace WashBooking.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Duplicate"))
                {
                    return Conflict(result.Error);
                }
                else if (result.Error.Code.Contains("Validation"))
                {
                    return UnprocessableEntity(result.Errors);
                }
                return BadRequest(result.Error);
            }

            return Ok(new { code = "Auth.Register.Success", message = "Account registration successful!"});
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Validation"))
                {
                    return UnprocessableEntity(result.Errors);
                }
                else if (result.Error.Code.Contains("InvalidCredentials"))
                {
                    return Unauthorized(result.Error);
                }
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var result = await _authService.LogoutAsync(request);
            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Validation"))
                {
                    return UnprocessableEntity(result.Errors);
                }
                else if (result.Error.Code.Contains("NotFound"))
                {
                    return NotFound(result.Error);
                }
                return BadRequest(result.Error);
            }
            return Ok(new { code = "Auth.Logout.Success", message = "Logout completed successfully." });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Validation"))
                {
                    return UnprocessableEntity(result.Errors);
                }
                else if (result.Error.Code.Contains("Invalid"))
                {
                    return Unauthorized(result.Error);
                }
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }
    }
}
