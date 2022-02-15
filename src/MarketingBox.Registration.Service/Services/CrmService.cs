using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Crm;
using MarketingBox.Registration.Service.Messages.Registrations;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;

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
                var registration = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                registration.UpdateCrmStatus(request.Crm);

                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent crm status to service bus {@context}", request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating crm status {@context}", request);
            }
        }
    }
}
