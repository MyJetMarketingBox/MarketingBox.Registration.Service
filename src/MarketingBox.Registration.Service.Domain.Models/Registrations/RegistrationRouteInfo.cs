﻿using System;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Common;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
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
        public long? IntegrationId { get; set; }

        [DataMember(Order = 6)]
        public RegistrationStatus Status { get; set; }

        [DataMember(Order = 7)]
        [Obsolete("This property is obsolete. Use CrmStatus instead.", true)]
        public string CrmCrmStatus { get; set; }

        [DataMember(Order = 8)]
        public DateTimeOffset? DepositDate { get; set; }

        [DataMember(Order = 9)]
        public DateTimeOffset? ConversionDate { get; set; }

        [DataMember(Order = 10)]
        public RegistrationBrandInfo BrandInfo { get; set; }

        [DataMember(Order = 11)]
        public UpdateMode UpdateMode { get; set; }

        [DataMember(Order = 12)]
        public CrmStatus CrmStatus { get; set; }

        [DataMember(Order = 13)]
        public string AffiliateName { get; set; }
        [DataMember(Order = 14)]
        public bool AutologinUsed { get; set; }

    }
}


