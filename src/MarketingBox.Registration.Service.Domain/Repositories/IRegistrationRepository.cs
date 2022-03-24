using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Registrations;

namespace MarketingBox.Registration.Service.Domain.Repositories
{
    public interface IRegistrationRepository
    {
        Task SaveAsync(Models.Registrations.Registration_nogrpc registrationNogrpc);
        Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId);
        Task<Models.Registrations.Registration_nogrpc> RestoreAsync(long registrationId);
        Task<Models.Registrations.Registration_nogrpc> GetLeadByCustomerIdAsync(string tenantId, string customerId);
        Task<Models.Registrations.Registration_nogrpc> GetLeadByRegistrationIdAsync(string tenantId, long registrationId);
        Task<int> GetCountForRegistrations(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus);
        Task<int> GetCountForDeposits(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus);
    }
}