using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Extensions;
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
                registration.Deposit(DateTimeOffset.UtcNow);

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

        public async Task<Response<Deposit>> ApproveDepositAsync(DepositApproveRequest request)
        {
            _logger.LogInformation("Approving a deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.Approve(DateTimeOffset.UtcNow, request.Mode.MapEnum<Domain.Registrations.DepositUpdateMode>());
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


        public async Task<Response<Deposit>> DeclineDepositAsync(DepositUpdateRequest request)
        {
            _logger.LogInformation("Declining a deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.Decline(request.Mode.MapEnum<Domain.Registrations.DepositUpdateMode>());
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent deposit decline to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error declining a deposit {@context}", request);

                return e.FailedResponse<Deposit>();
            }
        }

        public async Task<Response<Deposit>> ApproveDeclinedDepositAsync(DepositUpdateRequest request)
        {
            _logger.LogInformation("Approving a declined deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.ApproveDeclined(DateTimeOffset.UtcNow);
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent approving declined to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error approving a declined deposit {@context}", request);

                return e.FailedResponse<Deposit>();
            }
        }

        public async Task<Response<Deposit>> DeclineApprovedDepositAsync(DepositUpdateRequest request)
        {
            _logger.LogInformation("Declining an approved deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.DeclineApproved();
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent declining approved to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error declining an approved deposit {@context}", request);

                return e.FailedResponse<Deposit>();
            }
        }

        public async Task<Response<Deposit>> ApproveRegisteredDepositAsync(DepositUpdateRequest request)
        {
            _logger.LogInformation("Approving a registered deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.ApproveRegistered(DateTimeOffset.UtcNow);
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent approving registered to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error approving a registered deposit {@context}", request);

                return e.FailedResponse<Deposit>();
            }
        }

        public async Task<Response<Deposit>> RegisterApprovedDepositAsync(DepositUpdateRequest request)
        {
            _logger.LogInformation("Registering an approved deposit {@context}", request);
            try
            {
                var registration = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                registration.RegisterApproved();
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(registration.MapToMessage());
                _logger.LogInformation("Sent registering approved to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error registering an approved deposit {@context}", request);

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
                    Country = registration.RegistrationInfo.Country,
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
