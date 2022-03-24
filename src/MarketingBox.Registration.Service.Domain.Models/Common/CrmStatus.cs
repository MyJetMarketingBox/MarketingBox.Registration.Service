namespace MarketingBox.Registration.Service.Domain.Models
{
    public enum CrmStatus
    {
        New,
        FullyActivated,
        NA,
        HighPriority,
        Callback,
        FailedExpectation,
        NotValid,
        NotInterested,
        Transfer,
        FollowUp,
        ConversionRenew,
        Unknown
    }

    public static class CrmStatusExtensions
    {
        public static CrmStatus ToCrmStatus(this string status)
        {
            switch (status.ToLower())
            {
                case "new":
                    return CrmStatus.New;

                case "fullyactivated":
                    return CrmStatus.FullyActivated;

                case "highpriority":
                    return CrmStatus.HighPriority;

                case "callback":
                    return CrmStatus.Callback;

                case "failedexpectation":
                    return CrmStatus.FailedExpectation;

                case "notvaliddeletedaccount":
                    return CrmStatus.NotValid;

                case "notinterested":
                    return CrmStatus.NotInterested;

                case "transfer":
                    return CrmStatus.Transfer;

                case "followup":
                    return CrmStatus.FollowUp;

                case "noanswer":
                    return CrmStatus.NA;

                case "conversionrenew":
                    return CrmStatus.ConversionRenew;

                default:
                    return CrmStatus.Unknown;
            }
        }

        public static string ToCrmStatus(this CrmStatus status)
        {
            switch (status)
            {
                case CrmStatus.New:
                    return "New";

                case CrmStatus.FullyActivated:
                    return "FullyActivated";

                case CrmStatus.HighPriority:
                    return "HighPriority";

                case CrmStatus.Callback:
                    return "Callback";

                case CrmStatus.FailedExpectation:
                    return "FailedExpectation";

                case CrmStatus.NotValid:
                    return "NotValidDeletedAccount";

                case CrmStatus.NotInterested:
                    return "NotInterested";

                case CrmStatus.Transfer:
                    return "Transfer";

                case CrmStatus.FollowUp:
                    return "Followup";

                case CrmStatus.NA:
                    return "NoAnswer";

                case CrmStatus.ConversionRenew:
                    return "ConversionRenew";

                default:
                    return "Unknown";
            }
        }
    }
}