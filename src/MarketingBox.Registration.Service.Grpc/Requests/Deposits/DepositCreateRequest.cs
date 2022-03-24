using System;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Deposits
{
    [DataContract]
    public class DepositCreateRequest : ValidatableEntity
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public string CustomerId { get; set; }

        [DataMember(Order = 3)]
        public string Email { get; set; }

        [DataMember(Order = 4)]
        public string BrandName { get; set; }

        [DataMember(Order = 5)]
        public long BrandId { get; set; }
        
        [DataMember(Order = 6)]
        public DateTime CreatedAt { get; set; }
    }
}