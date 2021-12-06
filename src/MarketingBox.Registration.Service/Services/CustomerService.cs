using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Reporting.Service.Grpc;
using MarketingBox.Reporting.Service.Grpc.Models;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;

namespace MarketingBox.Registration.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly ICustomerReportService _customerReportService;
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _affiliateNoSqlServerDataReader;

        public CustomerService(ILogger<CustomerService> logger, 
            ICustomerReportService customerReportService, 
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader)
        {
            _logger = logger;
            _customerReportService = customerReportService;
            _affiliateNoSqlServerDataReader = affiliateNoSqlServerDataReader;
        }

        public async Task<GetCustomersResponse> GetCustomers(GetCustomersRequest request)
        {
            try
            {
                _logger.LogInformation($"CustomerService.GetCustomers receive request : {JsonConvert.SerializeObject(request)}");

                var isAuth = CheckAuth(request.TenantId, request.AffiliateId, request.ApiKey);
                if (!isAuth)
                    return new GetCustomersResponse()
                    {
                        Error = new Error()
                        {
                            Type = ErrorType.Unauthorized
                        }
                    };

                if (request.From == DateTime.MinValue)
                    return new GetCustomersResponse()
                    {
                        Error = new Error() {Type = ErrorType.InvalidParameter, Message = "Date From cannot be empty"}
                    };
                if (request.To == DateTime.MinValue)
                    return new GetCustomersResponse()
                    {
                        Error = new Error() {Type = ErrorType.InvalidParameter, Message = "Date To cannot be empty"}
                    };

                var reportResponse = await _customerReportService.GetCustomersReport(new GetCustomersReportRequest()
                {
                    From = request.From,
                    To = request.To,
                    Type = request.Type
                });

                _logger.LogInformation($"ICustomerReportService.GetCustomersReport response is : {JsonConvert.SerializeObject(reportResponse)}");
                
                if (reportResponse.Success)
                    return new GetCustomersResponse()
                    {
                        Customers = reportResponse.Customers
                    };
                
                return new GetCustomersResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.Unknown,
                        Message = reportResponse.ErrorMessage
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new GetCustomersResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.Unknown,
                        Message = ex.Message
                    }
                };
            }
        }

        private bool CheckAuth(string tenantId, long affiliateId, string apiKey)
        {
            var partner =
                _affiliateNoSqlServerDataReader.Get(AffiliateNoSql.GeneratePartitionKey(tenantId),
                    AffiliateNoSql.GenerateRowKey(affiliateId));

            if (partner?.GeneralInfo != null)
                return partner.GeneralInfo.ApiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase);

            return false;
        }

        public async Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request)
        {
            try
            {
                _logger.LogInformation($"CustomerService.GetCustomer receive request : {JsonConvert.SerializeObject(request)}");
                
                var isAuth = CheckAuth(request.TenantId, request.AffiliateId, request.ApiKey);
                if (!isAuth)
                    return new GetCustomerResponse()
                    {
                        Error = new Error()
                        {
                            Type = ErrorType.Unauthorized
                        }
                    };
                var reportResponse = await _customerReportService.GetCustomerReport(new GetCustomerReportRequest()
                {
                    UId = request.UId
                });

                _logger.LogInformation($"ICustomerReportService.GetCustomerReport response is : {JsonConvert.SerializeObject(reportResponse)}");
                
                if (reportResponse.Success)
                    return new GetCustomerResponse()
                    {
                        Customer = reportResponse.Customer
                    };
                
                return new GetCustomerResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.Unknown,
                        Message = reportResponse.ErrorMessage
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new GetCustomerResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.Unknown,
                        Message = ex.Message
                    }
                };
            }
        }
    }
}