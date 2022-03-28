using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Reporting.Service.Domain.Models;
using MarketingBox.Reporting.Service.Grpc;
using MarketingBox.Reporting.Service.Grpc.Models.RegistrationsByAffiliate;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using CrmStatus = MarketingBox.Registration.Service.Domain.Models.Common.CrmStatus;
using DepositUpdateMode = MarketingBox.Registration.Service.Domain.Models.Common.DepositUpdateMode;
using RegistrationStatus = MarketingBox.Registration.Service.Domain.Models.Common.RegistrationStatus;
using RegistrationDetails = MarketingBox.Registration.Service.Domain.Models.Registrations.RegistrationDetails;
using ReportingRegistrationDetails = MarketingBox.Reporting.Service.Domain.Models.RegistrationDetails;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationsByDateService : IRegistrationsByDateService
    {
        private readonly ILogger<RegistrationsByDateService> _logger;
        private readonly IAffiliateService _customerReportService;
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _affiliateNoSqlServerDataReader;

        private static RegistrationsReportType GetCustomersReportType(RegistrationType requestType)
        {
            return requestType switch
            {
                RegistrationType.Registrations => RegistrationsReportType.Registrations,
                RegistrationType.QFTDepositors => RegistrationsReportType.Ftd,
                RegistrationType.All => RegistrationsReportType.All,
                _ => throw new Exception()
            };
        }

        private bool CheckAuth(long affiliateId, string apiKey, out string tenantId)
        {
            var affiliates = _affiliateNoSqlServerDataReader
                .Get()
                .Select(x=>x.Affiliate);
            var affiliate = affiliates.FirstOrDefault(e => e.AffiliateId == affiliateId);
            tenantId = affiliate?.TenantId;
            return affiliate?.GeneralInfo != null && affiliate.GeneralInfo.ApiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase);
        }
        
        private static RegistrationDetails GetCustomerGrpc(ReportingRegistrationDetails details)
        {
            return new RegistrationDetails()
            {
                RegistrationUid = details.RegistrationUid,
                TenantId = details.TenantId,
                LastName = details.LastName,
                Phone = details.Phone,
                AffiliateId = details.AffiliateId,
                BrandId = details.BrandId,
                CampaignId = details.CampaignId,
                Country = details.Country,
                CreatedAt = details.CreatedAt,
                ConversionDate = details.ConversionDate,
                Email = details.Email,
                FirstName = details.FirstName,
                Ip = details.Ip,
                CrmStatus = details.CrmStatus.MapEnum<CrmStatus>(),
                AffCode = details.AffCode,
                AffiliateName = details.AffiliateName,
                ApprovedType = details.UpdateMode.MapEnum<DepositUpdateMode>(),
                CustomerBrand = details.CustomerBrand,
                CustomerId = details.CustomerId,
                CustomerLoginUrl = details.CustomerLoginUrl,
                CustomerToken = details.CustomerToken,
                Funnel = details.Funnel,
                Sub1 = details.Sub1,
                Sub2 = details.Sub2,
                Sub3 = details.Sub3,
                Sub4 = details.Sub4,
                Sub5 = details.Sub5,
                Sub6 = details.Sub6,
                Sub7 = details.Sub7,
                Sub8 = details.Sub8,
                Sub9 = details.Sub9,
                Sub10 = details.Sub10,
                Integration = details.Integration,
                IntegrationId = details.IntegrationId,
                RegistrationId = details.RegistrationId,
                Status = details.Status.MapEnum<RegistrationStatus>()
            };
        }
        
        private static List<RegistrationDetails> 
            GetCustomersGrpc(IEnumerable<ReportingRegistrationDetails> registrations)
        {
            return registrations?.Select(details => new RegistrationDetails()
            {
                RegistrationUid = details.RegistrationUid,
                TenantId = details.TenantId,
                LastName = details.LastName,
                Phone = details.Phone,
                AffiliateId = details.AffiliateId,
                BrandId = details.BrandId,
                CampaignId = details.CampaignId,
                Country = details.Country,
                CreatedAt = details.CreatedAt,
                ConversionDate = details.ConversionDate,
                Email = details.Email,
                FirstName = details.FirstName,
                Ip = details.Ip,
                CrmStatus = details.CrmStatus.MapEnum<CrmStatus>(),
                AffCode = details.AffCode,
                AffiliateName = details.AffiliateName,
                ApprovedType = details.UpdateMode.MapEnum<DepositUpdateMode>(),
                CustomerBrand = details.CustomerBrand,
                CustomerId = details.CustomerId,
                CustomerLoginUrl = details.CustomerLoginUrl,
                CustomerToken = details.CustomerToken,
                Funnel = details.Funnel,  
                Sub1 = details.Sub1,
                Sub2 = details.Sub2,
                Sub3 = details.Sub3,  
                Sub4 = details.Sub4,
                Sub5 = details.Sub5,
                Sub6 = details.Sub6,
                Sub7 = details.Sub7,
                Sub8 = details.Sub8,
                Sub9 = details.Sub9,
                Sub10 = details.Sub10,
                Integration = details.Integration,
                IntegrationId = details.IntegrationId,
                RegistrationId = details.RegistrationId,
                Status = details.Status.MapEnum<RegistrationStatus>()
            }).ToList();
        }
        
        public RegistrationsByDateService(ILogger<RegistrationsByDateService> logger,
            IAffiliateService customerReportService, 
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader)
        {
            _logger = logger;
            _customerReportService = customerReportService;
            _affiliateNoSqlServerDataReader = affiliateNoSqlServerDataReader;
        }

        public async Task<Response<IReadOnlyCollection<RegistrationDetails>>> GetRegistrationsAsync(RegistrationsGetByDateRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                _logger.LogInformation($"RegistrationsByDateService.GetRegistrationsAsync receive request : {JsonConvert.SerializeObject(request)}");

                var isAuth = CheckAuth(request.AffiliateId.Value, request.ApiKey, out var tenantId);
                if (!isAuth)
                    throw new UnauthorizedException("Affiliate", request.AffiliateId.Value);

                var reportResponse = await _customerReportService.GetRegistrations(new RegistrationsByAffiliateRequest()
                {
                    From = request.From.Value,
                    To = request.To.Value,
                    Type = GetCustomersReportType(request.Type.Value),
                    AffiliateId = request.AffiliateId.Value,
                    TenantId = tenantId,
                });

                _logger.LogInformation(@"ReportService.GetRegistrations response is: {@Request}", request);
                reportResponse.Process();
                return new Response<IReadOnlyCollection<RegistrationDetails>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = GetCustomersGrpc(reportResponse.Data)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ex.FailedResponse<IReadOnlyCollection<RegistrationDetails>>();
            }
        }

        public async Task<Response<RegistrationDetails>> GetRegistrationAsync(RegistrationGetRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                var affiliateId = request.AffiliateId.Value;
                _logger.LogInformation("RegistrationsByDateService.GetRegistrationAsync receive request: {@Request}",
                    request);
                
                var isAuth = CheckAuth(affiliateId, request.ApiKey, out var tenantId);
                if (!isAuth)
                    throw new UnauthorizedException("Affiliate", affiliateId);
                var reportResponse = await _customerReportService.GetRegistration(new RegistrationByAffiliateRequest()
                {
                    RegistrationUid = request.RegistrationUId,
                    AffiliateId = affiliateId,
                    TenantId = tenantId
                });

                _logger.LogInformation("ReportService.GetRegistration response is: {@Request}", request);

                reportResponse.Process();
                return new Response<RegistrationDetails>()
                {
                    Status = ResponseStatus.Ok,
                    Data = GetCustomerGrpc(reportResponse.Data)
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ex.FailedResponse<RegistrationDetails>();
            }
        }

    }
}