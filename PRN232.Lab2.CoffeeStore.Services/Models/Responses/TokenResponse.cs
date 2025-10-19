using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Responses
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        // Access token
        public int AccessTokenExpiresIn { get; set; }        // seconds until expiry
        public DateTime AccessTokenExpiresAtUtc { get; set; } // absolute expiry time

        // Refresh token
        public int RefreshTokenExpiresIn { get; set; }        // seconds until expiry
        public DateTime RefreshTokenExpiresAtUtc { get; set; } // absolute expiry time
    }
}
