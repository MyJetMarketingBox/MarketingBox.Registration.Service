using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IRegistrationsByDateService
    {
        [OperationContract]
        Task<Response<IReadOnlyCollection<Domain.Models.Registrations.Registration>>> GetRegistrationsAsync(RegistrationsGetByDateRequest request);

        [OperationContract]
        Task<Response<Domain.Models.Registrations.Registration>> GetRegistrationAsync(RegistrationGetRequest request);
    }
}