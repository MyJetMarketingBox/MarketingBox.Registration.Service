using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration
{
    [DataContract]
    public class RegistrationDeleteRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, Range(1,long.MaxValue)]
        public long? RegistrationId { get; set; }
    }
}
