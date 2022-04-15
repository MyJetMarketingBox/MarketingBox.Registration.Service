using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Deposits
{
    [DataContract]
    public class UpdateDepositStatusRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, StringLength(128, MinimumLength = 1)]
        public string TenantId { get; set; }
        
        [DataMember(Order = 2), Required, Range(1, long.MaxValue)]
        public long UserId { get; set; }

        [DataMember(Order = 3), Required, Range(1,long.MaxValue)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 4), Required]
        public DepositUpdateMode Mode { get; set; }
        
        [DataMember(Order = 5), Required]
        public RegistrationStatus NewStatus { get; set; }
        
        [DataMember(Order = 6), Required, StringLength(512, MinimumLength = 1)]
        public string Comment { get; set; }
    }
}