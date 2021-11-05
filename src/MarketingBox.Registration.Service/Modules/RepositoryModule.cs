using Autofac;
using MarketingBox.Registration.Postgres.Repositories;
using MarketingBox.Registration.Service.Domain.Repositories;

namespace MarketingBox.Registration.Service.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrationRepository>().As<IRegistrationRepository>().InstancePerDependency();
        }
    }
}
