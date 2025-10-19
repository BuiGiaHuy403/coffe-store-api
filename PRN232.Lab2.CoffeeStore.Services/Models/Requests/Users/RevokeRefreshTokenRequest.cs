using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users
{
    public class RevokeRefreshTokenRequest : BaseServiceRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}