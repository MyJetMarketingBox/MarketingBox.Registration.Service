﻿using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IDepositService
    {
        [OperationContract]
        Task<DepositResponse> RegisterDepositAsync(DepositCreateRequest request);

        [OperationContract]
        Task<DepositResponse> ApproveDepositAsync(DepositApproveRequest request);
    }
}
