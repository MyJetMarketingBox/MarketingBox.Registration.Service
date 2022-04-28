using System.Threading.Tasks;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.Domain.Models.TrafficEngine;

namespace MarketingBox.Registration.Service.Services.Interfaces
{
    public interface ITrafficEngineService
    {
        Task<bool> TryRegister(long campaignId, int countryId,
            Domain.Models.Registrations.Registration registration);
    }
}