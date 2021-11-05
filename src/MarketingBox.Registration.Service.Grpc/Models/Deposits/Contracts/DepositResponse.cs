using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;

namespace MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts
{
    [DataContract]
    public class DepositResponse
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public DepositGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }

    }
}
