using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Infrastructure
{
    public interface ITokenRevokeService
    {
        Task<BaseServiceResponse> RevokeToken(RevokeRefreshTokenRequest request, string? revokedBy = null);
        Task<BaseServiceResponse> RevokeAllUserTokens(string userId, string? revokedBy = null);
        Task<DataServiceResponse<bool>> ValidateRefreshToken(string token);
        Task<BaseServiceResponse> CleanupExpiredTokens();
    }

}

