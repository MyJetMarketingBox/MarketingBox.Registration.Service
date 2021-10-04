﻿using System;

namespace MarketingBox.Registration.Postgres.Entities.Lead
{
    public class LeadEntity
    {
        public string TenantId { get; set; }
        public string UniqueId { get; set; }
        public long LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public LeadBrandRegistrationInfo BrandRegistrationInfo { get; set; }
        public LeadAdditionalInfo AdditionalInfo { get; set; }
        public LeadType Type { get; set; }
        public LeadStatus Status{ get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long Sequence { get; set; }
    }
}
