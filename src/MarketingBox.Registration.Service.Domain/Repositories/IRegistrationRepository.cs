using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Registration.Service.Domain.Repositories
{
    public interface IRegistrationRepository
    {
        Task SaveAsync(Models.Registrations.Registration registration);
        Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId);
        Task<Models.Registrations.Registration> RestoreAsync(string tenantId, long registrationId);
        Task<Models.Registrations.Registration> GetLeadByCustomerIdAsync(string tenantId, string customerId);
        Task<Models.Registrations.Registration> GetRegistrationByIdAsync(string tenantId, long registrationId);
        Task<int> GetCountForRegistrations(DateTime date,string tenantId, long brandId, long campaignId, RegistrationStatus registrationStatus);
        Task<int> GetCountForDeposits(DateTime date,string tenantId, long brandId, long campaignId, RegistrationStatus registrationStatus);
        Task SaveStatusChangeLogAsync(StatusChangeLog log);
        Task<List<StatusChangeLog>> GetStatusChangeLogAsync(GetStatusChangeLogRequest request);
    }
}