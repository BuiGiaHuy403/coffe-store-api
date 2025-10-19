using PRN232.Lab2.CoffeeStore.API.Models.Requests;
using PRN232.Lab2.CoffeeStore.API.Models.Responses;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Momo;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<DataServiceResponse<MomoCreatePaymentResponse>> CreatePaymentWithMomo(MomoCreatePaymentRequest request);
        Task<BaseServiceResponse> ConfirmMomoPaymentAsync(MomoConfirmPaymentRequest request);
    }
}
