using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Deposits
{
    [DataContract]
    public class UpdateDepositStatusRequest : ValidatableEntity
    {
        [DataMember(Order = 1)]
        public long UpdatedBy { get; set; }
        
        [DataMember(Order = 2), Required]
        public string TenantId { get; set; }

        [DataMember(Order = 3)]
        public long RegistrationId { get; set; }

        [DataMember(Order = 4)]
        public DepositUpdateMode Mode { get; set; }
        
        [DataMember(Order = 5)]
        public RegistrationStatus NewStatus { get; set; }
        
        [DataMember(Order = 6)]
        public string Reason { get; set; }
    }
}