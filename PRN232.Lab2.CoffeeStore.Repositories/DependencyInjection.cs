using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRN232.Lab2.CoffeeStore.Repositories.Interfaces;
using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Repositories
{
    public static class DependencyInjection
    {

        public static void AddDataServices(this IServiceCollection services, IConfiguration configurtation)
        {
            //services.AddDataProtection();

            //services.AddSingleton<TimeProvider>(TimeProvider.System);

            var connectionString = configurtation.GetConnectionString("DefaultConnection");
            services.AddDbContext<CoffeeStoreDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;
            })
               .AddSignInManager()
               .AddRoles<IdentityRole>()
               .AddDefaultTokenProviders()
               .AddEntityFrameworkStores<CoffeeStoreDbContext>();

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
