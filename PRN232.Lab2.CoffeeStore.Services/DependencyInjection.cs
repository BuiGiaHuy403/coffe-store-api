using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRN232.Lab2.CoffeeStore.Services.Infrastructure;
using PRN232.Lab2.CoffeeStore.Services.Interfaces;
using PRN232.Lab2.CoffeeStore.Services.Interfaces.Services;
using PRN232.Lab2.CoffeeStore.Services.Models;
using PRN232.Lab2.CoffeeStore.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services
{
    public static class DependencyInjection
    {
        public static void AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(
                configuration.GetSection("Jwt"));

            services.Configure<MomoSettings>(
                configuration.GetSection("Momo"));

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenRevokeService, TokenRevokeService>();
            services.AddScoped<IAuthService, AuthService>();    
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddMemoryCache();

        }
    }
}
