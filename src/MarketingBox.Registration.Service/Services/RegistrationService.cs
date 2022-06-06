using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.ExternalReferenceProxy.Service.Grpc;
using MarketingBox.ExternalReferenceProxy.Service.Grpc.Models;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Registration.Service.Services.Interfaces;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ICountryClient _countryClient;
        private readonly IAffiliateClient _affiliateClient;
        private readonly ICampaignClient _campaignClient;
        private readonly IOfferClient _offerClient;
        private readonly IExternalReferenceProxyService _externalReferenceProxyService;
        private readonly ILogger<RegistrationService> _logger;
        private readonly IRegistrationRepository _repository;
        private readonly ITrafficEngineService _trafficEngineService;
        private readonly IMapper _mapper;

        public RegistrationService(ILogger<RegistrationService> logger,
            IRegistrationRepository repository,
            IExternalReferenceProxyService externalReferenceProxyService,
            ICountryClient countryClient,
            IMapper mapper,
            ITrafficEngineService trafficEngineService,
            IAffiliateClient affiliateClient, IOfferClient offerClient, ICampaignClient campaignClient)
        {
            _logger = logger;
            _repository = repository;
            _externalReferenceProxyService = externalReferenceProxyService;
            _countryClient = countryClient;
            _mapper = mapper;
            _trafficEngineService = trafficEngineService;
            _affiliateClient = affiliateClient;
            _offerClient = offerClient;
            _campaignClient = campaignClient;
        }

        public async Task<Response<Domain.Models.Registrations.Registration>> CreateAsync(
            RegistrationCreateRequest request)
        {
            try
            {
                request.ValidateEntity();
                _logger.LogInformation("Creating new Registration {@context}", request);

                var affiliate = await GetAffiliate(
                    request.AuthInfo.AffiliateId.Value,
                    request.AuthInfo.ApiKey);

                var tenantId = affiliate.TenantId;
                var affiliateName = affiliate.GeneralInfo.Username;
                var registrationId = await _repository.GenerateRegistrationIdAsync(
                    tenantId,
                    request.GeneralInfo.GeneratorId());

                var country = await GetCountry(
                    request.GeneralInfo.CountryCodeType.Value,
                    request.GeneralInfo.CountryCode);
                var campaign = await GetCampaign(request.CampaignId.Value, tenantId);

                var registration = _mapper.Map<Domain.Models.Registrations.Registration>(request);
                registration.UniqueId = UniqueIdGenerator.GetNextId();
                registration.Country = country.Alfa2Code;
                registration.CountryId = country.Id;
                registration.AffiliateName = affiliateName;
                registration.Id = registrationId;
                registration.TenantId = tenantId;
                registration.CampaignName = campaign.Name;

                try
                {
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
                }
                catch
                {
                    _logger.LogWarning("TrafficEngine failed register. Request: {@Request}", request);
                    registration.Status = RegistrationStatus.Failed;
                    throw;
                }
                finally
                {
                    await _repository.SaveAsync(registration);
                }

                return new Response<Domain.Models.Registrations.Registration>
                {
                    Data = registration,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during registration {@context}", request);

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

                var affiliate = await GetAffiliate(
                    request.AuthInfo.AffiliateId.Value,
                    request.AuthInfo.ApiKey);

                var tenantId = affiliate.TenantId;
                var affiliateName = affiliate.GeneralInfo.Username;
                var offer = await GetOffer(request.OfferId.Value, tenantId);
                var registrationId = await _repository.GenerateRegistrationIdAsync(tenantId,
                    request.GeneralInfo.GeneratorId());
                
                var country = await GetCountry(
                    request.GeneralInfo.CountryCodeType.Value,
                    request.GeneralInfo.CountryCode);

                var registration = _mapper.Map<Domain.Models.Registrations.Registration>(request);
                registration.UniqueId = UniqueIdGenerator.GetNextId();
                registration.Country = country.Alfa2Code;
                registration.CountryId = country.Id;
                registration.AffiliateName = affiliateName;
                registration.OfferName = offer.Name;
                registration.Id = registrationId;
                registration.TenantId = tenantId;
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

        private async Task<AffiliateMessage> GetAffiliate(long affiliateId, string apiKey)
        {
            AffiliateMessage affiliate = null;
            try
            {
                affiliate = await _affiliateClient.GetAffiliateByApiKey(
                    apiKey, true);
            }
            catch (NotFoundException)
            {
                throw new UnauthorizedException(
                    $"Required authentication for affiliate '{affiliateId}'");
            }

            return affiliate;
        }

        private async Task<Offer> GetOffer(long offerId, string tenantId)
        {
            var offer = await _offerClient.GetOfferByTenantAndId(offerId, tenantId, checkInService: true);
            return offer;
        }

        private async Task<CampaignMessage> GetCampaign(long campaignId, string tenantId)
        {
            var campaign = await _campaignClient.GetCampaignById(campaignId, tenantId, checkInService: true);
            return campaign;
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