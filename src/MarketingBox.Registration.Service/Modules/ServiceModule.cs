using Autofac;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Registration.Postgres.Repositories;
using MarketingBox.Registration.Service.Domain.Repositories;
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
            builder.RegisterType<RegistrationRouterService>().As<IRegistrationRouterService>().SingleInstance();
            builder.RegisterType<RegistrationRepository>().As<IRegistrationRepository>().InstancePerDependency();
        }
    }
}
