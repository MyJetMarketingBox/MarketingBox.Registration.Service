using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models
{
    [DataContract]
    public class CustomerGrpc
    {
        [DataMember(Order = 1)]
        public string UId { get; set; }

        [DataMember(Order = 2)]
        public DateTime CreatedDate { get; set; }

        [DataMember(Order = 3)]
        public string TenantId { get; set; }

        [DataMember(Order = 4)]
        public string FirstName { get; set; }

        [DataMember(Order = 5)]
        public string LastName { get; set; }

        [DataMember(Order = 6)]
        public string Email { get; set; }

        [DataMember(Order = 7)]
        public string Phone { get; set; }

        [DataMember(Order = 8)]
        public string Ip { get; set; }

        [DataMember(Order = 9)]
        public string Country { get; set; }

        [DataMember(Order = 10)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 11)]
        public long BrandId { get; set; }

        [DataMember(Order = 12)]
        public long CampaignId { get; set; }

        [DataMember(Order = 13)]
        public bool IsDeposit { get; set; }

        [DataMember(Order = 14)]
        public DateTime DepositDate { get; set; }

        [DataMember(Order = 15)]
        public string CrmStatus { get; set; }
    }
}