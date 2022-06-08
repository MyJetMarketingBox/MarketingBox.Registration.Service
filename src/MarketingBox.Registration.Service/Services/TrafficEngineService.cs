using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.Domain.Models.TrafficEngine;
using MarketingBox.Registration.Service.MyNoSql.TrafficEngine;
using MarketingBox.Registration.Service.Services.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.Services
{
    public class TrafficEngineService : ITrafficEngineService
    {
        private readonly IIntegrationService _service;
        private readonly ILogger<TrafficEngineService> _logger;
        private readonly IMyNoSqlServerDataReader<BrandCandidateNoSql> _brandCandidateNoSqlReader;
        private readonly IMyNoSqlServerDataWriter<BrandCandidateNoSql> _brandCandidateNoSqlWriter;
        private readonly IRouterFilterService _routerFilterService;
        private readonly IMapper _mapper;
        private readonly IMyNoSqlServerDataReader<IntegrationNoSql> _integrationNoSqlServerDataReader;
        private IBrandClient _brandClient;

        private BrandCandidate GetBrandCandidate(CampaignRowMessage campaignRowMessage)
        {
            var brand = _brandCandidateNoSqlReader.Get(
                BrandCandidateNoSql.GeneratePartitionKey(campaignRowMessage.TenantId),
                BrandCandidateNoSql.GenerateRowKey(campaignRowMessage.BrandId))?.BrandCandidate;

            var newBrand = new BrandCandidate
            {
                BrandId = campaignRowMessage.BrandId,
                DailyCapValue = campaignRowMessage.DailyCapValue,
                WeightCapValue = campaignRowMessage.Weight,
                Marked = brand?.Marked ?? false,
                UpdatedAt = DateTime.Today.DayOfWeek,
                SuccessfullySent = brand?.SuccessfullySent ?? false,
                CountOfSent = brand?.UpdatedAt == DateTime.Today.DayOfWeek ? brand.CountOfSent : 0,
                SentByWeight = brand?.UpdatedAt == DateTime.Today.DayOfWeek ? brand.SentByWeight : 0,
                TenantId = campaignRowMessage.TenantId
            };
            _brandCandidateNoSqlWriter
                .InsertOrReplaceAsync(BrandCandidateNoSql.Create(newBrand))
                .GetAwaiter()
                .GetResult();

            return newBrand;
        }

        private async Task<List<Priority>> GetTrafficEngineTree(long campaignId, int countryId, string tenantId)
        {
            // filter campaignRows by criteria 
            var campaignRows = await _routerFilterService.GetSuitableRoutes(campaignId, countryId, tenantId);

            // create traffic engine data structure
            var lookup = campaignRows.ToLookup(
                x => x.Priority,
                GetBrandCandidate);
            var priorities = lookup.Select(x => new Priority
            {
                PriorityValue = x.Key,
                Brands = x.ToList()
            }).ToList();
            return priorities;
        }

        private async Task<bool> ChooseBrand(
            List<BrandCandidate> brands,
            Domain.Models.Registrations.Registration registration)
        {
            do
            {
                Cleanup(brands);

                var freeBrands = brands
                    .Where(x => x.Marked == false)
                    .OrderByDescending(x => x.WeightCapValue)
                    .ToList();
                foreach (var brandCandidate in freeBrands)
                {
                    brandCandidate.Marked = true;
                    if (brandCandidate.CountOfSent >= brandCandidate.DailyCapValue ||
                        brandCandidate.SentByWeight >= brandCandidate.WeightCapValue)
                    {
                        await _brandCandidateNoSqlWriter.InsertOrReplaceAsync(
                            BrandCandidateNoSql.Create(brandCandidate));
                        continue;
                    }

                    try
                    {
                        var (brandNoSql, integrationNoSql) = await GetFromNoSql(registration.TenantId, brandCandidate.BrandId);

                        registration.BrandId = brandCandidate.BrandId;
                        registration.IntegrationId = integrationNoSql.Id;
                        registration.Integration = integrationNoSql.Name;

                        var registrationRequest = _mapper.Map<RegistrationRequest>(registration);

                        var response = await _service.SendRegisterationAsync(registrationRequest);

                        var result = response.Process();

                        registration.CustomerId = result.Customer.CustomerId;
                        registration.CustomerToken = result.Customer.Token;
                        registration.CustomerLoginUrl = result.Customer.LoginUrl;
                        registration.CustomerBrand = brandNoSql.Name;

                        ++brandCandidate.CountOfSent;
                        ++brandCandidate.SentByWeight;
                        brandCandidate.SuccessfullySent = true;

                        await _brandCandidateNoSqlWriter.InsertOrReplaceAsync(
                            BrandCandidateNoSql.Create(brandCandidate));
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Registration to brand failed {BrandId}", brandCandidate.BrandId);
                        brandCandidate.SuccessfullySent = false;
                        await _brandCandidateNoSqlWriter.InsertOrReplaceAsync(
                            BrandCandidateNoSql.Create(brandCandidate));
                    }
                }

                if (brands.All(x => !x.SuccessfullySent))
                {
                    break;
                }
            } while (brands.Any(x => x.SentByWeight < x.WeightCapValue));

            return false;
        }

        private async Task<(BrandMessage brandNoSql, IntegrationMessage integrationNoSql)> GetFromNoSql(string tenantId, long brandId)
        {
            var brandNoSql = await _brandClient.GetBrandById(brandId, tenantId, true);

            var integrationNoSql = _integrationNoSqlServerDataReader.Get(
                IntegrationNoSql.GeneratePartitionKey(tenantId),
                IntegrationNoSql.GenerateRowKey(brandNoSql.IntegrationId ?? default))?.Integration;
            if (integrationNoSql is null)
            {
                _logger.LogError(
                    $"{IntegrationNoSql.TableName} does not contain integration with id {brandNoSql.IntegrationId}");
                throw new NotFoundException("Integration with id", brandNoSql.IntegrationId);
            }

            return (brandNoSql, integrationNoSql);
        }


        private void Cleanup(List<BrandCandidate> brands)
        {
            if (brands.All(x => x.Marked))
            {
                brands.ForEach(x =>
                {
                    x.Marked = false;
                    x.SuccessfullySent = false;
                    _brandCandidateNoSqlWriter
                        .InsertOrReplaceAsync(BrandCandidateNoSql.Create(x))
                        .GetAwaiter()
                        .GetResult();
                });
            }

            if (brands.All(x => x.SentByWeight == x.WeightCapValue))
            {
                brands.ForEach(x =>
                {
                    x.SentByWeight = 0;
                    x.Marked = false;
                    x.SuccessfullySent = false;
                    _brandCandidateNoSqlWriter
                        .InsertOrReplaceAsync(BrandCandidateNoSql.Create(x))
                        .GetAwaiter()
                        .GetResult();
                });
            }
        }

        public TrafficEngineService(
            IIntegrationService service,
            ILogger<TrafficEngineService> logger,
            IRouterFilterService routerFilterService,
            IMyNoSqlServerDataReader<BrandCandidateNoSql> brandCandidateNoSqlReader,
            IMyNoSqlServerDataWriter<BrandCandidateNoSql> brandCandidateNoSqlWriter,
            IMapper mapper,
            IMyNoSqlServerDataReader<IntegrationNoSql> integrationNoSqlServerDataReader,
            IBrandClient brandClient)
        {
            _service = service;
            _logger = logger;
            _routerFilterService = routerFilterService;
            _brandCandidateNoSqlReader = brandCandidateNoSqlReader;
            _brandCandidateNoSqlWriter = brandCandidateNoSqlWriter;
            _mapper = mapper;
            _brandClient = brandClient;
            _integrationNoSqlServerDataReader = integrationNoSqlServerDataReader;
        }

        public async Task<bool> TryRegisterAsync(long campaignId, int countryId,
            Domain.Models.Registrations.Registration registration)
        {
            var priorities = await GetTrafficEngineTree(campaignId, countryId, registration.TenantId);

            // run algorithm among all priorities
            foreach (var campaignPriority in priorities.OrderBy(x => x.PriorityValue))
            {
                if (await ChooseBrand(campaignPriority.Brands, registration))
                {
                    return true;
                }
            }

            return false;
        }
    }
}