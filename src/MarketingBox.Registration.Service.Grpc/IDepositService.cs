using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models;
using MarketingBox.Registration.Service.Domain.Models.Deposit;
using MarketingBox.Registration.Service.Grpc.Requests.Deposits;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IDepositService
    {
        [OperationContract]
        Task<Response<Deposit>> RegisterDepositAsync(DepositCreateRequest request);

        [OperationContract]
        Task<Response<Deposit>> UpdateDepositStatusAsync(UpdateDepositStatusRequest request);
    }
}
