using Autofac;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Registration.Service.Messages;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Registration.Service.MyNoSql.RegistrationRouter;
using MarketingBox.Registration.Service.Services;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;

namespace MarketingBox.Registration.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    Program.LogFactory);

            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            var campaign = new MyNoSqlReadRepository<CampaignNoSql>(noSqlClient, CampaignNoSql.TableName);
            builder.RegisterInstance(campaign)
                .As<IMyNoSqlServerDataReader<CampaignNoSql>>();

            var campaignIndex = new MyNoSqlReadRepository<CampaignIndexNoSql>(noSqlClient, CampaignIndexNoSql.TableName);
            builder.RegisterInstance(campaignIndex)
                .As<IMyNoSqlServerDataReader<CampaignIndexNoSql>>();

            var brand = new MyNoSqlReadRepository<BrandNoSql>(noSqlClient, BrandNoSql.TableName);
            builder.RegisterInstance(brand)
                .As<IMyNoSqlServerDataReader<BrandNoSql>>();

            var campaignRow
            = new MyNoSqlReadRepository<CampaignRowNoSql>(noSqlClient, CampaignRowNoSql.TableName);
            builder.RegisterInstance(campaignRow)
                .As<IMyNoSqlServerDataReader<CampaignRowNoSql>>();

            var integration = new MyNoSqlReadRepository<IntegrationNoSql>(noSqlClient, IntegrationNoSql.TableName);
            builder.RegisterInstance(integration)
                .As<IMyNoSqlServerDataReader<IntegrationNoSql>>();

            var affiliate = new MyNoSqlReadRepository<AffiliateNoSql>(noSqlClient, AffiliateNoSql.TableName);
            builder.RegisterInstance(affiliate)
                .As<IMyNoSqlServerDataReader<AffiliateNoSql>>();

            var leadRouter = new MyNoSqlReadRepository<RegistrationRouterNoSqlEntity>(noSqlClient, RegistrationRouterNoSqlEntity.TableName);
            builder.RegisterInstance(leadRouter)
                .As<IMyNoSqlServerDataReader<RegistrationRouterNoSqlEntity>>();

            var leadRouterCapacitor = new MyNoSqlReadRepository<RegistrationRouterCapacitorBoxNoSqlEntity>(noSqlClient, RegistrationRouterCapacitorBoxNoSqlEntity.TableName);
            builder.RegisterInstance(leadRouterCapacitor)
                .As<IMyNoSqlServerDataReader<RegistrationRouterCapacitorBoxNoSqlEntity>>();

            builder.RegisterIntegrationServiceClient(Program.Settings.IntegrationServiceUrl);

            #region Registrations

            // publisher (IServiceBusPublisher<RegistrationUpdateMessage>)
            builder.RegisterMyServiceBusPublisher<RegistrationUpdateMessage>(serviceBusClient, Topics.RegistrationUpdateTopic, false);

            // register writer (IMyNoSqlServerDataWriter<RegistrationRouterNoSqlEntity>)
            builder.RegisterMyNoSqlWriter<RegistrationRouterNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), RegistrationRouterNoSqlEntity.TableName);

            // register writer (IMyNoSqlServerDataWriter<RegistrationRouterCapacitorBoxNoSqlEntity>)
            builder.RegisterMyNoSqlWriter<RegistrationRouterCapacitorBoxNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), RegistrationRouterCapacitorBoxNoSqlEntity.TableName);

            #endregion

            builder.RegisterType<RegistrationRouterService>().As<RegistrationRouterService>().SingleInstance();
        }
    }
}
