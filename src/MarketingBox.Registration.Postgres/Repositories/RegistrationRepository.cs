using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Registration.Postgres.Entities.Registration;
using MarketingBox.Registration.Postgres.Extensions;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Registration.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarketingBox.Registration.Postgres.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public RegistrationRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task SaveAsync(Service.Domain.Registrations.Registration registration)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entity = registration.CreateRegistrationEntity();
            var rowsCount = await ctx.Registrations.Upsert(entity)
                .AllowIdentityMatch()
                .UpdateIf(prev => prev.Sequence < entity.Sequence)
                .RunAsync();

            if (rowsCount == 0)
            {
                throw new Exception($"Registration {registration.RegistrationInfo.RegistrationId} already updated, try to use most recent version");
            }
        }

        public async Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entity = new RegistrationIdGeneratorEntity()
            {
                TenantId = tenantId,
                GeneratorId = generatorId,
            };
            await ctx.RegistrationIdGenerators.AddAsync(entity);
            await ctx.SaveChangesAsync();
            return entity.RegistrationId;
        }

        public Task<Service.Domain.Registrations.Registration> RestoreAsync(long registrationId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Service.Domain.Registrations.Registration> GetLeadByCustomerIdAsync(string tenantId, string customerId)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var existingLeadEntity = await ctx.Registrations.FirstOrDefaultAsync(x => x.TenantId == tenantId &&
                                                                              x.CustomerId == customerId);

            if (existingLeadEntity == null)
            {
                throw new Exception($"Registration with customerId {customerId} can't be found");
            }

            return existingLeadEntity.RestoreRegistration();
        }

        public async Task<Service.Domain.Registrations.Registration> GetLeadByRegistrationIdAsync(string tenantId, long registrationId)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var existingLeadEntity = await ctx.Registrations.FirstOrDefaultAsync(x => x.TenantId == tenantId &&
                                                                              x.Id == registrationId);

            if (existingLeadEntity == null)
            {
                throw new Exception($"Registration with registrationId {registrationId} can't be found");
            }

            return existingLeadEntity.RestoreRegistration();
        }

        public async Task<int> GetCountForRegistrations(
            DateTime date,
            long brandId,
            long campaignId,
            RegistrationStatus registrationStatus)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var nextDate = date.AddDays(1).Date;
            var count = await ctx.Registrations.Where(x => x.Status == registrationStatus &&
                                                   x.BrandId == brandId &&
                                                   x.CampaignId == campaignId &&
                                                   x.CreatedAt >= date.Date && x.CreatedAt < nextDate).CountAsync();

            return count;
        }

        public async Task<int> GetCountForDeposits(
            DateTime date,
            long brandId,
            long campaignId,
            RegistrationStatus registrationStatus)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var nextDate = date.AddDays(1).Date;
            var count = await ctx.Registrations.Where(x => x.Status == registrationStatus &&
                                                   x.BrandId == brandId &&
                                                   x.CampaignId == campaignId &&
                                                   x.DepositDate != null && 
                                                   x.DepositDate.Value >= date && x.DepositDate.Value < nextDate).CountAsync();

            return count;
        }
    }
}