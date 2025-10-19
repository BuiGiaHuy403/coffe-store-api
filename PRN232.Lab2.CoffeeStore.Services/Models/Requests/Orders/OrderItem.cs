using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Orders
{
    public class OrderItem
    {

        public Guid ProductId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "The value must not be negative.")]

        public int Quantity { get; set; }
    }
}
