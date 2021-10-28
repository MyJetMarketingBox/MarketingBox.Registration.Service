using System;
using System.Collections.Generic;

namespace MarketingBox.Registration.Service.Domain.Leads
{
    public class Lead
    {
        public string TenantId { get; }
        public long Sequence { get; private set; }
        public LeadGeneralInfo LeadInfo { get; private set; }
        public LeadAdditionalInfo AdditionalInfo { get; private set; }
        //public List<LeadRouteInfo> RouteInfo { get; private set; }
        public LeadRouteInfo RouteInfo { get; private set; }

        private Lead(string tenantId, long sequence, LeadGeneralInfo leadGeneralInfo, 
             LeadRouteInfo routeInfo, LeadAdditionalInfo additionalInfo)
        {
            TenantId = tenantId;
            Sequence = sequence;
            LeadInfo = leadGeneralInfo;
            RouteInfo = routeInfo;
            AdditionalInfo = additionalInfo;
        }

        private void ChangeStatus(LeadStatus from, LeadStatus to)
        {
            if (RouteInfo.Status != from)
                throw new Exception($"Transfer lead from {from} type to {to}, current status {RouteInfo.Status}");

            Sequence++;
            RouteInfo.Status = to;
            LeadInfo.UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Register(LeadCustomerInfo customerInfo)
        {
            ChangeStatus(LeadStatus.Created, LeadStatus.Registered);
            RouteInfo.CustomerInfo = customerInfo;
        }

        public void Deposit(DateTimeOffset depositDate)
        {
            ChangeStatus(LeadStatus.Registered, LeadStatus.Deposited);
            RouteInfo.DepositDate = depositDate;
        }

        public void Approved(DateTimeOffset depositDate)
        {
            ChangeStatus(LeadStatus.Deposited, LeadStatus.Approved);
            RouteInfo.ConversionDate = depositDate;
        }

        public static Lead Restore(string tenantId, long sequence, LeadGeneralInfo leadGeneralInfo, 
            LeadRouteInfo routeInfo, LeadAdditionalInfo additionalInfo)
        {
            return new Lead(
                tenantId,
                sequence,
                leadGeneralInfo,
                routeInfo,
                additionalInfo
            );
        }

    }
}
