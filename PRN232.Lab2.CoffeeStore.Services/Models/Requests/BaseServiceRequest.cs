using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests
{
    public class BaseServiceRequest
    {
        public string Format { get; set; } = "json";
    }
}
