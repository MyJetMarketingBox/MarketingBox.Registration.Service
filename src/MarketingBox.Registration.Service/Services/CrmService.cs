using System;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Crm;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Registration.Service.Services
{
    public class CrmService : ICrmService
    {
        private readonly ILogger<CrmService> _logger;
        private readonly IMapper _mapper;

        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IRegistrationRepository _repository;

        public CrmService(ILogger<CrmService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated, 
            IRegistrationRepository repository, IMapper mapper)
        {
            _logger = logger;
            _publisherLeadUpdated = publisherLeadUpdated;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<bool>> SetCrmStatusAsync(UpdateCrmStatusRequest request)
        {
            _logger.LogInformation("Update crm status {@context}", request);
            try
            {
                request.ValidateEntity();
                
                var registration = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                registration.CrmStatus = request.Crm.Value;
                registration.UpdatedAt = DateTimeOffset.UtcNow;
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(_mapper.Map<RegistrationUpdateMessage>(registration));
                _logger.LogInformation("Sent crm status to service bus {@context}", request);
                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating crm status {@context}", request);
                return e.FailedResponse<bool>();
            }
        }
    }
}
