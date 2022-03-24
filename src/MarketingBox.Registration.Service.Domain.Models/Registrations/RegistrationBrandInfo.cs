using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations
{
    [DataContract]
    public class RegistrationBrandInfo
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public string LoginUrl { get; set; }

        [DataMember(Order = 4)]
        public string Brand { get; set; }
    }
}