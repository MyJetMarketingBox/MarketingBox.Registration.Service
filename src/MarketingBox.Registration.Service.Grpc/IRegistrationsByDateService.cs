using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IRegistrationsByDateService
    {
        [OperationContract] 
        Task<RegistrationsGetByDateResponse> GetRegistrationsAsync(RegistrationsGetByDateRequest request);
        
        [OperationContract] 
        Task<RegistrationGetResponse> GetRegistrationAsync(RegistrationGetRequest request);
    }
}