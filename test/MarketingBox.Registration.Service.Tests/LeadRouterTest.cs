using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Registration.Service.Services;
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
            var leadRepository = new FakeLeadRepository();
            var campaignBoxNoSql1 = new CampaignBoxNoSql()
            {
                ActivityHours = new ActivityHours[]
                {
                    new ActivityHours()
                    {
                        IsActive = true,
                        Day = DateTime.Today.DayOfWeek,
                        From = DateTime.Today,
                        To = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59),
                    }
                },
                BoxId = 1,
                CapType = CapType.Lead,
                DailyCapValue = 100,
                Priority = 1,
                Weight = 3,
                CampaignBoxId = 1,
                CampaignId = 1,
                CountryCode = "CH",
                EnableTraffic = true,
                Sequence = 0,
            };
            var campaignBoxNoSql2 = new CampaignBoxNoSql()
            {
                ActivityHours = new ActivityHours[]
                {
                    new ActivityHours()
                    {
                        IsActive = true,
                        Day = DateTime.Today.DayOfWeek,
                        From = DateTime.Today,
                        To = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59),
                    }
                },
                BoxId = 1,
                CapType = CapType.Lead,
                DailyCapValue = 100,
                Priority = 1,
                Weight = 2,
                CampaignBoxId = 2,
                CampaignId = 1,
                CountryCode = "CH",
                EnableTraffic = true,
                Sequence = 0,
            };
            var campaignBoxNoSql3 = new CampaignBoxNoSql()
            {
                ActivityHours = new ActivityHours[]
                {
                    new ActivityHours()
                    {
                        IsActive = true,
                        Day = DateTime.Today.DayOfWeek,
                        From = DateTime.Today,
                        To = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59),
                    }
                },
                BoxId = 1,
                CapType = CapType.Lead,
                DailyCapValue = 100,
                Priority = 1,
                Weight = 1,
                CampaignBoxId = 3,
                CampaignId = 1,
                CountryCode = "CH",
                EnableTraffic = true,
                Sequence = 0,
            };

            var leadRouter = new LeadRouter(1, 0,new CampaignBoxNoSql[]
            {
                campaignBoxNoSql1,
                campaignBoxNoSql2,
                campaignBoxNoSql3,
            }, leadRepository);

            var campaignBox1 = await leadRouter.GetCampaignBox("CH");
            var campaignBox2 = await leadRouter.GetCampaignBox("CH");
            var campaignBox3 = await leadRouter.GetCampaignBox("CH");
            var campaignBox1_1 = await leadRouter.GetCampaignBox("CH");
            var campaignBox2_1 = await leadRouter.GetCampaignBox("CH");
            var campaignBox1_2 = await leadRouter.GetCampaignBox("CH");
            var campaignBox1_3 = await leadRouter.GetCampaignBox("CH");

            Assert.AreEqual(campaignBoxNoSql1, campaignBox1);
            Assert.AreEqual(campaignBoxNoSql1, campaignBox1_1);
            Assert.AreEqual(campaignBoxNoSql1, campaignBox1_2);
            Assert.AreEqual(campaignBoxNoSql1, campaignBox1_3);
        }
    }
}
