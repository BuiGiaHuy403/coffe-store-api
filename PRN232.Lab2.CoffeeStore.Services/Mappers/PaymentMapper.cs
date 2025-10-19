using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentResponse? ToPaymentResponse(this Payment? payment)
        {
            if (payment == null)
                return null;

            return new PaymentResponse
            {
                Id = payment.Id,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = nameof(payment.PaymentMethod)
            };
        }
    }
}
