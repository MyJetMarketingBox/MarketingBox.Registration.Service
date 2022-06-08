using Autofac;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Auth.Service.Client;
using MarketingBox.ExternalReferenceProxy.Service.Client;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Reporting.Service.Client;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;

namespace MarketingBox.Registration.Service.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterIntegrationServiceClient(Program.Settings.IntegrationServiceUrl);
            builder.RegisterReportingServiceClient(Program.Settings.ReportingServiceUrl);
            builder.RegisterExternalReferenceProxyServiceClient(Program.Settings.ExternalReferenceProxyServiceUrl);

            var noSqlClient = builder.CreateNoSqlClient(
                Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort).Invoke(),
                new LoggerFactory());
            builder.RegisterCountryClient(Program.Settings.AffiliateServiceUrl, noSqlClient);
            builder.RegisterUserClient(Program.Settings.AuthServiceUrl, noSqlClient);
            builder.RegisterAffiliateClient(Program.Settings.AffiliateServiceUrl, noSqlClient);
            builder.RegisterOfferClient(Program.Settings.AffiliateServiceUrl, noSqlClient);
            builder.RegisterCampaignClient(Program.Settings.AffiliateServiceUrl, noSqlClient);
            builder.RegisterBrandClient(Program.Settings.AffiliateServiceUrl, noSqlClient);
        }
    }
}