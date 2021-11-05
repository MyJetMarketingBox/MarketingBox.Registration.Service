using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class RegistrationBrandInfo
    {
        [DataMember(Order = 1)]
        public string Status { get; set; }

        [DataMember(Order = 2)]
        public RegistrationCustomerInfo Data { get; set; }
    }
}