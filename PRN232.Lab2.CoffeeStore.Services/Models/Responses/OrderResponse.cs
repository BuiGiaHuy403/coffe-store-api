using PRN232.Lab2.CoffeeStore.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Responses
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; } 
        public string UserId { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public int TotalItems { get; set; }

        public OrderStatus Status { get; set; }

        public Guid? PaymentId { get; set; }
    }
}
