using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class RegistrationUpdateMessage
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }
      
        [DataMember(Order = 2)]
        public RegistrationGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 3)]
        public RegistrationRouteInfo RouteInfo { get; set; }

        [DataMember(Order = 4)]
        public RegistrationAdditionalInfo AdditionalInfo { get; set; }
        
        [DataMember(Order = 5)]
        public long Sequence { get; set; }
    }
}
