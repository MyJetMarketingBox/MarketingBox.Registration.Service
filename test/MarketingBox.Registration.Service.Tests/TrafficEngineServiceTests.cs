using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Integration.Service.Grpc.Models.Registrations;
using MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration;
using MarketingBox.Registration.Service.MyNoSql.TrafficEngine;
using MarketingBox.Registration.Service.Services;
using MarketingBox.Registration.Service.Services.Interfaces;
using MarketingBox.Sdk.Common.Models.Grpc;
using RegistrationIntegration =
    MarketingBox.Integration.Service.Grpc.Models.Registrations.Contracts.Integration.Registration;
using Moq;
using Moq.AutoMock;
using MyNoSqlServer.Abstractions;
using NUnit.Framework;

namespace MarketingBox.Registration.Service.Tests
{
    [TestFixture]
    public class TrafficEngineServiceTests
    {
        private AutoMocker _autoMocker;
        private TrafficEngineService _engine;
        private readonly Domain.Models.Registrations.Registration _registration = new();
        private CampaignRowMessage _campaignRowNoSql1;
        private CampaignRowMessage _campaignRowNoSql2;
        private CampaignRowMessage _campaignRowNoSql3;
        private FakeMyNoSqlReaderWriter<BrandCandidateNoSql> brandCandidate;
        private const long CampaignId = 1;
        private const long BrandId1 = 1;
        private const long BrandId2 = 2;
        private const long BrandId3 = 3;
        private const int CountryId = 1;
        private const int DailyCapValue = 100;
        private const int Priority = 1;

        private static CampaignRowMessage GetCampaignRowNoSql(long brandId,
            int dailyCap,
            int priority,
            int weight)
        {
            return
                new CampaignRowMessage
                {
                    BrandId = brandId,
                    Priority = priority,
                    Weight = weight,
                    DailyCapValue = dailyCap
                };
        }

        private void CreateCampaignRows(
            int weight1, int weight2, int weight3,
            int dailyCap1 = DailyCapValue, int dailyCap2 = DailyCapValue, int dailyCap3 = DailyCapValue,
            int priority1 = Priority, int priority2 = Priority, int priority3 = Priority)
        {
            _campaignRowNoSql1 = GetCampaignRowNoSql(BrandId1, dailyCap1, priority1, weight1);
            _campaignRowNoSql2 = GetCampaignRowNoSql(BrandId2, dailyCap2, priority2, weight2);
            _campaignRowNoSql3 = GetCampaignRowNoSql(BrandId3, dailyCap3, priority3, weight3);
        }

        private void SetupIntegrationService(ResponseStatus status)
        {
            _autoMocker
                .Setup<IIntegrationService, Task<Response<RegistrationIntegration>>>(
                    x => x.SendRegisterationAsync(It.IsAny<RegistrationRequest>()))
                .ReturnsAsync(() => new Response<RegistrationIntegration>
                {
                    Status = status,
                    Data = new RegistrationIntegration
                    {
                        Customer = new CustomerInfo
                        {
                            Token = nameof(CustomerInfo.Token),
                            CustomerId = nameof(CustomerInfo.CustomerId),
                            LoginUrl = nameof(CustomerInfo.LoginUrl),
                        }
                    }
                });
        }

        private async Task MakeWholeRoutingCycleAndAssert(long campaignId, int countryId)
        {
            // 1st iteration
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(3, _registration.BrandId);

            // 2nd iteration
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(3, _registration.BrandId);

            // 3rd iteration
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);

            // 4th iteration
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);

