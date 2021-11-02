using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MarketingBox.Integration.Service.Grpc;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Messages.Leads;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LeadGeneralInfo = MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo;


namespace MarketingBox.Registration.Service.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILogger<LeadService> _logger;
        private readonly IServiceBusPublisher<LeadUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSqlEntity> _myNoSqlServerDataWriter;
        private readonly IMyNoSqlServerDataReader<BoxIndexNoSql> _boxIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BoxNoSql> _boxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campaignNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignBoxNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<PartnerNoSql> _partnerNoSqlServerDataReader;
        private readonly IIntegrationService _integrationService;
        private readonly ILeadRepository _repository;
        private readonly LeadRouter _leadRouter;

        public LeadService(ILogger<LeadService> logger,
            IServiceBusPublisher<LeadUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataWriter<LeadNoSqlEntity> myNoSqlServerDataWriter,
            IMyNoSqlServerDataReader<BoxIndexNoSql> boxIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> brandNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BoxNoSql> boxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignNoSql> campaignNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignBoxNoSql> campaignBoxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<PartnerNoSql> partnerNoSqlServerDataReader,
            IIntegrationService integrationService, 
            ILeadRepository repository,
            LeadRouter leadRouter)
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
            _leadRouter = leadRouter;
        }

        public async Task<LeadCreateResponse> CreateAsync(LeadCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);

            var partnerInfo = await TryGetPartnerInfo(request);
            
            //Save Lead
            if (partnerInfo == null)
            {
                return RegisterFailedMapToGrpc(request.GeneralInfo);
            }

            try
            {
                var leadId = await _repository.GenerateLeadIdAsync(partnerInfo.TenantId, request.GeneratorId());
                var leadBrandRegistrationInfo = new Domain.Leads.LeadRouteInfo()
                {
                    BrandId = partnerInfo.BrandId,
                    CampaignId = partnerInfo.CampaignId,
                    Brand = partnerInfo.BrandName,
                    BoxId = request.AuthInfo.BoxId,
                    AffiliateId = request.AuthInfo.AffiliateId,
                    Status = Domain.Leads.LeadStatus.Created,
                    CustomerInfo = new Domain.Leads.LeadCustomerInfo()
                    {

                    }
                };
                var leadAdditionalInfo = new Domain.Leads.LeadAdditionalInfo()
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
                var leadGeneralInfo = new Domain.Leads.LeadGeneralInfo()
                {
                    UniqueId = UniqueIdGenerator.GetNextId(),
                    LeadId = leadId,
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

                var lead = Lead.Restore(partnerInfo.TenantId, 0, leadGeneralInfo, leadBrandRegistrationInfo, leadAdditionalInfo);

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent original created lead to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(lead.MapToNoSql());
                _logger.LogInformation("Sent lead update to MyNoSql {@context}", request);

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

                lead.Register(new Domain.Leads.LeadCustomerInfo()
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

                return new LeadCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private async Task<PartnerInfo> TryGetPartnerInfo(LeadCreateRequest leadCreateRequest)
        {
            string tenantId = string.Empty;
            string brandName = string.Empty;
            long campaignId = 0;
            long brandId = 0;

            try
            {
                var boxIndexNoSql = _boxIndexNoSqlServerDataReader
                    .Get(BoxIndexNoSql.GeneratePartitionKey(leadCreateRequest.AuthInfo.BoxId)).FirstOrDefault();
                tenantId = boxIndexNoSql?.TenantId;

                var campaignBox = await _leadRouter.GetCampaignBox(tenantId, leadCreateRequest.AuthInfo.BoxId, leadCreateRequest.GeneralInfo.Country);

                if (campaignBox == null)
                    return null;

                var campaignNoSql = _campaignNoSqlServerDataReader.Get(
                    CampaignNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    CampaignNoSql.GenerateRowKey(campaignBox.CampaignId));

                campaignId = campaignNoSql.Id;

                var brandNoSql = _brandNoSqlServerDataReader.Get(BrandNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    BrandNoSql.GenerateRowKey(campaignNoSql.BrandId));

                brandName = brandNoSql.Name;
                brandId = brandNoSql.BrandId;

                return new PartnerInfo()
                {
                    BrandId = brandId,
                    BrandName = brandName,
                    CampaignId = campaignId,
                    TenantId = tenantId
                };
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can't TryGetRouteInfo {@Context} {@Error}", leadCreateRequest, e.Message);
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


        public async Task<Grpc.Models.Leads.LeadBrandInfo> BrandRegisterAsync(Lead lead)
        {
            var request = lead.CreateIntegrationRequest();
            var response = await _integrationService.RegisterLeadAsync(request);

            var brandInfo = new Grpc.Models.Leads.LeadBrandInfo()
            {
                Status = (ResultCode)response.Status,
                Data = new Grpc.Models.Leads.LeadCustomerInfo()
                {
                    LoginUrl = response.RegisteredLeadInfo?.LoginUrl,
                    CustomerId = response.RegisteredLeadInfo?.CustomerId,
                    Token = response.RegisteredLeadInfo?.Token,
                }
            };

            return brandInfo;
        }

        private static LeadCreateResponse SuccessfullMapToGrpc(Lead lead)
        {
            return new LeadCreateResponse()
            {
                Status = ResultCode.CompletedSuccessfully,
                Message = lead.RouteInfo.CustomerInfo.LoginUrl,
                BrandInfo = new MarketingBox.Registration.Service.Grpc.Models.Leads.LeadBrandInfo()
                {
                    Status = ResultCode.CompletedSuccessfully,
                    Data = new MarketingBox.Registration.Service.Grpc.Models.Leads.LeadCustomerInfo()
                    {
                        CustomerId = lead.RouteInfo.CustomerInfo.CustomerId,
                        LoginUrl = lead.RouteInfo.CustomerInfo.LoginUrl,
                        Token = lead.RouteInfo.CustomerInfo.Token,
                        Brand = lead.RouteInfo.CustomerInfo.Brand
                    },
                },
                FallbackUrl = string.Empty,
                LeadId = lead.LeadInfo.LeadId,
            };
        }

        private static LeadCreateResponse RegisterFailedMapToGrpc(
            MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo reneralInfo)
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


        private static LeadCreateResponse FailedMapToGrpc(Error error,
            MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo original)
        {
            return new LeadCreateResponse()
            {
                Status = ResultCode.Failed,
                Error = error,
                Message = error.Message,
                OriginalData = new LeadGeneralInfo()
                {
                    Email = original.Email,
                    Password = original.Password,
                    FirstName = original.FirstName,
                    Ip = original.Ip,
                    LastName = original.LastName,
                    Phone = original.Phone
                }
            };
        }
    }
}
