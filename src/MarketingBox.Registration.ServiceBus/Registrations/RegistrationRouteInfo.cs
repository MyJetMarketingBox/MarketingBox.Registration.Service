﻿using System;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Messages.Common;

namespace MarketingBox.Registration.Service.Messages.Registrations
{
    [DataContract]
    public class RegistrationRouteInfo
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public long CampaignId { get; set; }

        [DataMember(Order = 3)]
        public long BrandId { get; set; }

        [DataMember(Order = 4)]
        public string Integration { get; set; }

        [DataMember(Order = 5)]
        public long IntegrationId { get; set; }

        [DataMember(Order = 6)]
        public LeadStatus Status { get; set; }

        [DataMember(Order = 7)]
        public string CrmCrmStatus { get; set; }

        [DataMember(Order = 8)]
        public DateTime? DepositDate { get; set; }

        [DataMember(Order = 9)]
        public DateTime? ConversionDate { get; set; }

        [DataMember(Order = 10)]
        public RegistrationCustomerInfo CustomerInfo { get; set; }

        [DataMember(Order = 11)]
        public LeadApprovedType ApprovedType { get; set; }
    }
}

