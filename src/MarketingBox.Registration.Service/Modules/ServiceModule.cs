﻿using Autofac;
using MarketingBox.Registration.Postgres.Repositories;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Services;
using MarketingBox.Registration.Service.Subscribers;

namespace MarketingBox.Registration.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrationRouterService>().As<IRegistrationRouterService>().SingleInstance();
            builder.RegisterType<RegistrationRepository>().As<IRegistrationRepository>().InstancePerDependency();

            builder
                .RegisterType<RegistrationProxyEntityServiceBusSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
