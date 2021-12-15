using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Registration.Service.MyNoSql.RegistrationRouter;
using MarketingBox.Registration.Service.Modules;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MarketingBox.Registration.Service.Tests
{
    public class LeadRouterTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            const int boxId = 1;
            var leadRepository = new FakeRegistrationRepository();
            const string countryCode = "CH";
            var campaignBoxNoSql1 = CampaignRowNoSql.Create(boxId, 1, 1, countryCode, 1, 3, CapType.Lead, 100,
                new ActivityHours[]
                {
                    new ActivityHours()
                    {
                        IsActive = true,
                        Day = DateTime.Today.DayOfWeek,
                        From = new TimeSpan(0,0,0),
                        To = new TimeSpan(23,59,59),
                    }
                }, null, true, 0);
            var campaignBoxNoSql2 = CampaignRowNoSql.Create(boxId, 2, 1, countryCode, 1, 2, CapType.Lead, 100,
                new ActivityHours[]
                {
                    new ActivityHours()
                    {
                        IsActive = true,
                        Day = DateTime.Today.DayOfWeek,
                        From = new TimeSpan(0,0,0),
                        To = new TimeSpan(23,59,59),
                    }
                }, null, true, 0);
            var campaignBoxNoSql3 = CampaignRowNoSql.Create(boxId, 3, 1, countryCode, 1, 1, CapType.Lead, 100,
                new ActivityHours[]
                {
                    new ActivityHours()
                    {
                        IsActive = true,
                        Day = DateTime.Today.DayOfWeek,
                        From = new TimeSpan(0,0,0),
                        To = new TimeSpan(23,59,59),
                    }
                }, null, true, 0);

            var logger = LoggerFactory.Create((x) => x.AddConsole());
            var leadCounter = new FakeMyNoSqlReaderWriter<RegistrationRouterNoSqlEntity>();
            var capacitor = new FakeMyNoSqlReaderWriter<RegistrationRouterCapacitorBoxNoSqlEntity>();
            var campaignBoxNoSqlServerDataReader = new FakeMyNoSqlReaderWriter<CampaignRowNoSql>();

            await campaignBoxNoSqlServerDataReader.InsertOrReplaceAsync(campaignBoxNoSql1);
            await campaignBoxNoSqlServerDataReader.InsertOrReplaceAsync(campaignBoxNoSql2);
            await campaignBoxNoSqlServerDataReader.InsertOrReplaceAsync(campaignBoxNoSql3);

            const string tenantId = "default-tenant-id";
            var leadRouter = new RegistrationRouterService(
                campaignBoxNoSqlServerDataReader,
                leadRepository,
                leadCounter,
                leadCounter,
                capacitor,
                capacitor,
                logger.CreateLogger<RegistrationRouterService>());

            var filtered = await leadRouter.GetSuitableRoutes(boxId, countryCode);
            var campaignBox1 = await leadRouter.GetCampaignBox(tenantId, boxId, countryCode, filtered);
            var campaignBox2 = await leadRouter.GetCampaignBox(tenantId, boxId, countryCode, filtered);
            var campaignBox3 = await leadRouter.GetCampaignBox(tenantId, boxId, countryCode, filtered);
            var campaignBox11 = await leadRouter.GetCampaignBox(tenantId, boxId, countryCode, filtered);
            var campaignBox21 = await leadRouter.GetCampaignBox(tenantId, boxId, countryCode, filtered);
            var campaignBox12 = await leadRouter.GetCampaignBox(tenantId, boxId, countryCode, filtered);
            var campaignBox13 = await leadRouter.GetCampaignBox(tenantId, boxId, countryCode, filtered);

            Assert.AreEqual(campaignBoxNoSql1, campaignBox1);
            Assert.AreEqual(campaignBoxNoSql1, campaignBox11);
            Assert.AreEqual(campaignBoxNoSql1, campaignBox12);
            Assert.AreEqual(campaignBoxNoSql1, campaignBox13);

            Assert.AreEqual(campaignBoxNoSql2, campaignBox2);
            Assert.AreEqual(campaignBoxNoSql2, campaignBox21);

            Assert.AreEqual(campaignBoxNoSql3, campaignBox3);
        }
    }
}
