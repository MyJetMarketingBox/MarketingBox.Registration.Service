﻿using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.Registration.Service.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxRegistrationService.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxRegistrationService.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MarketingBoxRegistrationService.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("MarketingBoxRegistrationService.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("MarketingBoxRegistrationService.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("MarketingBoxRegistrationService.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("MarketingBoxRegistrationService.MarketingBoxServiceBusHostPort")]
        public string MarketingBoxServiceBusHostPort { get; set; }

        [YamlProperty("MarketingBoxRegistrationService.IntegrationServiceUrl")]
        public string IntegrationServiceUrl { get; set; }
    }
}
