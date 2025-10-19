using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models
{
    public class MomoSettings
    {

        [Required] public string? PartnerCode { get; set; }

        [Required] public string? AccessKey { get; set; }

        [Required] public string? SecretKey { get; set; }

        [Required] public string? PaymentUrl { get; set; }

        [Required] public string? RedirectUrl { get; set; }

        [Required] public string? IpnUrl { get; set; }

        [Required] public string? RequestType { get; set; }
    }
}
