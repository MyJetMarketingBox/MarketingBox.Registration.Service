using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations
{
    [DataContract]
    public class RegistrationBrandInfo
    {
        [DataMember(Order = 1)]
        public RegistrationCustomerInfo Data { get; set; }
        [DataMember(Order = 2)]
        public string Brand { get; set; }
    }
}