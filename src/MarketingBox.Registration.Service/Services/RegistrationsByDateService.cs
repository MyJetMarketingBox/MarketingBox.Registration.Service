using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
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
using RegistrationDetails = MarketingBox.Registration.Service.Domain.Models.Registrations.RegistrationDetails;
using ReportingRegistrationDetails = MarketingBox.Reporting.Service.Domain.Models.RegistrationDetails;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationsByDateService : IRegistrationsByDateService
    {
        private readonly ILogger<RegistrationsByDateService> _logger;
        private readonly IAffiliateService _customerReportService;
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _affiliateNoSqlServerDataReader;
        private readonly IMapper _mapper;

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

        public RegistrationsByDateService(ILogger<RegistrationsByDateService> logger,
            IAffiliateService customerReportService, 
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader, IMapper mapper)
        {
            _logger = logger;
            _customerReportService = customerReportService;
            _affiliateNoSqlServerDataReader = affiliateNoSqlServerDataReader;
            _mapper = mapper;
        }

        public async Task<Response<IReadOnlyCollection<RegistrationDetails>>> GetRegistrationsAsync(RegistrationsGetByDateRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                _logger.LogInformation("RegistrationsByDateService.GetRegistrationsAsync receive request : {@Request}", request);

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
                    Data = reportResponse.Data?.Select(_mapper.Map<RegistrationDetails>).ToList()
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
                    Data = _mapper.Map<RegistrationDetails>(reportResponse.Data)
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