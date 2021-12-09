﻿using Autofac;
using MarketingBox.Registration.Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.Registration.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterRegistrationServiceClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new RegistrationServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetRegistrationService()).As<IRegistrationService>().SingleInstance();
            builder.RegisterInstance(factory.GetDepositService()).As<IDepositService>().SingleInstance();
            builder.RegisterInstance(factory.GetCrmService()).As<ICrmService>().SingleInstance();
            builder.RegisterInstance(factory.GetAffiliateAuthServiceService()).As<IAffiliateAuthService>().SingleInstance();
            
            builder.RegisterInstance(factory.GetCustomerService()).As<ICustomerService>().SingleInstance();
        }
    }
}
