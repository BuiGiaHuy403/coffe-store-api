using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Responses
{
    public class DataServiceResponse<T> : BaseServiceResponse
    {
        public T Data { get; set; } = default!;
    }
}
