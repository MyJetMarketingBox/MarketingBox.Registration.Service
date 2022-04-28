using Autofac;
using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Repositories;
using MarketingBox.Registration.Service.Services;
using MarketingBox.Registration.Service.Services.Interfaces;
using MarketingBox.Registration.Service.Subscribers;

namespace MarketingBox.Registration.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseContextFactory>().AsSelf().SingleInstance();
            builder.RegisterType<RouterFilterService>().As<IRouterFilterService>().SingleInstance();
            builder.RegisterType<RegistrationRepository>().As<IRegistrationRepository>().InstancePerDependency();
            builder.RegisterType<TrafficEngineService>().As<ITrafficEngineService>().InstancePerRequest();

            builder
                .RegisterType<RegistrationProxyEntityServiceBusSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
            
        }
    }
}
