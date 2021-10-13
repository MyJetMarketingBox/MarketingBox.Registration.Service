﻿using System.ComponentModel;

namespace MarketingBox.Registration.Service.Messages.Common
{
    public enum ResultCode
    {
        [Description("Operation failed")]
        Failed = 0,
        [Description("Operation completed successfully")]
        CompletedSuccessfully = 1,
        [Description("Operation brand authentication")]
        RequiredAuthentication = 2,
    }
}