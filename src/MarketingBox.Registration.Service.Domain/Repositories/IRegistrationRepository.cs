using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;

namespace MarketingBox.Registration.Service.Domain.Repositories
{
    public interface IRegistrationRepository
    {
        Task SaveAsync(RegistrationEntity registration);
        Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId);
        Task<RegistrationEntity> RestoreAsync(long registrationId);
        Task<RegistrationEntity> GetLeadByCustomerIdAsync(string tenantId, string customerId);
        Task<RegistrationEntity> GetRegistrationByIdAsync(string tenantId, long registrationId);
        Task<int> GetCountForRegistrations(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus);
        Task<int> GetCountForDeposits(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus);
        Task SaveStatusChangeLogAsync(StatusChangeLog log);
        Task<List<StatusChangeLog>> GetStatusChangeLogAsync(GetStatusChangeLogRequest request);
    }
}