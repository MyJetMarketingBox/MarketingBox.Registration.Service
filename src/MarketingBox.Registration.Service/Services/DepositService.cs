using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Messages.Registrations;
using ErrorType = MarketingBox.Registration.Service.Grpc.Models.Common.ErrorType;
using MarketingBox.Registration.Service.Domain.Registrations;

namespace MarketingBox.Registration.Service.Modules
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

        public async Task<DepositResponse> RegisterDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                registration.Deposit(DateTimeOffset.UtcNow);

                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent deposit register to service bus {@context}", request);

                return MapToGrpc(registration);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating registration {@context}", request);

                return new DepositResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<DepositResponse> ApproveDepositAsync(DepositApproveRequest request)
        {
            _logger.LogInformation("Approving a deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.Approved(DateTimeOffset.UtcNow, request.Mode.MapEnum<Domain.Registrations.RegistrationApprovedType>());
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent deposit approve to service bus {@context}", request);

                return MapToGrpc(registration);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error approving a deposit {@context}", request);

                return new DepositResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static DepositResponse MapToGrpc(Domain.Registrations.Registration registration)
        {
            return new DepositResponse()
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
                    Status = registration.RouteInfo.Status.MapEnum<MarketingBox.Registration.Service.Grpc.Models.Registrations.RegistrationStatus>(),
                    Country = registration.RegistrationInfo.Country,
                    ConversionDate = registration.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = registration.RouteInfo.DepositDate?.UtcDateTime,
                    UpdatedAt = registration.RegistrationInfo.UpdatedAt.UtcDateTime,
                    AffiliateId = registration.RouteInfo.AffiliateId,
                    AffiliateName = registration.RouteInfo.AffiliateName,
                }
                //Id = registration.RegistrationInfo.Id,
                //Message = $"Registration {registration.RegistrationInfo.Id} can be approved as depositor, current status " +
                //          $"{registration.RegistrationInfo.RouteInfoStatus.ToString()} at {registration.RegistrationInfo.RouteInfoDepositDate}",
            };
        }
    }
}
