using System;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts
{
    [DataContract]
    public class DepositApproveRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 3)]
        public ApproveMode Mode { get; set; }
    }
}