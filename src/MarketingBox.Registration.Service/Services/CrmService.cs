using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Registration.Service.MyNoSql.Registrations;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using ErrorType = MarketingBox.Registration.Service.Grpc.Models.Common.ErrorType;

namespace MarketingBox.Registration.Service.Services
{
    public class CrmService : ICrmService
    {
        private readonly ILogger<CrmService> _logger;

        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<RegistrationNoSqlEntity> _myNoSqlServerDataWriter;
        private readonly IRegistrationRepository _repository;

        public CrmService(ILogger<CrmService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated, 
            IMyNoSqlServerDataWriter<RegistrationNoSqlEntity> myNoSqlServerDataWriter,
            IRegistrationRepository repository)
        {
            _logger = logger;
            _publisherLeadUpdated = publisherLeadUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _repository = repository;
        }

        public async Task SetCrmStatusAsync(UpdateCrmStatusRequest request)
        {
            _logger.LogInformation("Update crm status {@context}", request);
            try
            {
                var lead = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                lead.UpdateCrmStatus(request.Crm);

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent crm status to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(lead.MapToNoSql());
                _logger.LogInformation("Sent crm status to MyNoSql {@context}", request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating crm status {@context}", request);
            }
        }
    }
}
