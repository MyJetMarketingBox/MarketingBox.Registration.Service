using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Deposits
{
    [DataContract]
    public class UpdateDepositStatusRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, StringLength(128, MinimumLength = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2), Required, Range(1,long.MaxValue)]
        public long? RegistrationId { get; set; }

        [DataMember(Order = 3), Required]
        public DepositUpdateMode? Mode { get; set; }
        
        [DataMember(Order = 4), Required]
        public RegistrationStatus? NewStatus { get; set; }
    }
}