using PRN232.Lab2.CoffeeStore.API.Models.Responses;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;

namespace PRN232.Lab2.CoffeeStore.API.Mappers;

public static class BaseServiceResponseMapper
{
    public static BaseApiResponse ToBaseApiResponse(this BaseServiceResponse baseServiceResponse)
    {
        return new BaseApiResponse()
        {
            Success = baseServiceResponse.Success,
            Message = baseServiceResponse.Message
        };
    }
}