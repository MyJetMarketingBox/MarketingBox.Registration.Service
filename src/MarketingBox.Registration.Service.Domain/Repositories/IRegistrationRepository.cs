using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Registrations;

namespace MarketingBox.Registration.Service.Domain.Repositories
{
    public interface IRegistrationRepository
    {
        Task SaveAsync(Registrations.Registration registration);
        Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId);
        Task<Registrations.Registration> RestoreAsync(long registrationId);
        Task<Registrations.Registration> GetLeadByCustomerIdAsync(string tenantId, string customerId);
        Task<Registrations.Registration> GetLeadByRegistrationIdAsync(string tenantId, long registrationId);
        Task<int> GetCountForRegistrations(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus);
        Task<int> GetCountForDeposits(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus);
    }
}