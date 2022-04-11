using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    [DataContract]
    public class RegistrationGeneralInfo : ValidatableEntity
    {
        [DataMember(Order = 1)]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string FirstName { get; set; }

        [DataMember(Order = 2)]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string LastName { get; set; }

        [DataMember(Order = 3)]
        [Required]
        [IsValidPassword]
        [StringLength(128, MinimumLength = 6)]
        [LogMasked(PreserveLength = true)]
        public string Password { get; set; }

        [DataMember(Order = 4)]
        [Required]
        [IsValidEmail]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        [DataMember(Order = 5), Phone]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }

        [DataMember(Order = 6)]
        [Required]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "IP address has incorrect format.")]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Ip { get; set; }

        [DataMember(Order = 7)] 
        [Required]
        [IsEnum]
        public CountryCodeType? CountryCodeType { get; set; }

        [DataMember(Order = 8)]
        [Required]
        [StringLength(3, MinimumLength = 2)]
        public string CountryCode { get; set; }
    }
}