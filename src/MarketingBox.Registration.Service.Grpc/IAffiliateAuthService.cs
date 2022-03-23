using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate.Contracts;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateAuthService
    {
        [OperationContract]
        Task<Response<bool>> IsValidAffiliateAuthInfoAsync(AffiliateAuthRequest request);
    }
}
