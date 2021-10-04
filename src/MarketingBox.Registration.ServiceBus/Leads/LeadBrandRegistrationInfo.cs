﻿using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class LeadBrandRegistrationInfo
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public string LoginUrl { get; set; }

        [DataMember(Order = 4)]
        public string Brand { get; set; }

        [DataMember(Order = 5)]
        public long BrandId { get; set; }
    }
}