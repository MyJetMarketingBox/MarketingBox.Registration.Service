using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.ExternalReferenceProxy.Service.Grpc;
using MarketingBox.ExternalReferenceProxy.Service.Grpc.Models;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Registration.Service.Services.Interfaces;
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
        private readonly IMyNoSqlServerDataReader<CampaignIndexNoSql> _campaignIndexNoSqlServerDataReader;

        private readonly ICountryClient _countryClient;
        private readonly IExternalReferenceProxyService _externalReferenceProxyService;
        private readonly ILogger<RegistrationService> _logger;
        private readonly IRegistrationRepository _repository;
        private readonly ITrafficEngineService _trafficEngineService;
        private readonly IMapper _mapper;

        // Todo: implement proper logic for multi-tenancy and get rid of this constant.
        private const string TenantId = "default-tenant-id";

        public RegistrationService(ILogger<RegistrationService> logger,
            IMyNoSqlServerDataReader<CampaignIndexNoSql> campaignIndexNoSqlServerDataReader,
            IRegistrationRepository repository,
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader,
            IExternalReferenceProxyService externalReferenceProxyService,
            ICountryClient countryClient,
            IMapper mapper,
            ITrafficEngineService trafficEngineService)
        {
            _logger = logger;
            _campaignIndexNoSqlServerDataReader = campaignIndexNoSqlServerDataReader;
            _repository = repository;
            _affiliateNoSqlServerDataReader = affiliateNoSqlServerDataReader;
            _externalReferenceProxyService = externalReferenceProxyService;
            _countryClient = countryClient;
            _mapper = mapper;
            _trafficEngineService = trafficEngineService;
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
                    request.GeneralInfo.GeneratorId());

                var country = await GetCountry(
                    request.GeneralInfo.CountryCodeType.Value,
                    request.GeneralInfo.CountryCode);

                var registration = _mapper.Map<Domain.Models.Registrations.Registration>(request);
                registration.UniqueId = UniqueIdGenerator.GetNextId();
                registration.Country = country.Alfa2Code;
                registration.CountryId = country.Id;
                registration.AffiliateName = affiliateName;
                registration.Id = registrationId;
                registration.TenantId = TenantId;

                var success = await _trafficEngineService.TryRegisterAsync(
                    request.CampaignId.Value,
                    country.Id,
                    registration);
                if (!success)
                {
                    registration.Status = RegistrationStatus.Failed;
                    _logger.LogWarning("TrafficEngine could not register to brand. Request: {@Request}", request);
                }
                else
                {
                    var proxyLoginRef = await _externalReferenceProxyService.GetProxyRefAsync(
                        new GetProxyRefRequest
                        {
                            RegistrationId = registration.Id,
                            RegistrationUId = registration.UniqueId,
                            TenantId = registration.TenantId,
                            BrandLink = registration.CustomerLoginUrl
                        });
                    var res = proxyLoginRef.Process();
                    registration.CustomerLoginUrl = res;
                    registration.Status = RegistrationStatus.Registered;
                }

                await _repository.SaveAsync(registration);

                return new Response<Domain.Models.Registrations.Registration>
                {
                    Data = registration,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return e.FailedResponse<Domain.Models.Registrations.Registration>();
            }
        }

        public async Task<Response<Domain.Models.Registrations.Registration>> CreateS2SAsync(
            RegistrationCreateS2SRequest request)
        {
            try
            {
                request.ValidateEntity();
                _logger.LogInformation("Creating new S2S Registration {@context}", request);

                if (!IsAffiliateApiKeyValid(TenantId, request.AuthInfo.AffiliateId.Value,
                        request.AuthInfo.ApiKey, out var affiliateName))
                    throw new UnauthorizedException(
                        $"Required authentication for affiliate '{request.AuthInfo.AffiliateId}'");

                var registrationId = await _repository.GenerateRegistrationIdAsync(TenantId,
                    request.GeneralInfo.GeneratorId());

                var country = await GetCountry(
                    request.GeneralInfo.CountryCodeType.Value,
                    request.GeneralInfo.CountryCode);

                var registration = _mapper.Map<Domain.Models.Registrations.Registration>(request);
                registration.UniqueId = UniqueIdGenerator.GetNextId();
                registration.Country = country.Alfa2Code;
                registration.CountryId = country.Id;
                registration.AffiliateName = affiliateName;
                registration.Id = registrationId;
                registration.TenantId = TenantId;
                registration.Status = RegistrationStatus.Registered;

                await _repository.SaveAsync(registration);
                _logger.LogInformation("Sent original created registration to service bus {@context}", request);

                return new Response<Domain.Models.Registrations.Registration>
                {
                    Data = registration,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return e.FailedResponse<Domain.Models.Registrations.Registration>();
            }
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
                        new()
                        {
                            ErrorMessage = $"There is no country with code {countryCodeType}:{countryCode}",
                            ParameterName = nameof(countryCode)
                        }
                    }
                });

            return country;
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

        private static class UniqueIdGenerator
        {
            public static string GetNextId()
            {
                return Guid.NewGuid().ToString("N");
            }
        }
    }
}