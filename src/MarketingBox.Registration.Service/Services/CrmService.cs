using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Messages.Registrations;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using MarketingBox.Registration.Service.Grpc.Models.Crm;

namespace MarketingBox.Registration.Service.Services
{
    public class CrmService : ICrmService
    {
        private readonly ILogger<CrmService> _logger;

        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IRegistrationRepository _repository;

        public CrmService(ILogger<CrmService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated, 
            IRegistrationRepository repository)
        {
            _logger = logger;
            _publisherLeadUpdated = publisherLeadUpdated;
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
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating crm status {@context}", request);
            }
        }
    }
}
