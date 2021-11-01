using JetBrains.Annotations;
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

        public ILeadService GetRegistrationService() => CreateGrpcService<ILeadService>();

        public IDepositService GetDepositService() => CreateGrpcService<IDepositService>();

        public IAffiliateAuthService GetAffiliateAuthServiceService() => CreateGrpcService<IAffiliateAuthService>();
    }
}
