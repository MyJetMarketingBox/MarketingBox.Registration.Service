﻿using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Deposits
{
    [DataContract]
    public class DepositUpdateMessage
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
        public long Sequence { get; set; }

        [DataMember(Order = 6)]
        public long BrandId { get; set; }

        [DataMember(Order = 7)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 8)]
        public long CampaignId { get; set; }

        [DataMember(Order = 9)]
        public long BoxId { get; set; }


    }

}
