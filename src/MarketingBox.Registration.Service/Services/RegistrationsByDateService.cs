using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Reporting.Service.Grpc;
using MarketingBox.Reporting.Service.Grpc.Requests.Registrations;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using RegistrationDetails = MarketingBox.Registration.Service.Domain.Models.Registrations.Registration;

namespace MarketingBox.Registration.Service.Services
{
    public class RegistrationsByDateService : IRegistrationsByDateService
    {
        private readonly ILogger<RegistrationsByDateService> _logger;
        private readonly IAffiliateService _customerReportService;
        private readonly IAffiliateClient _affiliateClient;
        private readonly IMapper _mapper;

        public RegistrationsByDateService(ILogger<RegistrationsByDateService> logger,
            IAffiliateService customerReportService, 
            IAffiliateClient affiliateClient, IMapper mapper)
        {
            _logger = logger;
            _customerReportService = customerReportService;
            _affiliateClient = affiliateClient;
            _mapper = mapper;
        }

        public async Task<Response<IReadOnlyCollection<RegistrationDetails>>> GetRegistrationsAsync(RegistrationsGetByDateRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                _logger.LogInformation("RegistrationsByDateService.GetRegistrationsAsync receive request : {@Request}", request);

                var tenantId = string.Empty;
                try
                {
                    var affiliate = await _affiliateClient.GetAffiliateByApiKeyAndId(
                        request.ApiKey,
                        request.AffiliateId.Value);
                    tenantId = affiliate.TenantId;
                }
                catch (NotFoundException)
                {
                    throw new UnauthorizedException("Affiliate", request.AffiliateId.Value);
                }

                var reportResponse = await _customerReportService.GetRegistrations(new RegistrationsByAffiliateRequest()
                {
                    From = request.From.Value,
                    To = request.To.Value,
                    Type = request.Type.Value,
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
                
                var tenantId = string.Empty;
                try
                {
                    var affiliate = await _affiliateClient.GetAffiliateByApiKeyAndId(
                        request.ApiKey,
                        request.AffiliateId.Value);
                    tenantId = affiliate.TenantId;
                }
                catch (NotFoundException)
                {
                    throw new UnauthorizedException("Affiliate", request.AffiliateId.Value);
                }
                var reportResponse = await _customerReportService.GetRegistration(new RegistrationByAffiliateRequest()
                {
                    RegistrationUid = request.RegistrationUId,
                    AffiliateId = affiliateId,
                    TenantId = tenantId
                });

                _logger.LogInformation("ReportService.GetRegistration response is: {@Request}", request);

                var res = reportResponse.Process();
                return new Response<RegistrationDetails>()
                {
                    Status = ResponseStatus.Ok,
                    Data = _mapper.Map<RegistrationDetails>(res)
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