﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Registration.Service.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly DatabaseContextFactory _contextFactory;
        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisher;
        private readonly IMapper _mapper;

        public RegistrationRepository(DatabaseContextFactory contextFactory, 
            IServiceBusPublisher<RegistrationUpdateMessage> publisher, 
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _publisher = publisher;
            _mapper = mapper;
        }

        public async Task SaveAsync(Domain.Models.Registrations.Registration registration)
        {
            await using var ctx = _contextFactory.Create();
            var rowsCount = await ctx.Registrations.Upsert(registration)
                .AllowIdentityMatch()
                .RunAsync();
            
            await _publisher
                .PublishAsync(_mapper.Map<RegistrationUpdateMessage>(registration));

            if (rowsCount == 0)
            {
                throw new BadRequestException(
                    $"Registration {registration.Id} already updated, try to use most recent version");
            }
        }

        public async Task<long> GenerateRegistrationIdAsync(string tenantId, string generatorId)
        {
            await using var ctx = _contextFactory.Create();
            var entity = new RegistrationIdGeneratorEntity()
            {
                TenantId = tenantId,
                GeneratorId = generatorId,
            };
            await ctx.RegistrationIdGenerators.AddAsync(entity);
            await ctx.SaveChangesAsync();
            return entity.RegistrationId;
        }

        public Task<Domain.Models.Registrations.Registration> RestoreAsync(long registrationId)
        {
            throw new NotImplementedException();
        }

        public async Task<Domain.Models.Registrations.Registration> GetLeadByCustomerIdAsync(string tenantId, string customerId)
        {
            await using var ctx = _contextFactory.Create();
            var existingLeadEntity = await ctx.Registrations.FirstOrDefaultAsync(x => x.TenantId == tenantId &&
                                                                              x.CustomerId == customerId);

            if (existingLeadEntity == null)
            {
                throw new NotFoundException(nameof(customerId), customerId);
            }

            return existingLeadEntity;
        }

        public async Task<Domain.Models.Registrations.Registration> GetRegistrationByIdAsync(string tenantId, long registrationId)
        {
            await using var ctx = _contextFactory.Create();
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
            await using var ctx = _contextFactory.Create();
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
            await using var ctx = _contextFactory.Create();
            var nextDate = date.AddDays(1).Date;
            var count = await ctx.Registrations.Where(x => x.Status == registrationStatus &&
                                                   x.BrandId == brandId &&
                                                   x.CampaignId == campaignId &&
                                                   x.ConversionDate != null && 
                                                   x.ConversionDate.Value >= date.Date && x.ConversionDate.Value < nextDate).CountAsync();

            return count;
        }

        public async Task SaveStatusChangeLogAsync(StatusChangeLog log)
        {
            await using var ctx = _contextFactory.Create();
            await ctx.StatusChangeLogs.AddAsync(log);
            await ctx.SaveChangesAsync();
        }

        public async Task<List<StatusChangeLog>> GetStatusChangeLogAsync(GetStatusChangeLogRequest request)
        {
            await using var ctx = _contextFactory.Create();

            IQueryable<StatusChangeLog> query = ctx.StatusChangeLogs;

            if (request.Mode.HasValue)
                query = query.Where(e => e.Mode == request.Mode);
            if (request.UserId.HasValue)
                query = query.Where(e => e.UserId == request.UserId);
            if (request.RegistrationId.HasValue)
                query = query.Where(e => e.RegistrationId == request.RegistrationId);

            return await query.ToListAsync();
        }
    }
}