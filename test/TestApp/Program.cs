using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Client;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var factory = new RegistrationServiceClientFactory("http://localhost:12121");
            var leadService = factory.GetRegistrationService();
            var depositService = factory.GetDepositService();
            //var testTenant = "default-tenant-id";
            var logger = LoggerFactory.Create((x) => x.AddConsole()).CreateLogger<Program>();

            for (int i = 0; i < 12; i++)
            {
                var lead = await leadService.CreateAsync(new RegistrationCreateRequest()
                {
                    AdditionalInfo = new RegistrationAdditionalInfo()
                    {
                    },
                    AuthInfo = new AffiliateAuthInfo()
                    {
                        CampaignId = 1,
                        AffiliateId = 1,
                        ApiKey = "APIKEY123456"
                    },
                    GeneralInfo = new RegistrationGeneralInfo()
                    {
                        Country = "UA",
                        Email = $"test.testov.2020.11.08.{i}@mailinator.com",
                        FirstName = "Test",
                        Ip = "99.99.99.99",
                        LastName = "Testov",
                        Password = "Test123456!",
                        Phone = "+79995556677"
                    }
                });

                logger.LogInformation($"{Newtonsoft.Json.JsonConvert.SerializeObject(lead)}");
            }

            //var deposit = await depositService.RegisterDepositAsync(
            //    new DepositCreateRequest()
            //    {
            //        Email = "email@email.com",
            //        BrandId = 23,
            //        CreatedAt = DateTime.UtcNow,
            //        CustomerId = "CUSTOMER-1234",
            //        BrandName = "Monfex",
            //        TenantId = testTenant
            //    }
            //);
            //Console.WriteLine(leadCreated.Id);

            //var partnerUpdated = (await client.UpdateAsync(new RegistrationUpdateRequest()
            //{
            //    Id = leadCreated.Id,
            //    TenantId = leadCreated.TenantId,
            //    NoSqlInfo = request.NoSqlInfo,
            //    Sequence = 1
            //})).Registration;

            //await client.DeleteAsync(new RegistrationDeleteRequest()
            //{
            //    Id = partnerUpdated.Id,
            //});

            //var shouldBeNull =await client.GetAsync(new RegistrationGetRequest()
            //{
            //    Id = partnerUpdated.Id,
            //});

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
