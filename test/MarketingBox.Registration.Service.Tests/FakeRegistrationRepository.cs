using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Registration.Service.Tests
{
    public class FakeRegistrationRepository : IRegistrationRepository
    {
        public int LeadCount { get; set; } = 0;

        public int FtdCount { get; set; } = 0;
        public Task SaveAsync(Domain.Models.Registrations.Registration registration)
        {
            if (registration.Status == RegistrationStatus.Registered)
                LeadCount++;

            if (registration.Status == RegistrationStatus.Approved)
                FtdCount++;

            return Task.CompletedTask;
        }

        public Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Models.Registrations.Registration> RestoreAsync(string tenantId, long registrationId)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Models.Registrations.Registration> GetLeadByCustomerIdAsync(string tenantId, string customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Models.Registrations.Registration> GetRegistrationByIdAsync(string tenantId, long registrationId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountForRegistrations(DateTime date, string tenantId, long brandId, long campaignId, RegistrationStatus registrationStatus)
        {
            return Task.FromResult(LeadCount);
        }

        public Task<int> GetCountForDeposits(DateTime date, string tenantId, long brandId, long campaignId, RegistrationStatus registrationStatus)
        {
            return Task.FromResult(FtdCount);
        }

        public Task SaveStatusChangeLogAsync(StatusChangeLog log)
        {
            throw new NotImplementedException();
        }

        public Task<List<StatusChangeLog>> GetStatusChangeLogAsync(GetStatusChangeLogRequest request)
        {
            throw new NotImplementedException();
        }
    }
}