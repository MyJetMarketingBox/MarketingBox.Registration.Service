using System.Threading.Tasks;

namespace MarketingBox.Registration.Service.Services.Interfaces
{
    public interface ITrafficEngineService
    {
        Task<bool> TryRegisterAsync(long campaignId, int countryId,
            Domain.Models.Registrations.Registration registration);
    }
}