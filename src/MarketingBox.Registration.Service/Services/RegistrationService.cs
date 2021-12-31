using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Registration.Service.Messages.Registrations;
using RegistrationAdditionalInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationAdditionalInfo;
using RegistrationBrandInfo = MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationBrandInfo;
using RegistrationCustomerInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationCustomerInfo;
using RegistrationGeneralInfo = MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationGeneralInfo;
using RegistrationRouteInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationRouteInfo;
using RegistrationStatus = MarketingBox.Registration.Service.Domain.Registrations.RegistrationStatus;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using System.Collections.Generic;
using MarketingBox.Registration.Service.Domain.Route;

namespace MarketingBox.Registration.Service.Modules
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
        private readonly RegistrationRouterService _registrationRouter;

        public RegistrationService(ILogger<RegistrationService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataReader<CampaignIndexNoSql> campaignIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<IntegrationNoSql> integrationNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> brandNoSqlServerDataReader,
            IIntegrationService integrationService, 
            IRegistrationRepository repository,
            RegistrationRouterService registrationRouter, 
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader)
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
        }

        public async Task<RegistrationCreateResponse> CreateAsync(RegistrationCreateRequest request)
        {
            _logger.LogInformation("Creating new Registration {@context}", request);

            var tenantId = GetTenantId(request.AuthInfo.CampaignId);
            if (!IsAffiliateApiKeyValid(tenantId, request.AuthInfo.AffiliateId, 
                request.AuthInfo.ApiKey, out var affiliateName))
            {
                return await Task.FromResult<RegistrationCreateResponse>(
                    new RegistrationCreateResponse()
                {
                    Status = ResultCode.RequiredAuthentication,
                    Error = new Error()
                    {
                        Message = $"Require '{request.AuthInfo.AffiliateId}' authentication",
                        Type = ErrorType.InvalidAffiliateInfo
                    }
                });
            }

            try
            {
                var registrationId = await _repository.GenerateRegistrationIdAsync(tenantId,
                    request.GeneratorId());

                Domain.Registrations.Registration registration = GetRegistration(request, null, tenantId, registrationId, affiliateName);
                RegistrationCreateResponse response = null;
                var routes = await _registrationRouter.GetSuitableRoutes(request.AuthInfo.CampaignId, request.GeneralInfo.Country);

                while(routes.Count > 0)
                {
                    var route = await TryGetSpecificRoute(request.AuthInfo.CampaignId,
                        request.GeneralInfo.Country, routes);

                    if (route == null)
                    {
                        await SaveAndPublishRegistration(request, registration);
                        return RegisterFailedMapToGrpc(request.GeneralInfo);
                    }
                    
                    routes.RemoveAll(x => 
                        (x.BrandId == route.BrandId && 
                        x.CampaignId == route.CampaignId && 
                        x.CampaignRowId == route.CampaignRowId));

                    registration.RouteInfo.BrandId = route.BrandId;
                    registration.RouteInfo.CampaignId = route.CampaignId;
                    registration.RouteInfo.Integration = route.BrandName;
                    registration.RouteInfo.IntegrationId = route.IntegrationId;

                    response = await GetRegistrationCreateResponse(request, registration);
                    if (response.Status == ResultCode.CompletedSuccessfully)
                    {
                        break;
                    }

                    if (response?.Error?.Type == ErrorType.InvalidPersonalData)
                    {
                        continue;
                    }
                }
                await SaveAndPublishRegistration(request, registration);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return new RegistrationCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }
        private async Task<RegistrationCreateResponse> GetRegistrationCreateResponse(
            RegistrationCreateRequest request,
            Domain.Registrations.Registration registration)
        {

            var brandResponse = await BrandRegisterAsync(registration);

            _logger.LogInformation("Brand request: {@context}. Brand response: {@response}", request, brandResponse);
            
            switch (brandResponse.Status)
            {
                case ResultCode.CompletedSuccessfully:
                    registration.Register(new RegistrationCustomerInfo()
                    {
                        CustomerId = brandResponse.Data.CustomerId,
                        LoginUrl = brandResponse.Data.LoginUrl,
                        Token = brandResponse.Data.Token,
                        Brand = brandResponse.Data.Brand
                    });
                    return SuccessfullMapToGrpc(registration);
                case ResultCode.Failed:
                    return FailedMapToGrpc(new Error()
                    {
                        Message = "Can't register on brand",
                        Type = ErrorType.InvalidPersonalData
                    }, request.GeneralInfo);
                case ResultCode.RequiredAuthentication:
                    return FailedMapToGrpc(new Error()
                    {
                        Message = ResultCode.RequiredAuthentication.DescriptionAttr(),
                        Type = ErrorType.Unauthorized
                    }, request.GeneralInfo);
                default:
                    return FailedMapToGrpc(new Error()
                    {
                        Message = "Can't register on brand",
                        Type = ErrorType.Unknown
                    }, request.GeneralInfo);
            }
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
                IntegrationId = routeParameters?.IntegrationId ?? 0,
                BrandId = routeParameters?.BrandId ?? 0,
                Integration = routeParameters?.BrandName ?? string.Empty,
                CampaignId = request.AuthInfo.CampaignId,
                AffiliateId = request.AuthInfo.AffiliateId,
                AffiliateName = affiliateName,
                CrmStatus = Domain.Crm.CrmStatus.Unknown,
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
            var registration = Domain.Registrations.Registration.Restore(tenantId, 0, generalInfo,
                leadBrandRegistrationInfo, additionalInfo);

            return registration;
        }

        private async Task SaveAndPublishRegistration(RegistrationCreateRequest request, Domain.Registrations.Registration registration)
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

                var integrationNoSql = _integrationNoSqlServerDataReader.Get(IntegrationNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
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

            var brandInfo = new RegistrationBrandInfo()
            {
                Status = (ResultCode)response.Status,
                Data = new Grpc.Models.Registrations.RegistrationCustomerInfo()
                {
                    LoginUrl = response.Customer?.LoginUrl,
                    CustomerId = response.Customer?.CustomerId,
                    Token = response.Customer?.Token,
                }
            };

            return brandInfo;
        }

        private static RegistrationCreateResponse SuccessfullMapToGrpc(Domain.Registrations.Registration registration)
        {
            return new RegistrationCreateResponse()
            {
                Status = ResultCode.CompletedSuccessfully,
                Message = registration.RouteInfo.CustomerInfo.LoginUrl,
                BrandInfo = new RegistrationBrandInfo()
                {
                    Status = ResultCode.CompletedSuccessfully,
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

        private static RegistrationCreateResponse RegisterFailedMapToGrpc(
            RegistrationGeneralInfo reneralInfo)
        {
            return FailedMapToGrpc(
                new Error()
                {
                    Message = "Can't get partner info",
                    Type = ErrorType.Unknown
                },
                reneralInfo
            );
        }


        private static RegistrationCreateResponse FailedMapToGrpc(Error error,
            RegistrationGeneralInfo original)
        {
            return new RegistrationCreateResponse()
            {
                Status = ResultCode.Failed,
                Error = error,
                Message = error.Message,
                OriginalData = new RegistrationGeneralInfo()
                {
                    Email = original.Email,
                    Password = original.Password,
                    FirstName = original.FirstName,
                    Ip = original.Ip,
                    LastName = original.LastName,
                    Phone = original.Phone,
                    Country = original.Country
                }
            };
        }
    }
}
