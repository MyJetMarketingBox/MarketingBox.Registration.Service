using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts
{
    [DataContract]
    public class RegistrationCreateResponse
    {
        [DataMember(Order = 1)]
        public ResultCode Status { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public RegistrationBrandInfo BrandInfo{ get; set; }

        [DataMember(Order = 4)]
        public string FallbackUrl { get; set; }

        [DataMember(Order = 5)]
        public RegistrationGeneralInfo OriginalData { get; set; }

        [DataMember(Order = 6)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 7)]
        public string UniqueId { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}