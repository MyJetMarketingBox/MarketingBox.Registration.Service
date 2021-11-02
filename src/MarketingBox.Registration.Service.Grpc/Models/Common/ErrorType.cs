namespace MarketingBox.Registration.Service.Grpc.Models.Common
{
    public enum ErrorType
    {
        Unknown = 0,
        InvalidParameter = 1,
        AlreadyExist = 2,
        InvalidEmail = 3,
        InvalidUserNameOrPassword = 4,
        InvalidCountry = 5,
        InvalidPersonalData = 6,
        AlreadyUpdated = 7,
        InvalidAffiliateInfo = 8,
        Unauthorized = 401
    }
}