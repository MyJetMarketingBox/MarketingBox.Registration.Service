using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Registration.Service.Domain.Crm;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Reporting.Service.Grpc;
using MarketingBox.Reporting.Service.Grpc.Models.RegistrationsByAffiliate;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;

namespace MarketingBox.Registration.Service.Modules
{
    public class RegistrationsByDateService : IRegistrationsByDateService
    {
        private readonly ILogger<RegistrationsByDateService> _logger;
        private readonly IAffiliateService _customerReportService;
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _affiliateNoSqlServerDataReader;

        public RegistrationsByDateService(ILogger<RegistrationsByDateService> logger,
            IAffiliateService customerReportService, 
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader)
        {
            _logger = logger;
            _customerReportService = customerReportService;
            _affiliateNoSqlServerDataReader = affiliateNoSqlServerDataReader;
        }

        public async Task<RegistrationsGetByDateResponse> GetRegistrationsAsync(RegistrationsGetByDateRequest request)
        {
            try
            {
                _logger.LogInformation($"RegistrationsByDateService.GetRegistrationsAsync receive request : {JsonConvert.SerializeObject(request)}");

                var isAuth = CheckAuth(request.AffiliateId, request.ApiKey, out var tenantId);
                if (!isAuth)
                    return new RegistrationsGetByDateResponse()
                    {
                        Error = new Error()
                        {
                            Type = ErrorType.Unauthorized
                        }
                    };

                if (request.From == DateTime.MinValue)
                    return new RegistrationsGetByDateResponse()
                    {
                        Error = new Error() {Type = ErrorType.InvalidParameter, Message = "Date From cannot be empty"}
                    };
                if (request.To == DateTime.MinValue)
                    return new RegistrationsGetByDateResponse()
                    {
                        Error = new Error() {Type = ErrorType.InvalidParameter, Message = "Date To cannot be empty"}
                    };

                var reportResponse = await _customerReportService.GetRegistrations(new RegistrationsByAffiliateRequest()
                {
                    From = request.From,
                    To = request.To,
                    Type = GetCustomersReportType(request.Type),
                    AffiliateId = request.AffiliateId,
                    TenantId = tenantId,
                });

                _logger.LogInformation($"ReportService.GetRegistrations response is : {JsonConvert.SerializeObject(reportResponse)}");

                if (reportResponse.Error != null)
                {

                    return new RegistrationsGetByDateResponse()
                    {
                        Error = new Error()
                        {
                            Type = reportResponse.Error.Type.MapEnum<ErrorType>(),
                            Message = reportResponse.Error.Message
                        }
                    };
                }
                return new RegistrationsGetByDateResponse()
                {
                    Customers = GetCustomersGrpc(reportResponse.Registrations)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new RegistrationsGetByDateResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.Unknown,
                        Message = ex.Message
                    }
                };
            }
        }

        private static Reporting.Service.Grpc.Models.RegistrationsByAffiliate.RegistrationsReportType GetCustomersReportType(RegistrationType requestType)
        {
            return requestType switch
            {
                RegistrationType.Registrations => Reporting.Service.Grpc.Models.RegistrationsByAffiliate.RegistrationsReportType.Registrations,
                RegistrationType.QFTDepositors => Reporting.Service.Grpc.Models.RegistrationsByAffiliate.RegistrationsReportType.QFTDepositors,
                RegistrationType.All => Reporting.Service.Grpc.Models.RegistrationsByAffiliate.RegistrationsReportType.All,
                _ => throw new Exception()
            };
        }

        private bool CheckAuth(long affiliateId, string apiKey, out string tenantId)
        {
            var affiliates = _affiliateNoSqlServerDataReader.Get();
            var affiliate = affiliates.FirstOrDefault(e => e.AffiliateId == affiliateId);
            tenantId = affiliate?.TenantId;
            return affiliate?.GeneralInfo != null && affiliate.GeneralInfo.ApiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<RegistrationGetResponse> GetRegistrationAsync(RegistrationGetRequest request)
        {
            try
            {
                _logger.LogInformation($"RegistrationsByDateService.GetRegistrationAsync receive request : {JsonConvert.SerializeObject(request)}");
                
                var isAuth = CheckAuth(request.AffiliateId, request.ApiKey, out var tenantId);
                if (!isAuth)
                    return new RegistrationGetResponse()
                    {
                        Error = new Error()
                        {
                            Type = ErrorType.Unauthorized
                        }
                    };
                var reportResponse = await _customerReportService.GetRegistration(new RegistrationByAffiliateRequest()
                {
                    RegistrationUid = request.RegistrationUId,
                    AffiliateId = request.AffiliateId,
                    TenantId = tenantId
                });

                _logger.LogInformation($"ReportService.GetRegistration response is : {JsonConvert.SerializeObject(reportResponse)}");

                if (reportResponse.Error != null)
                {
                    return new RegistrationGetResponse()
                    {
                        Error = new Error()
                        {
                            Type = reportResponse.Error.Type.MapEnum<ErrorType>(),
                            Message = reportResponse.Error.Message
                        }
                    };
                }
                return new RegistrationGetResponse()
                {
                    Customer = GetCustomerGrpc(reportResponse.Registration)
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new RegistrationGetResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.Unknown,
                        Message = ex.Message
                    }
                };
            }
        }

        private static MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationDetails 
            GetCustomerGrpc(Reporting.Service.Domain.Models.RegistrationDetails registration)
        {
            return new MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationDetails()
            {
                RegistrationUid = registration.RegistrationUid,
                TenantId = registration.TenantId,
                LastName = registration.LastName,
                Phone = registration.Phone,
                AffiliateId = registration.AffiliateId,
                BrandId = registration.BrandId,
                CampaignId = registration.CampaignId,
                Country = registration.Country,
                CreatedAt = registration.CreatedAt,
                ConversionDate = registration.ConversionDate,
                Email = registration.Email,
                FirstName = registration.FirstName,
                Ip = registration.Ip,
                CrmStatus = registration.CrmStatus.MapEnum<CrmStatus>(),
                
            };
        }
        
        private static List<MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationDetails> 
            GetCustomersGrpc(IEnumerable<Reporting.Service.Domain.Models.RegistrationDetails> registrations)
        {
            return registrations.Select(e => new MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationDetails()
            {
                RegistrationUid = e.RegistrationUid,
                TenantId = e.TenantId,
                LastName = e.LastName,
                Phone = e.Phone,
                AffiliateId = e.AffiliateId,
                BrandId = e.BrandId,
                CampaignId = e.CampaignId,
                Country = e.Country,
                CreatedAt = e.CreatedAt,
                ConversionDate = e.ConversionDate,
                Email = e.Email,
                FirstName = e.FirstName,
                Ip = e.Ip,
                CrmStatus = e.CrmStatus.MapEnum<CrmStatus>()
            }).ToList();
        }
    }
}