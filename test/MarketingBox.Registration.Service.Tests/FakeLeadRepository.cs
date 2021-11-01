using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;

namespace MarketingBox.Registration.Service.Tests
{
    public class FakeLeadRepository : ILeadRepository
    {
        public int LeadCount { get; set; } = 0;

        public int FtdCount { get; set; } = 0;
        public Task SaveAsync(Lead lead)
        {
            if (lead.RouteInfo.Status == LeadStatus.Registered)
                LeadCount++;

            if (lead.RouteInfo.Status == LeadStatus.Approved)
                FtdCount++;

            return Task.CompletedTask;
        }

        public Task<long> GenerateLeadIdAsync(string tenantId, string generatorId)
        {
            throw new NotImplementedException();
        }

        public Task<Lead> RestoreAsync(long leadId)
        {
            throw new NotImplementedException();
        }

        public Task<Lead> GetLeadByCustomerIdAsync(string tenantId, string customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Lead> GetLeadByLeadIdAsync(string tenantId, long leadId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountForLeads(DateTime date, long campaignId, LeadStatus leadStatus)
        {
            return Task.FromResult(LeadCount);
        }

        public Task<int> GetCountForDeposits(DateTime date, long campaignId, LeadStatus leadStatus)
        {
            return Task.FromResult(FtdCount);
        }
    }
}