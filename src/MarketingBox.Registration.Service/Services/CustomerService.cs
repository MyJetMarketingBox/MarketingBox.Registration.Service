using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Reporting.Service.Grpc;
using MarketingBox.Reporting.Service.Grpc.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarketingBox.Registration.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly ICustomerReportService _customerReportService;

        public CustomerService(ILogger<CustomerService> logger, 
            ICustomerReportService customerReportService)
        {
            _logger = logger;
            _customerReportService = customerReportService;
        }

        public async Task<GetCustomersResponse> GetCustomers(GetCustomersRequest request)
        {
            try
            {
                _logger.LogInformation($"CustomerService.GetCustomers receive request : {JsonConvert.SerializeObject(request)}");

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
    }
}