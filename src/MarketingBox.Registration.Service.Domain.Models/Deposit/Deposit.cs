using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Domain.Models.Deposit
{
    [DataContract]
    public class Deposit
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public DepositGeneralInfo GeneralInfo { get; set; }
    }
}
