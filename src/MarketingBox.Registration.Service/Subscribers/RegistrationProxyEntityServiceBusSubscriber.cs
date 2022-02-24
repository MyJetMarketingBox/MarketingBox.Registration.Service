using System;
using System.Threading.Tasks;
using Autofac;
using DotNetCoreDecorators;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Messages.Registrations;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Registration.Service.Subscribers
{
    public class RegistrationProxyEntityServiceBusSubscriber : IStartable
    {
        private readonly ILogger<RegistrationProxyEntityServiceBusSubscriber> _logger;
        private readonly IRegistrationRepository _repository;

        public RegistrationProxyEntityServiceBusSubscriber(
            ISubscriber<RegistrationUpdateMessage> subscriber,
            ILogger<RegistrationProxyEntityServiceBusSubscriber> logger, 
            IRegistrationRepository repository)
        {
            _logger = logger;
            _repository = repository;
            subscriber.Subscribe(Handle);
        }

        private async ValueTask Handle(RegistrationUpdateMessage message)
        {
            _logger.LogInformation("Consuming message {@context}", message);

            try
            {
                
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
