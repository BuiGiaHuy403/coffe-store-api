using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PRN232.Lab2.CoffeeStore.Repositories.Interfaces;
using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Infrastructure;
using PRN232.Lab2.CoffeeStore.Services.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly UserManager<User> _userManager;

        private readonly JwtSettings _jwtSettings;

        private readonly IUnitOfWork _unitOfWork;


        public TokenService(
            ILogger<TokenService> logger,
                UserManager<User> userManager,
                IOptions<JwtSettings> jwtSettings,
                IUnitOfWork unitOfWork
            )
        {
            _logger = logger;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<DataServiceResponse<TokenResponse>> GenerateTokens(string userId)
        {
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new DataServiceResponse<TokenResponse>()
                {
                    Success = false,
                    Message = "User not found",
                    Data = null!
                };
            }
            //ROLE??
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Customer"; // fallback
            var accessToken = GenerateAccessToken(user, role);
            var refreshToken = await GenerateRefreshToken(userId);

            await refreshTokenRepo.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            var response = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiresIn = _jwtSettings.AccessTokenExpireInMinutes * 60,
                RefreshTokenExpiresIn = _jwtSettings.RefreshTokenExpireInDays * 24 * 60 * 60,
                AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpireInMinutes),
                RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireInDays)
            };

            return new DataServiceResponse<TokenResponse>
            {
                Success = true,
                Message = "Generate Access Token and Refresh Token successfully",
                Data = response
            };
        }

        private string GenerateAccessToken(User user, string role)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var cred = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, role),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpireInMinutes),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshToken(string userId)
        {
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = GenerateRandomToken(),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireInDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
            };

         

            return refreshToken;
        }

        private string GenerateRandomToken()
        {
            using var randomGenerator = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            randomGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<DataServiceResponse<TokenResponse>> RefreshTokens(RefreshTokenRequest request)
        {
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

            var refreshToken = await refreshTokenRepo.Query(false)
                .Include(rf => rf.User)
                .FirstOrDefaultAsync(rf => rf.Token == request.RefreshToken);

            if (refreshToken == null || !refreshToken.IsActive || refreshToken.IsRevoked)
            {
                return new DataServiceResponse<TokenResponse>()
                {
                    Success = false,
                    Message = "Invalid or expired refresh token",
                    Data = null!
                };
            }
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return new DataServiceResponse<TokenResponse>()
                {
                    Success = false,
                    Message = "User not found",
                    Data = null!
                };
            }

            refreshToken.ReasonRevoked = "Replaced by new token";

            var roles = await _userManager.GetRolesAsync(refreshToken.User);
            var role = roles.FirstOrDefault() ?? "Customer"; // fallback
            var accessToken = GenerateAccessToken(refreshToken.User, role);
            var newRefreshToken = await GenerateRefreshToken(refreshToken.UserId);

            refreshToken.ReplacedByToken = newRefreshToken.Token;

            await refreshTokenRepo.AddAsync(newRefreshToken);
            var rows = await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("SaveChanges affected rows: {Rows}", rows);
            var response = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiresIn = _jwtSettings.AccessTokenExpireInMinutes * 60,
                RefreshTokenExpiresIn = _jwtSettings.RefreshTokenExpireInDays * 24 * 60 * 60,
                AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpireInMinutes),
                RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireInDays)
            };

            return new DataServiceResponse<TokenResponse>
            {
                Success = true,
                Message = "Token Refresh Successfully",
                Data = response
            };
        }
    }
}
