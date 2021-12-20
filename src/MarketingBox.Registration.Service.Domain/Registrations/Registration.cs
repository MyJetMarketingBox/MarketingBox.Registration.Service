using MarketingBox.Registration.Service.Domain.Crm;
using System;

namespace MarketingBox.Registration.Service.Domain.Registrations
{
    public class Registration
    {
        public string TenantId { get; }
        public long Sequence { get; private set; }
        public RegistrationGeneralInfo RegistrationInfo { get; private set; }
        public RegistrationAdditionalInfo AdditionalInfo { get; private set; }
        public RegistrationRouteInfo RouteInfo { get; private set; }

        private Registration(string tenantId, long sequence, RegistrationGeneralInfo registrationGeneralInfo,
             RegistrationRouteInfo routeInfo, RegistrationAdditionalInfo additionalInfo)
        {
            TenantId = tenantId;
            Sequence = sequence;
            RegistrationInfo = registrationGeneralInfo;
            RouteInfo = routeInfo;
            AdditionalInfo = additionalInfo;
        }
        
        public void NextTry()
        {
            Sequence++;
        }

        public void UpdateCrmStatus(CrmStatus crmStatus)
        {
            //if (RouteInfo.CrmStatus.ToCrmStatus() == crmStatus)
            //    return;

            Sequence++;
            RouteInfo.CrmStatus = crmStatus;
            RegistrationInfo.UpdatedAt = DateTimeOffset.UtcNow;
        }

        private void ChangeStatus(RegistrationStatus from, RegistrationStatus to)
        {
            if (RouteInfo.Status != from)
                throw new Exception($"Transfer registration from {from} type to {to}, current status {RouteInfo.Status}");

            Sequence++;
            RouteInfo.Status = to;
            RegistrationInfo.UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Register(RegistrationCustomerInfo customerInfo)
        {
            ChangeStatus(RegistrationStatus.Created, RegistrationStatus.Registered);
            RouteInfo.CustomerInfo = customerInfo;
        }

        public void Deposit(DateTimeOffset depositDate)
        {
            ChangeStatus(RegistrationStatus.Registered, RegistrationStatus.Deposited);
            RouteInfo.DepositDate = depositDate;
        }

        public void Approve(DateTimeOffset depositDate, DepositUpdateMode type)
        {
            ChangeStatus(RegistrationStatus.Deposited, RegistrationStatus.Approved);
            RouteInfo.ApprovedType = type;
            RouteInfo.ConversionDate = depositDate;
        }

        public void Decline(DepositUpdateMode type)
        {
            ChangeStatus(RegistrationStatus.Deposited, RegistrationStatus.Declined);
            RouteInfo.ApprovedType = type;
            RouteInfo.ConversionDate = null;
        }

        public void ApproveDeclined(DateTimeOffset depositDate)
        {
            ChangeStatus(RegistrationStatus.Declined, RegistrationStatus.Approved);
            RouteInfo.ApprovedType = DepositUpdateMode.Manually; 
            RouteInfo.ConversionDate = depositDate;
        }

        public void DeclineApproved()
        {
            ChangeStatus(RegistrationStatus.Approved, RegistrationStatus.Declined);
            RouteInfo.ApprovedType = DepositUpdateMode.Manually;
            RouteInfo.ConversionDate = null;
        }

        public void ApproveRegistered(DateTimeOffset depositDate)
        {
            ChangeStatus(RegistrationStatus.Registered, RegistrationStatus.Approved);
            RouteInfo.ApprovedType = DepositUpdateMode.Manually;
            RouteInfo.ConversionDate = depositDate;
        }

        public void RegisterApproved()
        {
            ChangeStatus(RegistrationStatus.Approved, RegistrationStatus.Registered);
            RouteInfo.ApprovedType = DepositUpdateMode.Manually;
            RouteInfo.ConversionDate = null;
        }


        public static Registration Restore(string tenantId, long sequence, RegistrationGeneralInfo registrationGeneralInfo,
            RegistrationRouteInfo routeInfo, RegistrationAdditionalInfo additionalInfo)
        {
            return new Registration(
                tenantId,
                sequence,
                registrationGeneralInfo,
                routeInfo,
                additionalInfo
            );
        }

    }
}
