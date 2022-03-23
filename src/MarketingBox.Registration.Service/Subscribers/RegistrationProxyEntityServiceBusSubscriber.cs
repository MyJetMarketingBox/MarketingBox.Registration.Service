using System;
using System.Threading.Tasks;
using Autofac;
using DotNetCoreDecorators;
using MarketingBox.ExternalReferenceProxy.Api.Domain.Models;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Messages.Registrations;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Registration.Service.Subscribers
{
    public class RegistrationProxyEntityServiceBusSubscriber : IStartable
    {
        private readonly ILogger<RegistrationProxyEntityServiceBusSubscriber> _logger;
        private readonly IRegistrationRepository _repository;
        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _serviceBusPublisher;

        public RegistrationProxyEntityServiceBusSubscriber(
            ISubscriber<RegistrationProxyEntityServiceBus> subscriber,
            ILogger<RegistrationProxyEntityServiceBusSubscriber> logger, 
            IRegistrationRepository repository, 
            IServiceBusPublisher<RegistrationUpdateMessage> serviceBusPublisher)
        {
            _logger = logger;
            _repository = repository;
            _serviceBusPublisher = serviceBusPublisher;
            subscriber.Subscribe(Handle);
        }

        private async ValueTask Handle(RegistrationProxyEntityServiceBus message)
        {
            _logger.LogInformation("Consuming message {@context}", message);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(message.TenantId, message.RegistrationId);
                registration.RouteInfo.AutologinUsed = true;
                await _repository.SaveAsync(registration);
                await _serviceBusPublisher.PublishAsync(registration.MapToMessage());
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Error during consumptions {@context}", message);
                throw;
            }
            _logger.LogInformation("Has been consumed {@context}", message);
        }


        public void Start()
        {
        }
    }
}
