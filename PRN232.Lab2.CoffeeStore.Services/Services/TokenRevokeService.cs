using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PRN232.Lab2.CoffeeStore.Repositories.Interfaces;
using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Infrastructure;
using PRN232.Lab2.CoffeeStore.Services.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Services
{
    public class TokenRevokeService : ITokenRevokeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;

        public TokenRevokeService(
               UserManager<User> userManager,
               IOptions<JwtSettings> jwtSettings,
               IUnitOfWork unitOfWork
           )
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseServiceResponse> CleanupExpiredTokens()
        {
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

            var expiredTokens = await refreshTokenRepo.Query()
                .Where(rf => rf.IsExpired == true).ToListAsync();
            foreach (var expiredToken in expiredTokens)
            {
                expiredToken.IsRevoked = true;
                expiredToken.RevokedAt = DateTime.UtcNow;
                expiredToken.ReasonRevoked = "Clean Up Expired Tokens";
            }

            await _unitOfWork.SaveChangesAsync();
            return new BaseServiceResponse
            {
                Success = true,
                Message = $"Removed {expiredTokens.Count} expired tokens."
            };
        }

        public async Task<BaseServiceResponse> RevokeAllUserTokens(string userId, string? revokedBy = null)
        {
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

            var user = await _userManager.FindByIdAsync(userId);

            var revokedRefreshTokens = await refreshTokenRepo.Query()
                .Where(rf => rf.UserId == userId).ToListAsync();

            foreach(var refreshToken in revokedRefreshTokens)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.RevokedBy = revokedBy;
                refreshToken.ReasonRevoked = "Bulk revocation (logout all sessions)";
            }

            await _unitOfWork.SaveChangesAsync();

            return new BaseServiceResponse
            {
                Success = true,
                Message = $"Revoked active tokens for user {userId}"
            };
        }

        public async Task<BaseServiceResponse> RevokeToken(RevokeRefreshTokenRequest request, string? revokedBy = null)
        {
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

            var refreshToken = await refreshTokenRepo.Query(false)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken == null)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = "Refresh token not found"
                };
            }

            if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = "Refresh token is already revoked or expired"
                };
            }

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReasonRevoked = revokedBy ?? "User logout";

            await _unitOfWork.SaveChangesAsync();

            return new BaseServiceResponse
            {
                Success = true,
                Message = "Refresh token revoked successfully"
            };
        }

        public async Task<DataServiceResponse<bool>> ValidateRefreshToken(string token)
        {
            var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

            var refreshToken = await refreshTokenRepo.Query()
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken == null)
            {
                return new DataServiceResponse<bool>
                {
                    Success = false,
                    Message = "Refresh token not found",
                    Data = false
                };
            }

            var isValid = !refreshToken.IsRevoked && refreshToken.ExpiresAt > DateTime.UtcNow;

            return new DataServiceResponse<bool>
            {
                Success = isValid,
                Message = isValid ? "Valid refresh token" : "Invalid or expired refresh token",
                Data = isValid
            };
        }

    }
}
