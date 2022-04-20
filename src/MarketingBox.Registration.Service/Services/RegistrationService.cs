﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.ExternalReferenceProxy.Service.Grpc;
using MarketingBox.ExternalReferenceProxy.Service.Grpc.Models;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Domain.Models.Route;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _affiliateNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignIndexNoSql> _campaignIndexNoSqlServerDataReader;
        private readonly ICountryClient _countryClient;
        private readonly IExternalReferenceProxyService _externalReferenceProxyService;
        private readonly IMyNoSqlServerDataReader<IntegrationNoSql> _integrationNoSqlServerDataReader;
        private readonly IIntegrationService _integrationService;
        private readonly ILogger<RegistrationService> _logger;
        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IRegistrationRouterService _registrationRouter;
        private readonly IRegistrationRepository _repository;
        private readonly IMapper _mapper;

        // Todo: implement proper logic for multi-tenancy and get rid of this constant.
        private const string TenantId = "default-tenant-id";

        public RegistrationService(ILogger<RegistrationService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataReader<CampaignIndexNoSql> campaignIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<IntegrationNoSql> integrationNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> brandNoSqlServerDataReader,
            IIntegrationService integrationService,
            IRegistrationRepository repository,
            IRegistrationRouterService registrationRouter,
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader,
            IExternalReferenceProxyService externalReferenceProxyService,
            ICountryClient countryClient,
            IMapper mapper)
        {
            _logger = logger;
            _publisherLeadUpdated = publisherLeadUpdated;
            _campaignIndexNoSqlServerDataReader = campaignIndexNoSqlServerDataReader;
            _integrationNoSqlServerDataReader = integrationNoSqlServerDataReader;
            _brandNoSqlServerDataReader = brandNoSqlServerDataReader;
            _integrationService = integrationService;
            _repository = repository;
            _registrationRouter = registrationRouter;
            _affiliateNoSqlServerDataReader = affiliateNoSqlServerDataReader;
            _externalReferenceProxyService = externalReferenceProxyService;
            _countryClient = countryClient;
            _mapper = mapper;
        }

        public async Task<Response<Domain.Models.Registrations.Registration>> CreateAsync(
            RegistrationCreateRequest request)
        {
            try
            {
                request.ValidateEntity();
                _logger.LogInformation("Creating new Registration {@context}", request);
                // var tenantId = GetTenantId(request.AuthInfo.CampaignId.Value);
                // if (tenantId == null)
                //     throw new BadRequestException(new Error
                //     {
                //         ErrorMessage = BadRequestException.DefaultErrorMessage,
                //         ValidationErrors = new List<ValidationError>
                //         {
                //             new()
                //             {
                //                 ParameterName = nameof(request.AuthInfo.CampaignId),
                //                 ErrorMessage = $"Incorrect {nameof(request.AuthInfo.CampaignId)} '{request.AuthInfo.CampaignId}'"
                //             }
                //         }
                //     });

                if (!IsAffiliateApiKeyValid(TenantId, request.AuthInfo.AffiliateId.Value,
                        request.AuthInfo.ApiKey, out var affiliateName))
                    throw new UnauthorizedException(
                        $"Required authentication for affiliate '{request.AuthInfo.AffiliateId}'");

                var registrationId = await _repository.GenerateRegistrationIdAsync(TenantId,
                    request.GeneratorId());

                var country = await GetCountry(
                    request.GeneralInfo.CountryCodeType.Value,
                    request.GeneralInfo.CountryCode);

                var registration = _mapper.Map<RegistrationEntity>(request);
                registration.UniqueId = UniqueIdGenerator.GetNextId();
                registration.Country = country.Alfa2Code;
                registration.CountryId = country.Id;
                registration.AffiliateName = affiliateName;
                registration.Id = registrationId;
                registration.TenantId = TenantId;

                Domain.Models.Registrations.Registration response = null;

                switch (request.RegistrationMode)
                {
                    case RegistrationMode.Auto:
                        response = await AutoRegistration(request, country, registration);
                        if (response is null)
                        {
                            throw new Exception("Could not register to brand.");
                        }
                        break;
                    case RegistrationMode.Manual:
                        break;
                    case RegistrationMode.S2S:
                        registration.Status = RegistrationStatus.Registered;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await SaveAndPublishRegistration(request, registration);

                if (request.RegistrationMode == RegistrationMode.S2S)
                {
                    response = _mapper.Map<Domain.Models.Registrations.Registration>(registration);
                    response.OriginalData.CountryCodeType = request.GeneralInfo.CountryCodeType;
                    response.OriginalData.CountryCode = request.GeneralInfo.CountryCode;
                }

                return new Response<Domain.Models.Registrations.Registration>
                {
                    Data = response,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return e.FailedResponse<Domain.Models.Registrations.Registration>();
            }
        }

        private async Task<Domain.Models.Registrations.Registration> AutoRegistration(
            RegistrationCreateRequest request,
            Country country,
            RegistrationEntity registration)
        {
            var routes =
                await _registrationRouter.GetSuitableRoutes(request.AuthInfo.CampaignId.Value, country.Id);

            if (!routes.Any())
            {
                await _repository.SaveAsync(registration);
                throw new Exception("Can't register on brand");
            }

            while (routes.Count > 0)
            {
                var route = await TryGetSpecificRoute(request.AuthInfo.CampaignId.Value,
                    request.GeneralInfo.CountryCode,
                    routes);

                if (route == null)
                {
                    await SaveAndPublishRegistration(request, registration);
                    throw new Exception("Can't register on brand");
                }

                routes.RemoveAll(x =>
                    x.BrandId == route.BrandId &&
                    x.CampaignId == route.CampaignId &&
                    x.Id == route.CampaignRowId);

                registration.BrandId = route.BrandId;
                registration.CampaignId = route.CampaignId;
                try
                {
                    return await GetRegistrationCreateResponse(request, registration);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        private async Task<Country> GetCountry(CountryCodeType countryCodeType, string countryCode)
        {
            var countries = await _countryClient.GetCountries();
            var country = countryCodeType switch
            {
                CountryCodeType.Numeric => countries.FirstOrDefault(x => x.Numeric == countryCode),
                CountryCodeType.Alfa2Code => countries.FirstOrDefault(x => x.Alfa2Code == countryCode),
                CountryCodeType.Alfa3Code => countries.FirstOrDefault(x => x.Alfa3Code == countryCode),
                _ => throw new ArgumentOutOfRangeException(nameof(countryCodeType), countryCodeType, null)
            };
            if (country is null)
                throw new BadRequestException(new Error
                {
                    ErrorMessage = BadRequestException.DefaultErrorMessage,
                    ValidationErrors = new List<ValidationError>
                    {
                        new ValidationError
                        {
                            ErrorMessage = $"There is no country with code {countryCodeType}:{countryCode}",
                            ParameterName = nameof(countryCode)
                        }
                    }
                });

            return country;
        }

        private async Task<Domain.Models.Registrations.Registration> GetRegistrationCreateResponse(
            RegistrationCreateRequest request,
            RegistrationEntity registrationEntity)
        {
            var brandResponse = await BrandRegisterAsync(registrationEntity);

            _logger.LogInformation("Brand request: {@context}. Brand response: {@response}", request, brandResponse);

            registrationEntity.CustomerId = brandResponse.CustomerId;
            registrationEntity.CustomerLoginUrl = brandResponse.LoginUrl;
            registrationEntity.CustomerToken = brandResponse.Token;
            registrationEntity.CustomerBrand = brandResponse.Brand;
            return _mapper.Map<Domain.Models.Registrations.Registration>(registrationEntity);
        }

        private async Task SaveAndPublishRegistration(RegistrationCreateRequest request,
            RegistrationEntity registration)
        {
            await _repository.SaveAsync(registration);

            await _publisherLeadUpdated.PublishAsync(_mapper.Map<RegistrationUpdateMessage>(registration));
            _logger.LogInformation("Sent original created registration to service bus {@context}", request);
        }

        private string GetTenantId(long affiliateId)
        {
            var boxIndexNoSql = _campaignIndexNoSqlServerDataReader
                .Get(CampaignIndexNoSql.GeneratePartitionKey(affiliateId))
                .FirstOrDefault()
                ?.Campaign;

            return boxIndexNoSql?.TenantId;
        }

        private bool IsAffiliateApiKeyValid(string tenantId, long affiliateId, string apiKey, out string affiliateName)
        {
            var partner =
                _affiliateNoSqlServerDataReader
                    .Get(AffiliateNoSql.GeneratePartitionKey(tenantId),
                        AffiliateNoSql.GenerateRowKey(affiliateId))?
                    .Affiliate;

            if (partner == null)
            {
                affiliateName = string.Empty;
                return false;
            }

            var partnerApiKey = partner.GeneralInfo.ApiKey;

            affiliateName = partner.GeneralInfo.Username;

            return partnerApiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<RouteParameters> TryGetSpecificRoute(long campaignId, string country,
            List<CampaignRowMessage> filtered)
        {
            try
            {
                var boxIndexNoSql = _campaignIndexNoSqlServerDataReader
                    .Get(CampaignIndexNoSql.GeneratePartitionKey(campaignId)).Select(x => x.Campaign).FirstOrDefault();
                var tenantId = boxIndexNoSql?.TenantId;

                var campaignRow = await _registrationRouter.GetCampaignRow(tenantId, campaignId, filtered);

                if (campaignRow == null)
                    return null;

                var brandNoSql = _brandNoSqlServerDataReader.Get(
                    BrandNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    BrandNoSql.GenerateRowKey(campaignRow.BrandId)).Brand;

                var brandId = brandNoSql.Id;

                var integrationNoSql = _integrationNoSqlServerDataReader.Get(
                    IntegrationNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    IntegrationNoSql.GenerateRowKey(brandNoSql.IntegrationId ?? default)).Integration;

                var brandName = integrationNoSql.Name;
                var integrationId = integrationNoSql.Id;
                return new RouteParameters
                {
                    IntegrationId = integrationId,
                    BrandName = brandName,
                    BrandId = brandId,
                    TenantId = tenantId,
                    CampaignId = campaignId,
                    CampaignRowId = campaignRow.Id
                };
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Can't TryGetRouteInfo. CampaignId: {@Campaign} countryCode: {@CountryCode} Error: {@Error}",
                    campaignId, country, e.Message);
            }

            return null;
        }

        private async Task<RegistrationBrandInfo> BrandRegisterAsync(
            RegistrationEntity registrationEntity)
        {
            var request =
                _mapper.Map<RegistrationRequest>(registrationEntity); //registrationNogrpc.CreateIntegrationRequest();
            var response = await _integrationService.SendRegisterationAsync(request);

            var proxyLoginRef = await _externalReferenceProxyService.GetProxyRefAsync(new GetProxyRefRequest
            {
                RegistrationId = registrationEntity.Id,
                RegistrationUId = registrationEntity.UniqueId,
                TenantId = registrationEntity.TenantId,
                BrandLink = response.Data?.Customer?.LoginUrl
            });
            if (proxyLoginRef.Status != ResponseStatus.Ok)
            {
                _logger.LogError(proxyLoginRef.Error.ErrorMessage);
                throw new Exception(proxyLoginRef.Error.ErrorMessage);
            }

            var brandInfo = new RegistrationBrandInfo
            {
                LoginUrl = proxyLoginRef.Data,
                CustomerId = response.Data?.Customer?.CustomerId,
                Token = response.Data?.Customer?.Token
            };
            return brandInfo;
        }

        private static class UniqueIdGenerator
        {
            public static string GetNextId()
            {
                return Guid.NewGuid().ToString("N");
            }
        }
    }
}