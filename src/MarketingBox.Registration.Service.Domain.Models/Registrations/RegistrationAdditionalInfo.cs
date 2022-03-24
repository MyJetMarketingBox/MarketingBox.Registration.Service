using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    [DataContract]
    public class RegistrationAdditionalInfo
    {
        [DataMember(Order = 1)]
        [Obsolete("This property is obsolete. Use Funnel instead.", false)]
        public string So { get; set; }

        [Obsolete("This property is obsolete. Use AffCode instead.", false)]
        [DataMember(Order = 2)]
        public string Sub { get; set; }

        [DataMember(Order = 3)] public string Sub1 { get; set; }

        [DataMember(Order = 4)] public string Sub2 { get; set; }

        [DataMember(Order = 5)] public string Sub3 { get; set; }

        [DataMember(Order = 6)] public string Sub4 { get; set; }

        [DataMember(Order = 7)] public string Sub5 { get; set; }

        [DataMember(Order = 8)] public string Sub6 { get; set; }

        [DataMember(Order = 9)] public string Sub7 { get; set; }

        [DataMember(Order = 10)] public string Sub8 { get; set; }

        [DataMember(Order = 11)] public string Sub9 { get; set; }

        [DataMember(Order = 12)] public string Sub10 { get; set; }

        [DataMember(Order = 13)] public string Funnel { get; set; }

        [DataMember(Order = 14)] public string AffCode { get; set; }
    }
}