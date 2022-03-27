using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    [DataContract]
    public class Registration
    {
        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public RegistrationBrandInfo BrandInfo{ get; set; }

        [DataMember(Order = 5)]
        public RegistrationGeneralInfo OriginalData { get; set; }

        [DataMember(Order = 6)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 7)]
        public string RegistrationUId { get; set; }
    }
}