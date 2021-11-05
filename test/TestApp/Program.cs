using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Client;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
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

            var factory = new RegistrationServiceClientFactory("http://localhost:90");
            var leadService = factory.GetRegistrationService();
            var depositService = factory.GetDepositService();
            var testTenant = "Test-Tenant";
            var lead = await leadService.CreateAsync(new RegistrationCreateRequest()
            {
                
            });



            var deposit = await depositService.RegisterDepositAsync(
                new DepositCreateRequest()
                {
                    Email = "email@email.com",
                    BrandId = 23,
                    CreatedAt = DateTime.UtcNow,
                    CustomerId = "CUSTOMER-1234",
                    BrandName = "Monfex",
                    TenantId = testTenant
                }
            );
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
