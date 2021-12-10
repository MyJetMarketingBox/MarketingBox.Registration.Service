using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Reporting.Service.Domain.Models;
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

                var isAuth = CheckAuth(request.AffiliateId, request.ApiKey);
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
                    Type = GetCustomersReportType(request.Type)
                });

                _logger.LogInformation($"ICustomerReportService.GetCustomersReport response is : {JsonConvert.SerializeObject(reportResponse)}");
                
                if (reportResponse.Success)
                    return new GetCustomersResponse()
                    {
                        Customers = GetCustomersGrpc(reportResponse.Customers)
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

        private static CustomersReportType GetCustomersReportType(CustomerType requestType)
        {
            return requestType switch
            {
                CustomerType.Leads => CustomersReportType.Leads,
                CustomerType.Deposits => CustomersReportType.Deposits,
                CustomerType.LeadsAndDeposits => CustomersReportType.LeadsAndDeposits,
                _ => throw new Exception()
            };
        }

        private bool CheckAuth(long affiliateId, string apiKey)
        {
            var affiliates = _affiliateNoSqlServerDataReader.Get();
            var affiliate = affiliates.FirstOrDefault(e => e.AffiliateId == affiliateId);
            return affiliate?.GeneralInfo != null && affiliate.GeneralInfo.ApiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request)
        {
            try
            {
                _logger.LogInformation($"CustomerService.GetCustomer receive request : {JsonConvert.SerializeObject(request)}");
                
                var isAuth = CheckAuth(request.AffiliateId, request.ApiKey);
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
                        Customer = GetCustomerGrpc(reportResponse.Customer)
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

        private static CustomerGrpc GetCustomerGrpc(Customer customer)
        {
            return new CustomerGrpc()
            {
                UId = customer.UId,
                TenantId = customer.TenantId,
                LastName = customer.LastName,
                Phone = customer.Phone,
                AffiliateId = customer.AffiliateId,
                BrandId = customer.BrandId,
                CampaignId = customer.CampaignId,
                Country = customer.Country,
                CreatedDate = customer.CreatedDate,
                DepositDate = customer.DepositDate,
                Email = customer.Email,
                FirstName = customer.FirstName,
                Ip = customer.Ip,
                IsDeposit = customer.IsDeposit,
                CrmStatus = customer.CrmStatus
            };
        }
        
        private static List<CustomerGrpc> GetCustomersGrpc(IEnumerable<Customer> customers)
        {
            return customers.Select(e => new CustomerGrpc()
            {
                UId = e.UId,
                TenantId = e.TenantId,
                LastName = e.LastName,
                Phone = e.Phone,
                AffiliateId = e.AffiliateId,
                BrandId = e.BrandId,
                CampaignId = e.CampaignId,
                Country = e.Country,
                CreatedDate = e.CreatedDate,
                DepositDate = e.DepositDate,
                Email = e.Email,
                FirstName = e.FirstName,
                Ip = e.Ip,
                IsDeposit = e.IsDeposit,
                //TODO Add CrmStatus
                //CrmStatus = e.CrmStatus
            }).ToList();
        }
    }
}