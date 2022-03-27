using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketingBox.Registration.Postgres.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public RegistrationRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task SaveAsync(RegistrationEntity registration)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var rowsCount = await ctx.Registrations.Upsert(registration)
                .AllowIdentityMatch()
                .RunAsync();

            if (rowsCount == 0)
            {
                throw new BadRequestException(
                    $"Registration {registration.Id} already updated, try to use most recent version");
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

        public Task<RegistrationEntity> RestoreAsync(long registrationId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<RegistrationEntity> GetLeadByCustomerIdAsync(string tenantId, string customerId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var existingLeadEntity = await ctx.Registrations.FirstOrDefaultAsync(x => x.TenantId == tenantId &&
                                                                              x.CustomerId == customerId);

            if (existingLeadEntity == null)
            {
                throw new NotFoundException(nameof(customerId), customerId);
            }

            return existingLeadEntity;
        }

        public async Task<RegistrationEntity> GetLeadByRegistrationIdAsync(string tenantId, long registrationId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var existingLeadEntity = await ctx.Registrations
                .FirstOrDefaultAsync(x => x.TenantId == tenantId &&
                                          x.Id == registrationId);

            if (existingLeadEntity == null)
            {
                throw new NotFoundException(nameof(registrationId), registrationId);
            }

            return existingLeadEntity;
        }

        public async Task<int> GetCountForRegistrations(
            DateTime date,
            long brandId,
            long campaignId,
            RegistrationStatus registrationStatus)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
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
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var nextDate = date.AddDays(1).Date;
            var count = await ctx.Registrations.Where(x => x.Status == registrationStatus &&
                                                   x.BrandId == brandId &&
                                                   x.CampaignId == campaignId &&
                                                   x.ConversionDate != null && 
                                                   x.ConversionDate.Value >= date.Date && x.ConversionDate.Value < nextDate).CountAsync();

            return count;
        }
    }
}