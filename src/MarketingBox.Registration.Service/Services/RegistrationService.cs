using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.ExternalReferenceProxy.Service.Grpc;
using MarketingBox.ExternalReferenceProxy.Service.Grpc.Models;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Domain.Route;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using RegistrationAdditionalInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationAdditionalInfo;
using RegistrationBrandInfo = MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationBrandInfo;
using RegistrationCustomerInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationCustomerInfo;
using RegistrationRouteInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationRouteInfo;
using RegistrationStatus = MarketingBox.Registration.Service.Domain.Registrations.RegistrationStatus;
using RegistrationContract = MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts.Registration;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ILogger<RegistrationService> _logger;
        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataReader<CampaignIndexNoSql> _campaignIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<IntegrationNoSql> _integrationNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _affiliateNoSqlServerDataReader;
        private readonly IIntegrationService _integrationService;
        private readonly IRegistrationRepository _repository;
        private readonly IRegistrationRouterService _registrationRouter;
        private readonly IExternalReferenceProxyService _externalReferenceProxyService;

        public RegistrationService(ILogger<RegistrationService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataReader<CampaignIndexNoSql> campaignIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<IntegrationNoSql> integrationNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> brandNoSqlServerDataReader,
            IIntegrationService integrationService,
            IRegistrationRepository repository,
            IRegistrationRouterService registrationRouter,
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader,
            IExternalReferenceProxyService externalReferenceProxyService)
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
        }

        public async Task<Response<RegistrationContract>> CreateAsync(RegistrationCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Registration {@context}", request);

                var tenantId = GetTenantId(request.AuthInfo.CampaignId);
                if (tenantId == null)
                {
                    throw new BadRequestException(new Sdk.Common.Models.Error
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
                }

                if (!IsAffiliateApiKeyValid(tenantId, request.AuthInfo.AffiliateId,
                        request.AuthInfo.ApiKey, out var affiliateName))
                {
                    throw new UnauthorizedException(
                        $"Required authentication for affiliate '{request.AuthInfo.AffiliateId}'");
                }

                var registrationId = await _repository.GenerateRegistrationIdAsync(tenantId,
                    request.GeneratorId());

                var registration = GetRegistration(request, null, tenantId, registrationId, affiliateName);
                Grpc.Models.Registrations.Contracts.Registration response = null;
                var routes =
                    await _registrationRouter.GetSuitableRoutes(request.AuthInfo.CampaignId,
                        request.GeneralInfo.Country);

                if (!routes.Any())
                {
                    await _repository.SaveAsync(registration);
                    throw new Exception("Can't register on brand");
                }

                while (routes.Count > 0)
                {
                    var route = await TryGetSpecificRoute(request.AuthInfo.CampaignId,
                        request.GeneralInfo.Country, routes);

                    if (route == null)
                    {
                        await SaveAndPublishRegistration(request, registration);
                        throw new Exception("Can't register on brand");
                    }

                    routes.RemoveAll(x =>
                        (x.BrandId == route.BrandId &&
                         x.CampaignId == route.CampaignId &&
                         x.CampaignRowId == route.CampaignRowId));

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
                return new Response<RegistrationContract>
                {
                    Data = response,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return e.FailedResponse<RegistrationContract>();
            }
        }

        private async Task<Grpc.Models.Registrations.Contracts.Registration> GetRegistrationCreateResponse(
            RegistrationCreateRequest request,
            Domain.Registrations.Registration registration)
        {
            var brandResponse = await BrandRegisterAsync(registration);

            _logger.LogInformation("Brand request: {@context}. Brand response: {@response}", request, brandResponse);

            registration.Register(new RegistrationCustomerInfo()
            {
                CustomerId = brandResponse.Data.CustomerId,
                LoginUrl = brandResponse.Data.LoginUrl,
                Token = brandResponse.Data.Token,
                Brand = brandResponse.Data.Brand
            });
            return MapToGrpc(registration);
        }

        private Domain.Registrations.Registration GetRegistration(
            RegistrationCreateRequest request,
            RouteParameters routeParameters,
            string tenantId,
            long registrationId,
            string affiliateName)
        {
            var leadBrandRegistrationInfo = new RegistrationRouteInfo()
            {
                IntegrationId = routeParameters?.IntegrationId,
                BrandId = routeParameters?.BrandId,
                Integration = routeParameters?.BrandName ?? string.Empty,
                CampaignId = request.AuthInfo.CampaignId,
                AffiliateId = request.AuthInfo.AffiliateId,
                AffiliateName = affiliateName,
                CrmStatus = Domain.Crm.CrmStatus.New,
                Status = RegistrationStatus.Created,
                CustomerInfo = new RegistrationCustomerInfo()
            };
            var additionalInfo = new RegistrationAdditionalInfo()
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
                Sub10 = request.AdditionalInfo.Sub10,
            };
            var currentDate = DateTimeOffset.UtcNow;
            var generalInfo = new Domain.Registrations.RegistrationGeneralInfo()
            {
                RegistrationUid = UniqueIdGenerator.GetNextId(),
                RegistrationId = registrationId,
                FirstName = request.GeneralInfo?.FirstName,
                LastName = request.GeneralInfo?.LastName,
                Password = request.GeneralInfo?.Password,
                Email = request.GeneralInfo?.Email,
                Phone = request.GeneralInfo?.Phone,
                Ip = request.GeneralInfo?.Ip,
                Country = request.GeneralInfo?.Country,
                CreatedAt = currentDate,
                UpdatedAt = currentDate
            };
            var registration = Domain.Registrations.Registration.Restore(tenantId, generalInfo,
                leadBrandRegistrationInfo, additionalInfo);

            return registration;
        }

        private async Task SaveAndPublishRegistration(RegistrationCreateRequest request,
            Domain.Registrations.Registration registration)
        {
            await _repository.SaveAsync(registration);

            await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
            _logger.LogInformation("Sent original created registration to service bus {@context}", request);
        }

        private string GetTenantId(long campaignId)
        {
            var boxIndexNoSql = _campaignIndexNoSqlServerDataReader
                .Get(CampaignIndexNoSql.GeneratePartitionKey(campaignId)).FirstOrDefault();

            return boxIndexNoSql?.TenantId;
        }

        private bool IsAffiliateApiKeyValid(string tenantId, long affiliateId, string apiKey, out string affiliateName)
        {
            var partner =
                _affiliateNoSqlServerDataReader.Get(AffiliateNoSql.GeneratePartitionKey(tenantId),
                    AffiliateNoSql.GenerateRowKey(affiliateId));

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
            List<CampaignRowNoSql> filtered)
        {
            try
            {
                var boxIndexNoSql = _campaignIndexNoSqlServerDataReader
                    .Get(CampaignIndexNoSql.GeneratePartitionKey(campaignId)).FirstOrDefault();
                var tenantId = boxIndexNoSql?.TenantId;

                var campaignBox = await _registrationRouter.GetCampaignBox(tenantId, campaignId, country, filtered);

                if (campaignBox == null)
                    return null;

                var brandNoSql = _brandNoSqlServerDataReader.Get(
                    BrandNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    BrandNoSql.GenerateRowKey(campaignBox.BrandId));

                var brandId = brandNoSql.Id;

                var integrationNoSql = _integrationNoSqlServerDataReader.Get(
                    IntegrationNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    IntegrationNoSql.GenerateRowKey(brandNoSql.IntegrationId));

                var brandName = integrationNoSql.Name;
                var integrationId = integrationNoSql.IntegrationId;
                return new RouteParameters()
                {
                    IntegrationId = integrationId,
                    BrandName = brandName,
                    BrandId = brandId,
                    TenantId = tenantId,
                    CampaignId = campaignId,
                    CampaignRowId = campaignBox.CampaignRowId,
                };
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can't TryGetRouteInfo {@Campaign} {@Country} {@Error}",
                    campaignId, country, e.Message);
            }

            return null;
        }

        private static class UniqueIdGenerator
        {
            public static string GetNextId()
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        private async Task<RegistrationBrandInfo> BrandRegisterAsync(Domain.Registrations.Registration registration)
        {
            var request = registration.CreateIntegrationRequest();
            var response = await _integrationService.SendRegisterationAsync(request);

            var proxyLoginRef = await _externalReferenceProxyService.GetProxyRefAsync(new GetProxyRefRequest()
            {
                RegistrationId = registration.RegistrationInfo.RegistrationId,
                RegistrationUId = registration.RegistrationInfo.RegistrationUid,
                TenantId = registration.TenantId,
                BrandLink = response.Data?.Customer?.LoginUrl
            });
            if (proxyLoginRef.Status != ResponseStatus.Ok)
            {
                _logger.LogError(proxyLoginRef.Error.ErrorMessage);
                throw new Exception(proxyLoginRef.Error.ErrorMessage);
            }

            var brandInfo = new RegistrationBrandInfo()
            {
                Data = new Grpc.Models.Registrations.RegistrationCustomerInfo()
                {
                    LoginUrl = proxyLoginRef.Data,
                    CustomerId = response.Data?.Customer?.CustomerId,
                    Token = response.Data?.Customer?.Token,
                }
            };
            return brandInfo;
        }

        private static Grpc.Models.Registrations.Contracts.Registration MapToGrpc(
            Domain.Registrations.Registration registration)
        {
            return new Grpc.Models.Registrations.Contracts.Registration()
            {
                Message = registration.RouteInfo.CustomerInfo.LoginUrl,
                BrandInfo = new RegistrationBrandInfo()
                {
                    Data = new Grpc.Models.Registrations.RegistrationCustomerInfo()
                    {
                        CustomerId = registration.RouteInfo.CustomerInfo.CustomerId,
                        LoginUrl = registration.RouteInfo.CustomerInfo.LoginUrl,
                        Token = registration.RouteInfo.CustomerInfo.Token,
                        Brand = registration.RouteInfo.CustomerInfo.Brand
                    },
                },
                FallbackUrl = string.Empty,
                RegistrationId = registration.RegistrationInfo.RegistrationId,
                RegistrationUId = registration.RegistrationInfo.RegistrationUid
            };
        }
    }
}