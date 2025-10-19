using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Infrastructure
{
    public interface ITokenService
    {
        Task<DataServiceResponse<TokenResponse>> GenerateTokens(string userId);

        Task<DataServiceResponse<TokenResponse>> RefreshTokens(RefreshTokenRequest request);
    }
}
