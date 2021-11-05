using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Service.Domain.Repositories
{
    public interface IRegistrationRepository
    {
        Task SaveAsync(Leads.Registration registration);
        Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId);
        Task<Leads.Registration> RestoreAsync(long registrationId);
        Task<Leads.Registration> GetLeadByCustomerIdAsync(string tenantId, string customerId);
        Task<Leads.Registration> GetLeadByRegistrationIdAsync(string tenantId, long registrationId);
        Task<int> GetCountForRegistrations(DateTime date, long brandId, RegistrationStatus registrationStatus);
        Task<int> GetCountForDeposits(DateTime date, long brandId, RegistrationStatus registrationStatus);
    }
}