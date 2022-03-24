using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Models;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Deposit;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Deposits;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Registration.Service.Services
{
    public class DepositService : IDepositService
    {
        private readonly ILogger<DepositService> _logger;

        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IRegistrationRepository _repository;

        public DepositService(ILogger<DepositService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated, 
            IRegistrationRepository repository)
        {
            _logger = logger;
            _publisherLeadUpdated = publisherLeadUpdated;
            _repository = repository;
        }

        public async Task<Response<Deposit>> RegisterDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                registration.UpdateStatus(DepositUpdateMode.Automatically, RegistrationStatus.Deposited);

                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent deposit register to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating registration {@context}", request);
                return e.FailedResponse<Deposit>();
            }
        }

        public async Task<Response<Deposit>> UpdateDepositStatusAsync(UpdateDepositStatusRequest request)
        {
            _logger.LogInformation("Approving a deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.UpdateStatus(request.Mode, request.NewStatus);
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent deposit approve to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error approving a deposit {@context}", request);

                return e.FailedResponse<Deposit>();
            }
        }

        private static Deposit MapToGrpc(Domain.Models.Registrations.Registration_nogrpc registrationNogrpc)
        {
            return new Deposit()
            {
                TenantId = registrationNogrpc.TenantId,
                GeneralInfo = new DepositGeneralInfo()
                {
                    Email = registrationNogrpc.RegistrationInfoNotgrpc.Email,
                    FirstName = registrationNogrpc.RegistrationInfoNotgrpc.FirstName,
                    LastName = registrationNogrpc.RegistrationInfoNotgrpc.LastName,
                    Phone = registrationNogrpc.RegistrationInfoNotgrpc.Phone,
                    Ip = registrationNogrpc.RegistrationInfoNotgrpc.Ip,
                    Password = registrationNogrpc.RegistrationInfoNotgrpc.Password,
                    CreatedAt = registrationNogrpc.RegistrationInfoNotgrpc.CreatedAt.UtcDateTime,
                    RegistrationId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationId,
                    UniqueId = registrationNogrpc.RegistrationInfoNotgrpc.RegistrationUid,
                    CrmStatus = registrationNogrpc.RouteInfo.CrmStatus,
                    Status = registrationNogrpc.RouteInfo.Status.MapEnum<RegistrationStatus>(),
                    CountryId = registrationNogrpc.RegistrationInfoNotgrpc.CountryId,
                    ConversionDate = registrationNogrpc.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = registrationNogrpc.RouteInfo.DepositDate?.UtcDateTime,
                    UpdatedAt = registrationNogrpc.RegistrationInfoNotgrpc.UpdatedAt.UtcDateTime,
                    AffiliateId = registrationNogrpc.RouteInfo.AffiliateId,
                    AffiliateName = registrationNogrpc.RouteInfo.AffiliateName,
                }
            };
        }
    }
}
