using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Postgres.Extensions;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarketingBox.Registration.Postgres.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public LeadRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task SaveAsync(Lead lead)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entity = lead.CreateLeadEntity();
            var rowsCount = await ctx.Leads.Upsert(entity)
                .AllowIdentityMatch()
                .UpdateIf(prev => prev.Sequence < entity.Sequence)
                .RunAsync();

            if (rowsCount == 0)
            {
                throw new Exception($"Lead {lead.LeadInfo.LeadId} already updated, try to use most recent version");
            }
        }

        public async Task<long> GenerateLeadIdAsync(string tenantId, string generatorId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entity = new LeadIdGeneratorEntity()
            {
                TenantId = tenantId,
                GeneratorId = generatorId,
            };
            await ctx.LeadIdGenerators.AddAsync(entity);
            await ctx.SaveChangesAsync();
            return entity.LeadId;
        }

        public Task<Lead> RestoreAsync(long leadId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Lead> GetLeadByCustomerIdAsync(string tenantId, string customerId)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var existingLeadEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.TenantId == tenantId &&
                                                                              x.RouteInfoCustomerInfoCustomerId == customerId);

            if (existingLeadEntity == null)
            {
                throw new Exception($"Lead with customerId {customerId} can't be found");
            }

            return existingLeadEntity.RestoreLead();
        }

        public async Task<Lead> GetLeadByLeadIdAsync(string tenantId, long leadId)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var existingLeadEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.TenantId == tenantId &&
                                                                              x.LeadId == leadId);

            if (existingLeadEntity == null)
            {
                throw new Exception($"Lead with leadId {leadId} can't be found");
            }

            return existingLeadEntity.RestoreLead();
        }

        public async Task<int> GetCountForLeads(DateTime date, long campaignId, LeadStatus leadStatus)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var count = await ctx.Leads.Where(x => x.RouteInfoStatus == leadStatus &&
                                                   x.RouteInfoCampaignId == campaignId &&
                                                   x.CreatedAt.Date == date.Date).CountAsync();

            return count;
        }

        public async Task<int> GetCountForDeposits(DateTime date, long campaignId, LeadStatus leadStatus)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var count = await ctx.Leads.Where(x => x.RouteInfoStatus == leadStatus &&
                                                   x.RouteInfoCampaignId == campaignId &&
                                                   x.RouteInfoDepositDate != null && 
                                                   x.RouteInfoDepositDate.Value.Date == date.Date).CountAsync();

            return count;
        }
    }
}