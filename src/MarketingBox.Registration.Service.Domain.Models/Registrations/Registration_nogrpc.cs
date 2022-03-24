using System;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;

namespace MarketingBox.Registration.Service.Domain.Models.Registrations
{
    public class Registration_nogrpc
    {
        public string TenantId { get; }
        public RegistrationGeneralInfo_notgrpc RegistrationInfoNotgrpc { get; }
        public RegistrationAdditionalInfo AdditionalInfo { get; }
        public RegistrationRouteInfo RouteInfo { get; }

        private Registration_nogrpc(string tenantId, RegistrationGeneralInfo_notgrpc registrationGeneralInfoNotgrpc,
             RegistrationRouteInfo routeInfo, RegistrationAdditionalInfo additionalInfo)
        {
            TenantId = tenantId;
            RegistrationInfoNotgrpc = registrationGeneralInfoNotgrpc;
            RouteInfo = routeInfo;
            AdditionalInfo = additionalInfo;
        }

        public void UpdateCrmStatus(CrmStatus crmStatus)
        {
            RouteInfo.CrmStatus = crmStatus;
            RegistrationInfoNotgrpc.UpdatedAt = DateTimeOffset.UtcNow;
        }

        private void ChangeStatus(RegistrationStatus to)
        {
            RouteInfo.Status = to;
            RegistrationInfoNotgrpc.UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Register(RegistrationBrandInfo brandInfo)
        {
            ChangeStatus(RegistrationStatus.Registered);
            RouteInfo.BrandInfo = brandInfo;
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
        
        public static Registration_nogrpc Restore(string tenantId, RegistrationGeneralInfo_notgrpc registrationGeneralInfoNotgrpc,
            RegistrationRouteInfo routeInfo, RegistrationAdditionalInfo additionalInfo)
        {
            return new Registration_nogrpc(
                tenantId,
                registrationGeneralInfoNotgrpc,
                routeInfo,
                additionalInfo
            );
        }

    }
}
