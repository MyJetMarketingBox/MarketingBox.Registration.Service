using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Service.Domain.Repositories
{
    public interface ILeadRepository
    {
        Task SaveAsync(Lead lead);
        Task<long> GenerateLeadIdAsync(string tenantId, string generatorId);
        Task<Lead> RestoreAsync(long leadId);
        Task<Lead> GetLeadByCustomerIdAsync(string tenantId, string customerId);
        Task<Lead> GetLeadByLeadIdAsync(string tenantId, long leadId);
        Task<int> GetCountForLeads(DateTime date, long campaignId, LeadStatus leadStatus);
        Task<int> GetCountForDeposits(DateTime date, long campaignId, LeadStatus leadStatus);
    }
}