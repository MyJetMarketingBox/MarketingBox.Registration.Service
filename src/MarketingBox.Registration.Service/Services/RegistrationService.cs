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
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Registration.Service.MyNoSql.Registrations;
using RegistrationAdditionalInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationAdditionalInfo;
using RegistrationBrandInfo = MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationBrandInfo;
using RegistrationCustomerInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationCustomerInfo;
using RegistrationGeneralInfo = MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationGeneralInfo;
using RegistrationRouteInfo = MarketingBox.Registration.Service.Domain.Registrations.RegistrationRouteInfo;
using RegistrationStatus = MarketingBox.Registration.Service.Domain.Registrations.RegistrationStatus;


namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ILogger<RegistrationService> _logger;
        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<RegistrationNoSqlEntity> _myNoSqlServerDataWriter;
        private readonly IMyNoSqlServerDataReader<CampaignIndexNoSql> _boxIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<IntegrationNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _boxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _campaignNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignRowNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _partnerNoSqlServerDataReader;
        private readonly IIntegrationService _integrationService;
        private readonly IRegistrationRepository _repository;
        private readonly RegistrationRouter _registrationRouter;

        public RegistrationService(ILogger<RegistrationService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataWriter<RegistrationNoSqlEntity> myNoSqlServerDataWriter,
            IMyNoSqlServerDataReader<CampaignIndexNoSql> boxIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<IntegrationNoSql> brandNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignNoSql> boxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> campaignNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignRowNoSql> campaignBoxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<AffiliateNoSql> partnerNoSqlServerDataReader,
            IIntegrationService integrationService, 
            IRegistrationRepository repository,
            RegistrationRouter registrationRouter)
        {
            _logger = logger;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherLeadUpdated = publisherLeadUpdated;
            _boxIndexNoSqlServerDataReader = boxIndexNoSqlServerDataReader;
            _brandNoSqlServerDataReader = brandNoSqlServerDataReader;
            _boxNoSqlServerDataReader = boxNoSqlServerDataReader;
            _campaignNoSqlServerDataReader = campaignNoSqlServerDataReader;
            _campaignBoxNoSqlServerDataReader = campaignBoxNoSqlServerDataReader;
            _partnerNoSqlServerDataReader = partnerNoSqlServerDataReader;
            _integrationService = integrationService;
            _repository = repository;
            _registrationRouter = registrationRouter;
        }

        public async Task<RegistrationCreateResponse> CreateAsync(RegistrationCreateRequest request)
        {
            _logger.LogInformation("Creating new Registration {@context}", request);

            var partnerInfo = await TryGetPartnerInfo(request);
            
            //Save Registration
            if (partnerInfo == null)
            {
                return RegisterFailedMapToGrpc(request.GeneralInfo);
            }

            try
            {
                var registrationId = await _repository.GenerateRegistrationIdAsync(partnerInfo.TenantId, request.GeneratorId());
                var leadBrandRegistrationInfo = new RegistrationRouteInfo()
                {
                    IntegrationId = partnerInfo.IntegrationId,
                    BrandId = partnerInfo.BrandId,
                    Integration = partnerInfo.BrandName,
                    CampaignId = request.AuthInfo.CampaignId,
                    AffiliateId = request.AuthInfo.AffiliateId,
                    Status = RegistrationStatus.Created,
                    CustomerInfo = new RegistrationCustomerInfo()
                    {

                    }
                };
                var leadAdditionalInfo = new RegistrationAdditionalInfo()
                {
                    So = request.AdditionalInfo.So,
                    Sub = request.AdditionalInfo.Sub,
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
                var leadGeneralInfo = new Domain.Registrations.RegistrationGeneralInfo()
                {
                    UniqueId = UniqueIdGenerator.GetNextId(),
                    RegistrationId = registrationId,
                    FirstName = request.GeneralInfo?.FirstName,
                    LastName = request.GeneralInfo?.LastName,
                    Password = request.GeneralInfo?.Password,
                    Email = request.GeneralInfo?.Email,
                    Phone = request.GeneralInfo?.Phone,
                    Ip = request.GeneralInfo?.Ip,
                    Country = request.GeneralInfo?.Country,
                    CreatedAt = currentDate,
                    UpdatedAt = currentDate,
                };

                var lead = Domain.Registrations.Registration.Restore(partnerInfo.TenantId, 0, leadGeneralInfo, leadBrandRegistrationInfo, leadAdditionalInfo);

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent original created registration to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(lead.MapToNoSql());
                _logger.LogInformation("Sent registration update to MyNoSql {@context}", request);

                var brandResponse = await BrandRegisterAsync(lead);

                if (brandResponse.Status != ResultCode.CompletedSuccessfully)
                {
                    _logger.LogInformation("Failed to register on brand {@context} {@response}", request, brandResponse);
                    return FailedMapToGrpc(new Error()
                        {
                            Message = "Can't register on brand",
                            Type = ErrorType.InvalidPersonalData
                        },
                        request.GeneralInfo);
                }

                lead.Register(new RegistrationCustomerInfo()
                {
                    CustomerId = brandResponse.Data.CustomerId,
                    LoginUrl = brandResponse.Data.LoginUrl,
                    Token = brandResponse.Data.Token,
                    Brand = brandResponse.Data.Brand
                });

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent original created to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(lead.MapToNoSql());
                _logger.LogInformation("Sent original update to MyNoSql {@context}", request);


                return brandResponse.Status == ResultCode.CompletedSuccessfully ?
                    SuccessfullMapToGrpc(lead) : FailedMapToGrpc(new Error()
                    {
                        Message = "Can't register on brand",
                        Type = ErrorType.InvalidPersonalData
                    },
                    request.GeneralInfo);
                ;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return new RegistrationCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private async Task<PartnerInfo> TryGetPartnerInfo(RegistrationCreateRequest registrationCreateRequest)
        {
            string tenantId = string.Empty;
            string brandName = string.Empty;
            long campaignId = 0;
            long integrationId = 0;

            try
            {
                var boxIndexNoSql = _boxIndexNoSqlServerDataReader
                    .Get(CampaignIndexNoSql.GeneratePartitionKey(registrationCreateRequest.AuthInfo.CampaignId)).FirstOrDefault();
                tenantId = boxIndexNoSql?.TenantId;

                var campaignBox = await _registrationRouter.GetCampaignBox(tenantId, registrationCreateRequest.AuthInfo.CampaignId, registrationCreateRequest.GeneralInfo.Country);

                if (campaignBox == null)
                    return null;

                var campaignNoSql = _campaignNoSqlServerDataReader.Get(
                    BrandNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    BrandNoSql.GenerateRowKey(campaignBox.CampaignId));

                campaignId = campaignNoSql.Id;

                var brandNoSql = _brandNoSqlServerDataReader.Get(IntegrationNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    IntegrationNoSql.GenerateRowKey(campaignNoSql.IntegrationId));

                brandName = brandNoSql.Name;
                integrationId = brandNoSql.IntegrationId;

                return new PartnerInfo()
                {
                    IntegrationId = integrationId,
                    BrandName = brandName,
                    BrandId = campaignId,
                    TenantId = tenantId
                };
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can't TryGetRouteInfo {@Context} {@Error}", registrationCreateRequest, e.Message);
            }

            return null;
        }

        private bool IsPartnerRequestInvalid(string requestApiKey, string apiKey)
        {
            return !apiKey.Equals(requestApiKey, StringComparison.OrdinalIgnoreCase);
        }

        public static class UniqueIdGenerator
        {
            public static string GetNextId()
            {
                return Guid.NewGuid().ToString();
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public async Task<RegistrationBrandInfo> BrandRegisterAsync(Domain.Registrations.Registration registration)
        {
            var request = registration.CreateIntegrationRequest();
            var response = await _integrationService.RegisterLeadAsync(request);

            var brandInfo = new RegistrationBrandInfo()
            {
                Status = (ResultCode)response.Status,
                Data = new Grpc.Models.Registrations.RegistrationCustomerInfo()
                {
                    LoginUrl = response.RegisteredLeadInfo?.LoginUrl,
                    CustomerId = response.RegisteredLeadInfo?.CustomerId,
                    Token = response.RegisteredLeadInfo?.Token,
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
