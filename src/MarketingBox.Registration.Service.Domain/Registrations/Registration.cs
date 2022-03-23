using MarketingBox.Registration.Service.Domain.Crm;
using System;

namespace MarketingBox.Registration.Service.Domain.Registrations
{
    public class Registration
    {
        public string TenantId { get; }
        public RegistrationGeneralInfo RegistrationInfo { get; }
        public RegistrationAdditionalInfo AdditionalInfo { get; }
        public RegistrationRouteInfo RouteInfo { get; }

        private Registration(string tenantId, RegistrationGeneralInfo registrationGeneralInfo,
             RegistrationRouteInfo routeInfo, RegistrationAdditionalInfo additionalInfo)
        {
            TenantId = tenantId;
            RegistrationInfo = registrationGeneralInfo;
            RouteInfo = routeInfo;
            AdditionalInfo = additionalInfo;
        }

        public void UpdateCrmStatus(CrmStatus crmStatus)
        {
            RouteInfo.CrmStatus = crmStatus;
            RegistrationInfo.UpdatedAt = DateTimeOffset.UtcNow;
        }

        private void ChangeStatus(RegistrationStatus to)
        {
            RouteInfo.Status = to;
            RegistrationInfo.UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Register(RegistrationCustomerInfo customerInfo)
        {
            ChangeStatus(RegistrationStatus.Registered);
            RouteInfo.CustomerInfo = customerInfo;
        }

        public void UpdateStatus(DepositUpdateMode type, RegistrationStatus newStatus)
        {
            switch (newStatus)
            {
                case RegistrationStatus.Created:
                    break;
                case RegistrationStatus.Registered:
                    ChangeStatus(RegistrationStatus.Registered);
                    RouteInfo.UpdateMode = type;
                    RouteInfo.ConversionDate = null;
                    break;
                case RegistrationStatus.Deposited:
                    ChangeStatus(RegistrationStatus.Deposited);
                    RouteInfo.DepositDate = DateTime.UtcNow;
                    break;
                case RegistrationStatus.Approved:
                    ChangeStatus(RegistrationStatus.Approved);
                    RouteInfo.UpdateMode = type;
                    RouteInfo.ConversionDate = DateTime.UtcNow;
                    break;
                case RegistrationStatus.Declined:
                    ChangeStatus(RegistrationStatus.Declined);
                    RouteInfo.UpdateMode = type;
                    RouteInfo.ConversionDate = null;
                    break;
                default:
                    break;
            }
        }
        
        public static Registration Restore(string tenantId, RegistrationGeneralInfo registrationGeneralInfo,
            RegistrationRouteInfo routeInfo, RegistrationAdditionalInfo additionalInfo)
        {
            return new Registration(
                tenantId,
                registrationGeneralInfo,
                routeInfo,
                additionalInfo
            );
        }

    }
}
