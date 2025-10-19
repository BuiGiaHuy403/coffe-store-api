using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Mappers;
using PRN232.Lab2.CoffeeStore.Repositories.Constants;
using PRN232.Lab2.CoffeeStore.Services.Infrastructure;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users;
using PRN232.Lab2.CoffeeStore.Services.Services;

namespace PRN232.Lab2.CoffeeStore.API.Controllers.Users
{
    [ApiController]
    [Route("api/v{v:apiversion}/[controller]")]
    [ApiVersion(1)]

    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenRevokeService _tokenRevokeService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
             IAuthService authService,
             ITokenService tokenService,
             ITokenRevokeService tokenRevokeService,
             ILogger<UsersController> logger)
        {
            _authService = authService;
            _tokenRevokeService = tokenRevokeService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// User login endpoint
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT tokens if login successful</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var serviceResponse = await _authService.LoginAsync(request);

            if (!serviceResponse.Success)
            {
                return Unauthorized(serviceResponse.ToBaseApiResponse());
            }

            return Ok(serviceResponse.ToDataApiResponse());
        }

        /// <summary>
        /// User registration endpoint
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>JWT tokens if registration successful</returns>
        [HttpPost("customer/signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var serviceResponse = await _authService.RegisterAsync(request, Roles.Customer);

            if (!serviceResponse.Success)
            {
                return BadRequest(serviceResponse.ToBaseApiResponse());
            }

            return Ok(serviceResponse.ToBaseApiResponse());
        }

        /// <summary>
        /// Refresh JWT tokens endpoint
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>New JWT tokens if refresh successful</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            _logger.LogInformation("Token refresh attempt");

            var serviceResponse = await _tokenService.RefreshTokens(request);

            if (!serviceResponse.Success)
            {
                return Unauthorized(serviceResponse.ToBaseApiResponse());
            }

            return Ok(serviceResponse.ToDataApiResponse());
        }

        /// <summary>
        /// Revoke user's refresh token (logout)
        /// </summary>
        /// <param name="request">Refresh token to revoke</param>
        /// <returns>Success or error message</returns>
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeRefreshTokenRequest request)
        {
            _logger.LogInformation("Token revocation attempt");

            var serviceResponse = await _tokenRevokeService.RevokeToken(request);

            if (!serviceResponse.Success)
            {
                return Unauthorized(serviceResponse.ToBaseApiResponse());
            }

            return Ok(serviceResponse.ToBaseApiResponse());
        }
    }
}
