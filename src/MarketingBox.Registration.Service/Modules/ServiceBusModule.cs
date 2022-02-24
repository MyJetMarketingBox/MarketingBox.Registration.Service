using Autofac;
using MarketingBox.Registration.Service.Messages.Registrations;
using MyJetWallet.Sdk.ServiceBus;

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
        }
    }
}