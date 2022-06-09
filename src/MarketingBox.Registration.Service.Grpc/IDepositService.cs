﻿using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
using MarketingBox.Registration.Service.Grpc.Requests.Deposits;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IDepositService
    {
        [OperationContract]
        Task<Response<Domain.Models.Registrations.Registration>> RegisterDepositAsync(DepositCreateRequest request);

        [OperationContract]
        Task<Response<Domain.Models.Registrations.Registration>> UpdateDepositStatusAsync(UpdateDepositStatusRequest request);
        
        [OperationContract]
        Task<Response<List<StatusChangeLog>>> GetStatusChangeLogAsync(GetStatusChangeLogRequest request);
    }
}
