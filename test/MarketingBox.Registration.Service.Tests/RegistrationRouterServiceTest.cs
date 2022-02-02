using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.MyNoSql.RegistrationRouter;
using MarketingBox.Registration.Service.Services;
using Moq;
using Moq.AutoMock;
using MyNoSqlServer.Abstractions;
using NUnit.Framework;

namespace MarketingBox.Registration.Service.Tests
{
    [TestFixture]
    public class RegistrationRouterServiceTest
    {
        private AutoMocker _autoMocker;
        private CampaignRowNoSql _campaignBoxNoSql1;
        private CampaignRowNoSql _campaignBoxNoSql2;
        private CampaignRowNoSql _campaignBoxNoSql3;
        private RegistrationRouterService _leadRouter;
        private const long BoxId = 1;
        private const long BrandId1 = 1;
        private const long BrandId2 = 2;
        private const long BrandId3 = 3;
        private const int Weight1 = 6;
        private const int Weight2 = 4;
        private const int Weight3 = 2;
        private const long CampaignRowId1 = 1;
        private const long CampaignRowId2 = 2;
        private const long CampaignRowId3 = 3;
        private const int Priority = 1;
        private const int Sequence = 0;
        private const int DailyCapValue = 100;
        private const bool EnableTraffic = true;
        private const string CountryCode = "CH";
        private const string TenantId = "default-tenant-id";
        private const CapType CapType = Affiliate.Service.Domain.Models.CampaignRows.CapType.Lead;

        private static ActivityHours GetActivityHours(Action<ActivityHours> action = null)
        {
            var activityHours = new ActivityHours()
            {
                IsActive = true,
                Day = DateTime.Today.DayOfWeek,
                From = new TimeSpan(0, 0, 0),
                To = new TimeSpan(23, 59, 59),
            };
            action?.Invoke(activityHours);
            return activityHours;
        }

        private static object[] _wrongActivityHours =
        {
            new object[] {Array.Empty<ActivityHours>()},
            new object[]
            {
                new[]
                {
                    GetActivityHours(x => x.IsActive = false)
                }
            },
            new object[]
            {
                new[]
                {
                    GetActivityHours(x => ++x.Day)
                }
            }
        };
        
        private async Task MakeWholeRoutingCycleAndAssert(List<CampaignRowNoSql> filtered)
        {
            // 1st iteration
            var campaignBox11 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            var campaignBox12 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            var campaignBox13 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            Assert.AreEqual(_campaignBoxNoSql1, campaignBox11);
            Assert.AreEqual(_campaignBoxNoSql2, campaignBox12);
            Assert.AreEqual(_campaignBoxNoSql3, campaignBox13);

            // 2nd iteration
            var campaignBox21 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            var campaignBox22 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            var campaignBox23 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            Assert.AreEqual(_campaignBoxNoSql1, campaignBox21);
            Assert.AreEqual(_campaignBoxNoSql2, campaignBox22);
            Assert.AreEqual(_campaignBoxNoSql3, campaignBox23);

            // 3rd iteration
            var campaignBox31 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            var campaignBox32 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            Assert.AreEqual(_campaignBoxNoSql1, campaignBox31);
            Assert.AreEqual(_campaignBoxNoSql2, campaignBox32);

            // 4th iteration
            var campaignBox41 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            var campaignBox42 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            Assert.AreEqual(_campaignBoxNoSql1, campaignBox41);
            Assert.AreEqual(_campaignBoxNoSql2, campaignBox42);

            // 5th iteration
            var campaignBox51 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            Assert.AreEqual(_campaignBoxNoSql1, campaignBox51);

            // 6th iteration
            var campaignBox61 = await _leadRouter.GetCampaignBox(TenantId, BoxId, CountryCode, filtered);
            Assert.AreEqual(_campaignBoxNoSql1, campaignBox61);
        }
        
        private void NoOtherCalls()
        {
            _autoMocker.GetMock<IRegistrationRepository>().VerifyNoOtherCalls();
            _autoMocker.GetMock<IMyNoSqlServerDataReader<CampaignRowNoSql>>().VerifyNoOtherCalls();
        }

