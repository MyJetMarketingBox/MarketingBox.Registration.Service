using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IRegistrationsByDateService
    {
        [OperationContract] 
        Task<Response<IReadOnlyCollection<RegistrationDetails>>> GetRegistrationsAsync(RegistrationsGetByDateRequest request);
        
        [OperationContract] 
        Task<Response<RegistrationDetails>> GetRegistrationAsync(RegistrationGetRequest request);
    }
}