using Autofac;
using MarketingBox.ExternalReferenceProxy.Service.Client;
using MarketingBox.Reporting.Service.Client;

namespace MarketingBox.Registration.Service.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterReportingServiceClient(Program.Settings.ReportingServiceUrl);
            builder.RegisterExternalReferenceProxyServiceClient(Program.Settings.ExternalReferenceProxyServiceUrl);
        }
    }
}