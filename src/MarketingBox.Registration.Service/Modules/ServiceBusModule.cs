using Autofac;
using MarketingBox.ExternalReferenceProxy.Api.Domain.Models;
using MarketingBox.Registration.Service.Messages.Registrations;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;

namespace MarketingBox.Registration.Service.Modules
{
    public class ServiceBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    Program.LogFactory);
            
            
            builder.RegisterMyServiceBusPublisher<RegistrationUpdateMessage>(serviceBusClient, RegistrationUpdateMessage.Topic, false);
            
            const string queueName = "marketingbox-reporting-service";
            builder.RegisterMyServiceBusSubscriberSingle<RegistrationProxyEntityServiceBus>(
                serviceBusClient,
                RegistrationProxyEntityServiceBus.Topic,
                queueName,
                TopicQueueType.Permanent);
        }
    }
}