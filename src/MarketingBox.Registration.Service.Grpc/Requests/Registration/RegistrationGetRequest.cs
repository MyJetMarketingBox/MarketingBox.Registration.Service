using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration
{
    [DataContract]
    public class RegistrationGetRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, Range(1,long.MaxValue)] public long? AffiliateId { get; set; }
        [DataMember(Order = 2), Required] public string ApiKey { get; set; }
        [DataMember(Order = 3), Required] public string RegistrationUId { get; set; }
    }
}