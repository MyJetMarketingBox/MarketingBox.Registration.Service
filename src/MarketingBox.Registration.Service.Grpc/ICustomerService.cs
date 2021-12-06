using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface ICustomerService
    {
        [OperationContract] 
        Task<GetCustomersResponse> GetCustomers(GetCustomersRequest request);
        
        [OperationContract] 
        Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request);
    }
}