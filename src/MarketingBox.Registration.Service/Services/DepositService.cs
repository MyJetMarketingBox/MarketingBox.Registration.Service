using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Registrations;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using RegistrationStatus = MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationStatus;

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
                registration.UpdateStatus(DepositUpdateMode.Automatically, Domain.Registrations.RegistrationStatus.Deposited);

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

        private static Deposit MapToGrpc(Domain.Registrations.Registration registration)
        {
            return new Deposit()
            {
                TenantId = registration.TenantId,
                GeneralInfo = new DepositGeneralInfo()
                {
                    Email = registration.RegistrationInfo.Email,
                    FirstName = registration.RegistrationInfo.FirstName,
                    LastName = registration.RegistrationInfo.LastName,
                    Phone = registration.RegistrationInfo.Phone,
                    Ip = registration.RegistrationInfo.Ip,
                    Password = registration.RegistrationInfo.Password,
                    CreatedAt = registration.RegistrationInfo.CreatedAt.UtcDateTime,
                    RegistrationId = registration.RegistrationInfo.RegistrationId,
                    UniqueId = registration.RegistrationInfo.RegistrationUid,
                    CrmStatus = registration.RouteInfo.CrmStatus,
                    Status = registration.RouteInfo.Status.MapEnum<RegistrationStatus>(),
                    CountryId = registration.RegistrationInfo.CountryId,
                    ConversionDate = registration.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = registration.RouteInfo.DepositDate?.UtcDateTime,
                    UpdatedAt = registration.RegistrationInfo.UpdatedAt.UtcDateTime,
                    AffiliateId = registration.RouteInfo.AffiliateId,
                    AffiliateName = registration.RouteInfo.AffiliateName,
                }
            };
        }
    }
}
