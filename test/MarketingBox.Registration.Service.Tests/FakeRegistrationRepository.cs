using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Repositories;

namespace MarketingBox.Registration.Service.Tests
{
    public class FakeRegistrationRepository : IRegistrationRepository
    {
        public int LeadCount { get; set; } = 0;

        public int FtdCount { get; set; } = 0;
        public Task SaveAsync(RegistrationEntity registration)
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

        public Task<RegistrationEntity> RestoreAsync(long registrationId)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationEntity> GetLeadByCustomerIdAsync(string tenantId, string customerId)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationEntity> GetLeadByRegistrationIdAsync(string tenantId, long registrationId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountForRegistrations(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus)
        {
            return Task.FromResult(LeadCount);
        }

        public Task<int> GetCountForDeposits(DateTime date, long brandId, long campaignId, RegistrationStatus registrationStatus)
        {
            return Task.FromResult(FtdCount);
        }
    }
}