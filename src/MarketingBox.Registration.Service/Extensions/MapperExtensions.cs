using System;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class MapperExtensions
    {
        public static string GeneratorId(this RegistrationCreateRequest request)
        {
            return request.GeneralInfo.Email + "_" +
                   request.GeneralInfo.FirstName + "_" +
                   request.GeneralInfo.LastName + "_" +
                   request.GeneralInfo.Ip + "_" +
                   DateTime.UtcNow.ToString("O");
        }
    }
}
