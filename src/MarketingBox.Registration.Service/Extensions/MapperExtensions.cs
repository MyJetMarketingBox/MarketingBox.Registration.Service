using System;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class MapperExtensions
    {
        public static string GeneratorId(this RegistrationGeneralInfo request)
        {
            return request.Email + "_" +
                   request.FirstName + "_" +
                   request.LastName + "_" +
                   request.Ip + "_" +
                   DateTime.UtcNow.ToString("O");
        }
    }
}
