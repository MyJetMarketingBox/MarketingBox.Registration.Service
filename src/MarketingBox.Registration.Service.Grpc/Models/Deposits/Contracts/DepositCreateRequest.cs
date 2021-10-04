﻿using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts
{
    [DataContract]
    public class DepositCreateRequest
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
        public DateTimeOffset CreatedAt { get; set; }
    }
}