using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IRegistrationService
    {
        [OperationContract]
        Task<RegistrationCreateResponse> CreateAsync(RegistrationCreateRequest request);
    }
}