            // 5th iteration
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);

            // 6th iteration
            Assert.IsTrue(await _engine.TryRegisterAsync(campaignId, countryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);
        }

        [SetUp]
        public void Setup()
        {
            _autoMocker = new AutoMocker();

            _autoMocker.Setup<IRouterFilterService, Task<List<CampaignRowMessage>>>(
                    x => x.GetSuitableRoutes(CampaignId, CountryId))
                .ReturnsAsync(() => new()
                {
                    _campaignRowNoSql1,
                    _campaignRowNoSql2,
                    _campaignRowNoSql3
                });
            _autoMocker.Setup<IMyNoSqlServerDataReader<IntegrationNoSql>, IntegrationNoSql>(
                    x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => new IntegrationNoSql
                {
                    Integration = new IntegrationMessage()
                });
            _autoMocker.Setup<IMyNoSqlServerDataReader<BrandNoSql>, BrandNoSql>(
                    x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => new BrandNoSql()
                {
                    Brand = new BrandMessage()
                });
            _autoMocker.Setup<IMapper, RegistrationRequest>(
                    x => x.Map<RegistrationRequest>(It.IsAny<Domain.Models.Registrations.Registration>()))
                .Returns(() => new RegistrationRequest());


            brandCandidate = new FakeMyNoSqlReaderWriter<BrandCandidateNoSql>();
            _autoMocker.Use<IMyNoSqlServerDataReader<BrandCandidateNoSql>>(brandCandidate);
            _autoMocker.Use<IMyNoSqlServerDataWriter<BrandCandidateNoSql>>(brandCandidate);

            SetupIntegrationService(ResponseStatus.Ok);

            _engine = _autoMocker.CreateInstance<TrafficEngineService>();
        }

        /// <summary>
        /// This test checks routing for the following case:
        /// BrandId | Weight |Iteration 1   ||Iteration 2   ||Iteration 3   ||Iteration 4    ||Iteration 5    ||Iteration 6    |
        /// --------|--------|--------------||--------------||--------------||---------------||---------------||---------------|
        ///    1    |   5    | 1 |   |      || 1 |   |      ||  1  |        ||  1  |         ||     X         ||               |
        ///    2    |   3    |   | 1 |      ||   | 1 |      ||     |  1     ||     |  X      ||               ||     X         |
        ///    3    |   1    |   |   | 1    ||   |   |      ||     |        ||     |         ||               ||               |
        /// </summary>
        [Test]
        public async Task SameEmailByWeightTest()
        {
            CreateCampaignRows(5, 3, 1);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(3, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(2, _registration.BrandId); // same email

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(1, _registration.BrandId); // same email

            SetupIntegrationService(ResponseStatus.BadRequest);
            Assert.IsFalse(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
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
        public async Task IterationsByWeightTest()
        {
            CreateCampaignRows(6, 4, 2);
            await MakeWholeRoutingCycleAndAssert(CampaignId, CountryId);
            await MakeWholeRoutingCycleAndAssert(CampaignId, CountryId);
        }

        /// <summary>
        /// This test checks routing for the following case:
        /// BrandId | Weight | Priority | DailyCap |Iteration 1||Iteration 2||Iteration 3|
        /// --------|--------|----------|----------|-----------||-----------||-----------|
        ///    1    |   1    |   1      |   1      |       1   ||           ||           |
        ///    2    |   1    |   2      |   1      |           ||     1     ||           |
        ///    3    |   1    |   3      |   1      |           ||           ||      1    |
        /// </summary>
        [Test]
        public async Task IterationsByPriorityTest()
        {
            CreateCampaignRows(
                1, 1, 1, // weight
                1, 1, 1, // dailyCap
                1, 2, 3); // Priority
            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(3, _registration.BrandId);
        }

        /// <summary>
        /// This test checks that cache is updated for next day.
        /// </summary>
        [Test]
        public async Task IterationsByPriorityForDifferentDaysTest()
        {
            CreateCampaignRows(
                1, 1, 1, // weight
                1, 1, 1, // dailyCap
                1, 2, 3); // Priority
            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(3, _registration.BrandId);
            
            brandCandidate.Date = DateTime.Today.AddDays(-1).DayOfWeek;
            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(1, _registration.BrandId);
            brandCandidate.Date = DateTime.Today.DayOfWeek;

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(2, _registration.BrandId);

            Assert.IsTrue(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
            Assert.AreEqual(3, _registration.BrandId);
        }

        /// <summary>
        /// This test checks that traffic engine can't register to any brand that does not respond to criteria.
        /// </summary>
        [Test]
        public async Task NoSuitableRoutesTest()
        {
            _autoMocker.Setup<IRouterFilterService, Task<List<CampaignRowMessage>>>(
                    x => x.GetSuitableRoutes(CampaignId, CountryId))
                .ReturnsAsync(new List<CampaignRowMessage>());

            Assert.IsFalse(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
        }

        /// <summary>
        /// This test checks that traffic engine can't register to any brand nosql doesn't contain brand.
        /// </summary>
        [Test]
        public async Task NoBrandFromNoSqlTest()
        {
            _autoMocker.Setup<IMyNoSqlServerDataReader<BrandNoSql>, BrandNoSql>(
                    x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((BrandNoSql) null);

            Assert.IsFalse(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
        }

        /// <summary>
        /// This test checks that traffic engine can't register to any brand nosql doesn't contain integration.
        /// </summary>
        [Test]
        public async Task NoIntegrationFromNoSqlTest()
        {
            _autoMocker.Setup<IMyNoSqlServerDataReader<IntegrationNoSql>, IntegrationNoSql>(
                    x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((IntegrationNoSql) null);

            Assert.IsFalse(await _engine.TryRegisterAsync(CampaignId, CountryId, _registration));
        }
    }
}