using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Users
{
    public class RefreshTokenRequest : BaseServiceRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
