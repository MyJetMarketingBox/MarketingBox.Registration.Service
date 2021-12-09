using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Crm;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface ICrmService
    {
        [OperationContract]
        Task SetCrmStatusAsync(UpdateCrmStatusRequest request);
    }
}
