using Autofac;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Registration.Service.MyNoSql.RegistrationRouter;
using MyJetWallet.Sdk.NoSql;

namespace MarketingBox.Registration.Service.Modules
{
    public class NoSqlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            
            builder.RegisterMyNoSqlReader<CampaignNoSql>(noSqlClient, CampaignNoSql.TableName);
            builder.RegisterMyNoSqlReader<CampaignIndexNoSql>(noSqlClient, CampaignIndexNoSql.TableName);
            builder.RegisterMyNoSqlReader<BrandNoSql>(noSqlClient, BrandNoSql.TableName);
            builder.RegisterMyNoSqlReader<CampaignRowNoSql>(noSqlClient, CampaignRowNoSql.TableName);
            builder.RegisterMyNoSqlReader<IntegrationNoSql>(noSqlClient, IntegrationNoSql.TableName);
            builder.RegisterMyNoSqlReader<AffiliateNoSql>(noSqlClient, AffiliateNoSql.TableName);
            builder.RegisterMyNoSqlReader<RegistrationRouterNoSqlEntity>(noSqlClient, RegistrationRouterNoSqlEntity.TableName);
            builder.RegisterMyNoSqlReader<RegistrationRouterCapacitorBoxNoSqlEntity>(noSqlClient, RegistrationRouterCapacitorBoxNoSqlEntity.TableName);

            builder.RegisterMyNoSqlWriter<RegistrationRouterNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), RegistrationRouterNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<RegistrationRouterCapacitorBoxNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), RegistrationRouterCapacitorBoxNoSqlEntity.TableName);
        }
    }
}