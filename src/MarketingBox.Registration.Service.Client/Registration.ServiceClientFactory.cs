﻿using JetBrains.Annotations;
using MarketingBox.Registration.Service.Grpc;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.Registration.Service.Client
{
    [UsedImplicitly]
    public class RegistrationServiceClientFactory: MyGrpcClientFactory
    {
        public RegistrationServiceClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IRegistrationService GetRegistrationService() => CreateGrpcService<IRegistrationService>();

        public IDepositService GetDepositService() => CreateGrpcService<IDepositService>();

        public ICrmService GetCrmService() => CreateGrpcService<ICrmService>();

        public IAffiliateAuthService GetAffiliateAuthServiceService() => CreateGrpcService<IAffiliateAuthService>();
        
        public IRegistrationsByDateService GetCustomerService() => CreateGrpcService<IRegistrationsByDateService>();
    }
}
