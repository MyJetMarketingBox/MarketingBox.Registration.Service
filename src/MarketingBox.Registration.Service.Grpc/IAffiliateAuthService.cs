using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate.Contracts;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateAuthService
    {
        [OperationContract]
        Task<AffiliateAuthResponse> IsValidAffiliateAuthInfoAsync(AffiliateAuthRequest request);
    }
}
