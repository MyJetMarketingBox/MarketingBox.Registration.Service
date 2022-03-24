using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using MarketingBox.Registration.Service.Domain.Models;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Domain.Route;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using RegistrationRouteInfo = MarketingBox.Registration.Service.Domain.Models.Registrations.RegistrationRouteInfo;

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
            ICountryClient countryClient)
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
        }

        public async Task<Response<Grpc.Registration>> CreateAsync(RegistrationCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Registration {@context}", request);

                var tenantId = GetTenantId(request.AuthInfo.CampaignId);
                if (tenantId == null)
                    throw new BadRequestException(new Error
                    {
                        ErrorMessage = BadRequestException.DefaultErrorMessage,
                        ValidationErrors = new List<ValidationError>
                        {
                            new ValidationError
                            {
                                ParameterName = nameof(request.AuthInfo.CampaignId),
                                ErrorMessage = $"Incorrect offerid '{request.AuthInfo.CampaignId}'"
                            }
                        }
                    });

                if (!IsAffiliateApiKeyValid(tenantId, request.AuthInfo.AffiliateId,
                        request.AuthInfo.ApiKey, out var affiliateName))
                    throw new UnauthorizedException(
                        $"Required authentication for affiliate '{request.AuthInfo.AffiliateId}'");

                var registrationId = await _repository.GenerateRegistrationIdAsync(tenantId,
                    request.GeneratorId());

                var country =
                    await GetCountry(request.GeneralInfo.CountryCodeType, request.GeneralInfo.CountryCode);

                var registration = GetRegistration(request, null, tenantId, registrationId, affiliateName, country);


                Grpc.Registration response = null;
                var routes =
                    await _registrationRouter.GetSuitableRoutes(request.AuthInfo.CampaignId, country.Id);

                if (!routes.Any())
                {
                    await _repository.SaveAsync(registration);
                    throw new Exception("Can't register on brand");
                }

                while (routes.Count > 0)
                {
                    var route = await TryGetSpecificRoute(request.AuthInfo.CampaignId, request.GeneralInfo.CountryCode,
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

                    registration.RouteInfo.BrandId = route.BrandId;
                    registration.RouteInfo.CampaignId = route.CampaignId;
                    registration.RouteInfo.Integration = route.BrandName;
                    registration.RouteInfo.IntegrationId = route.IntegrationId;
                    try
                    {
                        response = await GetRegistrationCreateResponse(request, registration);
                        break;
                    }
                    catch
                    {
                        // ignored
                    }
                }

                await SaveAndPublishRegistration(request, registration);
                return new Response<Grpc.Registration>
                {
                    Data = response,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return e.FailedResponse<Grpc.Registration>();
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
                        new ValidationError
                        {
                            ErrorMessage = $"There is no country with code {countryCodeType}:{countryCode}",
                            ParameterName = nameof(countryCode)
                        }
                    }
                });

            return country;
        }

        private async Task<Grpc.Registration> GetRegistrationCreateResponse(
            RegistrationCreateRequest request,
            Registration_nogrpc registrationNogrpc)
        {
            var brandResponse = await BrandRegisterAsync(registrationNogrpc);

            _logger.LogInformation("Brand request: {@context}. Brand response: {@response}", request, brandResponse);

            registrationNogrpc.Register(new RegistrationBrandInfo
            {
                CustomerId = brandResponse.CustomerId,
                LoginUrl = brandResponse.LoginUrl,
                Token = brandResponse.Token,
                Brand = brandResponse.Brand
            });
            return MapToGrpc(registrationNogrpc);
        }

        private Registration_nogrpc GetRegistration(
            RegistrationCreateRequest request,
            RouteParameters routeParameters,
            string tenantId,
            long registrationId,
            string affiliateName,
            Country country)
        {
            var leadBrandRegistrationInfo = new RegistrationRouteInfo
            {
                IntegrationId = routeParameters?.IntegrationId,
                BrandId = routeParameters?.BrandId,
                Integration = routeParameters?.BrandName ?? string.Empty,
                CampaignId = request.AuthInfo.CampaignId,
                AffiliateId = request.AuthInfo.AffiliateId,
                AffiliateName = affiliateName,
                CrmStatus = CrmStatus.New,
                Status = RegistrationStatus.Created,
                BrandInfo = new RegistrationBrandInfo()
            };
            var additionalInfo = new RegistrationAdditionalInfo
            {
                Funnel = request.AdditionalInfo.Funnel,
                AffCode = request.AdditionalInfo.AffCode,
                Sub1 = request.AdditionalInfo.Sub1,
                Sub2 = request.AdditionalInfo.Sub2,
                Sub3 = request.AdditionalInfo.Sub3,
                Sub4 = request.AdditionalInfo.Sub4,
                Sub5 = request.AdditionalInfo.Sub5,
                Sub6 = request.AdditionalInfo.Sub6,
                Sub7 = request.AdditionalInfo.Sub7,
                Sub8 = request.AdditionalInfo.Sub8,
                Sub9 = request.AdditionalInfo.Sub9,
                Sub10 = request.AdditionalInfo.Sub10
            };
            var currentDate = DateTimeOffset.UtcNow;
            var generalInfo = new RegistrationGeneralInfo_notgrpc
            {
                RegistrationUid = UniqueIdGenerator.GetNextId(),
                RegistrationId = registrationId,
                FirstName = request.GeneralInfo?.FirstName,
                LastName = request.GeneralInfo?.LastName,
                Password = request.GeneralInfo?.Password,
                Email = request.GeneralInfo?.Email,
                Phone = request.GeneralInfo?.Phone,
                Ip = request.GeneralInfo?.Ip,
                CountryId = country.Id,
                CountryAlfa2Code = country.Alfa2Code,
                CreatedAt = currentDate,
                UpdatedAt = currentDate
            };
            var registration = Registration_nogrpc.Restore(tenantId, generalInfo,
                leadBrandRegistrationInfo, additionalInfo);

            return registration;
        }

        private async Task SaveAndPublishRegistration(RegistrationCreateRequest request,
            Registration_nogrpc registrationNogrpc)
        {
            await _repository.SaveAsync(registrationNogrpc);

            await _publisherLeadUpdated.PublishAsync(registrationNogrpc.MapToMessage());
            _logger.LogInformation("Sent original created registration to service bus {@context}", request);
        }

        private string GetTenantId(long campaignId)
        {
            var boxIndexNoSql = _campaignIndexNoSqlServerDataReader
                .Get(CampaignIndexNoSql.GeneratePartitionKey(campaignId))
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
            Registration_nogrpc registrationNogrpc)
        {
            var request = registrationNogrpc.CreateIntegrationRequest();
            var response = await _integrationService.SendRegisterationAsync(request);

            var proxyLoginRef = await _externalReferenceProxyService.GetProxyRefAsync(new GetProxyRefRequest
            {
                RegistrationId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationId,
                RegistrationUId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationUid,
                TenantId = registrationNogrpc.TenantId,
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

        private static Grpc.Registration MapToGrpc(
            Registration_nogrpc registrationNogrpc)
        {
            return new Grpc.Registration
            {
                Message = registrationNogrpc.RouteInfo.BrandInfo.LoginUrl,
                BrandInfo = new RegistrationBrandInfo
                {
                    CustomerId = registrationNogrpc.RouteInfo.BrandInfo.CustomerId,
                    LoginUrl = registrationNogrpc.RouteInfo.BrandInfo.LoginUrl,
                    Token = registrationNogrpc.RouteInfo.BrandInfo.Token,
                    Brand = registrationNogrpc.RouteInfo.BrandInfo.Brand
                },
                FallbackUrl = string.Empty,
                RegistrationId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationId,
                RegistrationUId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationUid
            };
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