﻿namespace MarketingBox.Registration.Service.Grpc.Models.Common
{
    public enum RegistrationApprovedType
    {
        Unknown = 0,
        Declined = 1,
        Approved = 2,
        ApprovedManually = 3,
        ApprovedFromCrm = 4
    }
}