using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class RegistrationBrandInfo
    {
        [DataMember(Order = 1)]
        public ResultCode Status { get; set; }

        [DataMember(Order = 2)]
        public RegistrationCustomerInfo Data { get; set; }
        [DataMember(Order = 3)]
        public string Brand { get; set; }
    }
}