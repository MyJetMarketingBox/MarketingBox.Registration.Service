using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.MyNoSql.Registrations
{
    [DataContract]
    public class RegistrationCustomerInfo
    {
        public string CustomerId { get; set; }

        public string Token { get; set; }

        public string LoginUrl { get; set; }
        
        public string Brand { get; set; }
    }
}