using Autofac;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Registration.Service.Messages;
using MarketingBox.Registration.Service.Messages.Leads;
using MarketingBox.Registration.Service.MyNoSql.LeadRouter;
using MarketingBox.Registration.Service.MyNoSql.Leads;
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

            var box = new MyNoSqlReadRepository<BoxNoSql>(noSqlClient, BoxNoSql.TableName);
            builder.RegisterInstance(box)
                .As<IMyNoSqlServerDataReader<BoxNoSql>>();

            var boxIndex = new MyNoSqlReadRepository<BoxIndexNoSql>(noSqlClient, BoxIndexNoSql.TableName);
            builder.RegisterInstance(boxIndex)
                .As<IMyNoSqlServerDataReader<BoxIndexNoSql>>();

            var campaign = new MyNoSqlReadRepository<CampaignNoSql>(noSqlClient, CampaignNoSql.TableName);
            builder.RegisterInstance(campaign)
                .As<IMyNoSqlServerDataReader<CampaignNoSql>>();

            var campaignBox = new MyNoSqlReadRepository<CampaignBoxNoSql>(noSqlClient, CampaignBoxNoSql.TableName);
            builder.RegisterInstance(campaignBox)
                .As<IMyNoSqlServerDataReader<CampaignBoxNoSql>>();

            var brand = new MyNoSqlReadRepository<BrandNoSql>(noSqlClient, BrandNoSql.TableName);
            builder.RegisterInstance(brand)
                .As<IMyNoSqlServerDataReader<BrandNoSql>>();

            var partner = new MyNoSqlReadRepository<PartnerNoSql>(noSqlClient, PartnerNoSql.TableName);
            builder.RegisterInstance(partner)
                .As<IMyNoSqlServerDataReader<PartnerNoSql>>();

            var leadRouter = new MyNoSqlReadRepository<LeadRouterNoSqlEntity>(noSqlClient, LeadRouterNoSqlEntity.TableName);
            builder.RegisterInstance(leadRouter)
                .As<IMyNoSqlServerDataReader<LeadRouterNoSqlEntity>>();

            var leadRouterCapacitor = new MyNoSqlReadRepository<LeadRouterCapacitorBoxNoSqlEntity>(noSqlClient, LeadRouterCapacitorBoxNoSqlEntity.TableName);
            builder.RegisterInstance(leadRouter)
                .As<IMyNoSqlServerDataReader<LeadRouterCapacitorBoxNoSqlEntity>>();

            builder.RegisterIntegrationServiceClient(Program.Settings.IntegrationServiceUrl);

            #region Leads

            // publisher (IServiceBusPublisher<LeadUpdateMessage>)
            builder.RegisterMyServiceBusPublisher<LeadUpdateMessage>(serviceBusClient, Topics.LeadUpdateTopic, false);

            // register writer (IMyNoSqlServerDataWriter<LeadNoSqlEntity>)
            builder.RegisterMyNoSqlWriter<LeadNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), LeadNoSqlEntity.TableName);

            // register writer (IMyNoSqlServerDataWriter<LeadRouterNoSqlEntity>)
            builder.RegisterMyNoSqlWriter<LeadRouterNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), LeadRouterNoSqlEntity.TableName);

            // register writer (IMyNoSqlServerDataWriter<LeadRouterCapacitorBoxNoSqlEntity>)
            builder.RegisterMyNoSqlWriter<LeadRouterCapacitorBoxNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), LeadRouterCapacitorBoxNoSqlEntity.TableName);

            #endregion
        }
    }
}
