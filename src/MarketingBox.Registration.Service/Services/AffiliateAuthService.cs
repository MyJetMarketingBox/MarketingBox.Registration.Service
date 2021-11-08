using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Common;


namespace MarketingBox.Registration.Service.Services
{
    public class AffiliateAuthService : IAffiliateAuthService
    {
        private readonly ILogger<IAffiliateAuthService> _logger;
        private readonly IMyNoSqlServerDataReader<CampaignIndexNoSql> _campaignIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _affiliateNoSqlServerDataReader;

        public AffiliateAuthService(ILogger<IAffiliateAuthService> logger, 
            IMyNoSqlServerDataReader<CampaignIndexNoSql> campaignIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<AffiliateNoSql> affiliateNoSqlServerDataReader)

        {
            _logger = logger;
            _affiliateNoSqlServerDataReader = affiliateNoSqlServerDataReader;
            _campaignIndexNoSqlServerDataReader = campaignIndexNoSqlServerDataReader;
        }

        public async Task<AffiliateAuthResponse> IsValidAffiliateAuthInfoAsync(AffiliateAuthRequest request)
        {
            _logger.LogInformation("Auth new affiliate request {@context}", request);
            try
            {
                var boxIndexNoSql = _campaignIndexNoSqlServerDataReader
                .Get(CampaignIndexNoSql.GeneratePartitionKey(request.AuthInfo.CampaignId)).FirstOrDefault();

                var partner =
                    _affiliateNoSqlServerDataReader.Get(AffiliateNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                        AffiliateNoSql.GenerateRowKey(request.AuthInfo.AffiliateId));

                var partnerApiKey = partner.GeneralInfo.ApiKey;

                if (!IsAffiliateApiKeyValid(request.AuthInfo.ApiKey, partnerApiKey))
                {
                    return await Task.FromResult<AffiliateAuthResponse>(new AffiliateAuthResponse()
                    {
                        Status = ResultCode.RequiredAuthentication,
                        Error = new Error()
                        {
                            Message = $"Require '{request.AuthInfo.AffiliateId}' authentication",
                            Type = ErrorType.InvalidAffiliateInfo
                        }
                    });
                }

                return await Task.FromResult<AffiliateAuthResponse>(new AffiliateAuthResponse()
                {
                    Status = ResultCode.CompletedSuccessfully
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return await Task.FromResult<AffiliateAuthResponse>(new AffiliateAuthResponse()
                {
                    Status = ResultCode.Failed,
                    Error = new Error()
                    {
                        Message = "Affiliate authentication error", 
                        Type = ErrorType.Unknown
                    }
                });
            }
        }

        private bool IsAffiliateApiKeyValid(string requestApiKey, string apiKey)
        {
            return apiKey.Equals(requestApiKey, StringComparison.OrdinalIgnoreCase);
        }

    }
}