        private static CampaignRowNoSql GetCampaignBoxNoSql(
            long brandId,
            long campaignRowId,
            int weight)
        {
            return CampaignRowNoSql.Create(
                BoxId,
                campaignRowId,
                brandId,
                CountryCode,
                Priority,
                weight,
                CapType,
                DailyCapValue,
                new[]
                {
                    GetActivityHours()
                },
                null,
                EnableTraffic,
                Sequence);
        }

        private void CreateCampaignBoxes()
        {
            _campaignBoxNoSql1 = GetCampaignBoxNoSql(BrandId1, CampaignRowId1, Weight1);
            _campaignBoxNoSql2 = GetCampaignBoxNoSql(BrandId2, CampaignRowId2, Weight2);
            _campaignBoxNoSql3 = GetCampaignBoxNoSql(BrandId3, CampaignRowId3, Weight3);
        }

        private void SetupRepository(CampaignRowNoSql campaignBoxNoSql, int count)
        {
            switch (campaignBoxNoSql.CapType)
            {
                case CapType.Lead:
                    _autoMocker.Setup<IRegistrationRepository, Task<int>>(
                            repository => repository.GetCountForRegistrations(
                                It.IsAny<DateTime>(),
                                campaignBoxNoSql.BrandId,
                                campaignBoxNoSql.CampaignId,
                                RegistrationStatus.Registered))
                        .ReturnsAsync(count)
                        .Verifiable();
                    break;

                case CapType.Ftds:
                    _autoMocker.Setup<IRegistrationRepository, Task<int>>(
                            repository => repository.GetCountForDeposits(
                                It.IsAny<DateTime>(),
                                campaignBoxNoSql.BrandId,
                                campaignBoxNoSql.CampaignId,
                                RegistrationStatus.Approved))
                        .ReturnsAsync(count)
                        .Verifiable();
                    break;
            }
        }
        
        [SetUp]
        public void Setup()
        {
            _autoMocker = new AutoMocker();

            CreateCampaignBoxes();

            _autoMocker
                .Setup<IMyNoSqlServerDataReader<CampaignRowNoSql>, IReadOnlyList<CampaignRowNoSql>>(
                    x => x.Get(
                        It.Is<string>(p => p == CampaignRowNoSql.GeneratePartitionKey(BoxId))))
                .Returns(() =>
                    new[]
                    {
                        _campaignBoxNoSql1,
                        _campaignBoxNoSql2,
                        _campaignBoxNoSql3
                    })
                .Verifiable();

            _autoMocker.Setup<IRegistrationRepository, Task<int>>(
                    repository => repository.GetCountForRegistrations(
                        It.IsAny<DateTime>(),
                        It.IsAny<long>(),
                        It.IsAny<long>(),
                        RegistrationStatus.Registered))
                .ReturnsAsync(0)
                .Verifiable();

            var capacitor = new FakeMyNoSqlReaderWriter<RegistrationRouterCapacitorBoxNoSqlEntity>();
            var leadCounter = new FakeMyNoSqlReaderWriter<RegistrationRouterNoSqlEntity>();
            _autoMocker.Use<IMyNoSqlServerDataReader<RegistrationRouterCapacitorBoxNoSqlEntity>>(capacitor);
            _autoMocker.Use<IMyNoSqlServerDataWriter<RegistrationRouterCapacitorBoxNoSqlEntity>>(capacitor);
            _autoMocker.Use<IMyNoSqlServerDataReader<RegistrationRouterNoSqlEntity>>(leadCounter);
            _autoMocker.Use<IMyNoSqlServerDataWriter<RegistrationRouterNoSqlEntity>>(leadCounter);

            _leadRouter = _autoMocker.CreateInstance<RegistrationRouterService>();
        }

