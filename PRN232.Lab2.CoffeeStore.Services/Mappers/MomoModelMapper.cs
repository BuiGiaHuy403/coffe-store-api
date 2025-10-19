using Newtonsoft.Json;
using PRN232.Lab2.CoffeeStore.API.Models.Requests;
using PRN232.Lab2.CoffeeStore.Services.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Momo;
using PRN232.Lab2.CoffeeStore.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Mappers
{
    public static class MomoModelMapper
    {
        public static MomoOneTimePayment ToMomoOneTimePayment(
         this MomoCreatePaymentRequest request,
         MomoSettings options, long Amount)
        {
            var orderId = request.OrderId.ToString();
            var requestId = Guid.NewGuid().ToString();

            // Serialize OrderDetails to base64 extraData
            string extraDataJson = JsonConvert.SerializeObject( new());
            string extraDataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(extraDataJson));

            var momoModel = new MomoOneTimePayment
            {
                PartnerCode = options.PartnerCode,
                OrderId = orderId,
                RequestId = requestId,
                Amount = Amount,
                OrderInfo = $"Thanh toán Coffe Store FPTU",
                RedirectUrl = options.RedirectUrl,
                IpnUrl = options.IpnUrl,
                RequestType = options.RequestType,
                ExtraData = extraDataBase64,
                Lang = "vi"
            };

            momoModel.MakeSignature(options.AccessKey ?? "", options.SecretKey ?? "");

            return momoModel;
        }
        public static string GenerateSignature(this MomoConfirmPaymentRequest request, MomoSettings options)
        {
            var rawHash = "accessKey=" + options.AccessKey +
                          "&amount=" + request.Amount +
                          "&extraData=" + request.ExtraData +
                          "&message=" + request.Message +
                          "&orderId=" + request.OrderId +
                          "&orderInfo=" + request.OrderInfo +
                          "&orderType=" + request.OrderType +
                          "&partnerCode=" + request.PartnerCode +
                          "&payType=" + request.PayType +
                          "&requestId=" + request.RequestId +
                          "&responseTime=" + request.ResponseTime +
                          "&resultCode=" + request.resultCode +
                          "&transId=" + request.TransId;

            return HashHelper.HmacSHA256(rawHash, options.SecretKey ?? "");
        }

        public static bool IsValidSignature(this MomoConfirmPaymentRequest request, MomoSettings options)
        {
            var expected = request.GenerateSignature(options);
            return string.Equals(expected, request.Signature, StringComparison.OrdinalIgnoreCase);
        }
    }
}
