using Microsoft.AspNetCore.Identity;
using PRN232.Lab2.CoffeeStore.Repositories.Interfaces;
using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Infrastructure;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(ITokenService tokenService, IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<DataServiceResponse<TokenResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new DataServiceResponse<TokenResponse>
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Data = null!
                };
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                lockoutOnFailure: true
            );

            if (signInResult.IsLockedOut)
            {
                return new DataServiceResponse<TokenResponse>
                {
                    Success = false,
                    Message = "Account locked. Too many failed attempts.",
                    Data = null!
                };
            }


            if (!signInResult.Succeeded)
            {
                return new DataServiceResponse<TokenResponse>
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Data = null!
                };
            }

            var tokenResponse = await _tokenService.GenerateTokens(user.Id);
            if (!tokenResponse.Success)
            {
                return new DataServiceResponse<TokenResponse>()
                {
                    Success = false,
                    Message = "Failed to generate tokens",
                    Data = null!
                };
            }

            return new DataServiceResponse<TokenResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = tokenResponse.Data
            };
        }

        public async Task<BaseServiceResponse> RegisterAsync(RegisterRequest request, string role)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

           
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = string.Join(", ", createResult.Errors.Select(e => e.Description))
                };
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = string.Join(", ", roleResult.Errors.Select(e => e.Description))
                };
            }

            await transaction.CommitAsync(); // ✅ commit only on success

            return new BaseServiceResponse
            {
                Success = true,
                Message = "Registration successful"
            };

        }
    }
}