        /// <summary>
        /// This test checks routing for the following case:
        /// BrandId | Weight |Iteration 1(7)||Iteration 2(8)||Iteration 3(9)||Iteration 4(10)||Iteration 5(11)||Iteration 6(12)|
        /// --------|--------|--------------||--------------||--------------||---------------||---------------||---------------|
        ///    1    |   6    | 1 |   |      || 1 |   |      ||  1  |        ||  1  |         ||     1         ||     1         |
        ///    2    |   4    |   | 1 |      ||   | 1 |      ||     |  1     ||     |  1      ||               ||               |
        ///    3    |   2    |   |   | 1    ||   |   | 1    ||     |        ||     |         ||               ||               |
        /// </summary>
        [Test]
        public async Task GetCampaignBoxForDifferentWeightsTest()
        {
            var filtered = new List<CampaignRowNoSql>
            {
                _campaignBoxNoSql1,
                _campaignBoxNoSql2,
                _campaignBoxNoSql3
            };
            // iterations 1-6
            await MakeWholeRoutingCycleAndAssert(filtered);
            
            // iterations 7-12
            await MakeWholeRoutingCycleAndAssert(filtered);
        }

        /// <summary>
        /// Test that all campaigns pass by filters.
        /// </summary>
        [Test]
        public async Task GetSuitableRoutesAllCampaignPassTest()
        {
            var routes = await _leadRouter.GetSuitableRoutes(BoxId, CountryCode);

            CollectionAssert.IsNotEmpty(routes);
            Assert.That(routes, Has.Exactly(3).Items);

            _autoMocker.Verify();
            NoOtherCalls();
        }

        /// <summary>
        /// Test that campaign with EnableTraffic = false does not pass by filter.
        /// </summary>
        [Test]
        public async Task GetSuitableRoutesEnableTrafficTest()
        {
            _campaignBoxNoSql1.EnableTraffic = false;

            var routes = await _leadRouter.GetSuitableRoutes(BoxId, CountryCode);

            CollectionAssert.IsNotEmpty(routes);
            Assert.That(routes, Has.Exactly(2).Items);
            CollectionAssert.DoesNotContain(routes, _campaignBoxNoSql1);

            _autoMocker.Verify();
            NoOtherCalls();
        }

        /// <summary>
        /// Test that campaign with wrong CountryCode does not pass by filter.
        /// </summary>
        [Test]
        public async Task GetSuitableRoutesWrongCountryCodeTest()
        {
            _campaignBoxNoSql1.CountryCode = "UA";

            var routes = await _leadRouter.GetSuitableRoutes(BoxId, CountryCode);

            CollectionAssert.IsNotEmpty(routes);
            Assert.That(routes, Has.Exactly(2).Items);
            CollectionAssert.DoesNotContain(routes, _campaignBoxNoSql1);

            _autoMocker.Verify();
            NoOtherCalls();
        }

        /// <summary>
        /// Test that campaign with empty list of activity hours
        /// or with IsActive=false or with another day does not pass by filter.
        /// </summary>
        [TestCaseSource(nameof(_wrongActivityHours))]
        public async Task GetSuitableRoutesActivityHoursNotSetTest(ActivityHours[] activityHours)
        {
            _campaignBoxNoSql1.ActivityHours = activityHours;

            var routes = await _leadRouter.GetSuitableRoutes(BoxId, CountryCode);

            CollectionAssert.IsNotEmpty(routes);
            Assert.That(routes, Has.Exactly(2).Items);
            CollectionAssert.DoesNotContain(routes, _campaignBoxNoSql1);
        }

        /// <summary>
        /// Test that campaign does not pass by filter
        /// if amount of registrations is equal to DailyCapValue. 
        /// </summary>
        [TestCase(CapType.Lead)]
        [TestCase(CapType.Ftds)]
        public async Task GetSuitableRoutesDailyCapValueOverflowTest(CapType capType)
        {
            _campaignBoxNoSql1.CapType = capType;
            SetupRepository(_campaignBoxNoSql1, 100);

            var routes = await _leadRouter.GetSuitableRoutes(BoxId, CountryCode);

            CollectionAssert.IsNotEmpty(routes);
            Assert.That(routes, Has.Exactly(2).Items);
            CollectionAssert.DoesNotContain(routes, _campaignBoxNoSql1);

            _autoMocker.Verify();
            NoOtherCalls();
        }
    }
}