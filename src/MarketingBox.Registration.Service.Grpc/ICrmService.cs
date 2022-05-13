using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Requests.Crm;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface ICrmService
    {
        [OperationContract]
        Task<Response<bool>> SetCrmStatusAsync(UpdateCrmStatusRequest request);
    }
}
