using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Deposits;
using MarketingBox.Registration.Service.Messages.Registrations;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Newtonsoft.Json;

namespace MarketingBox.Registration.Service.Services
{
    public class DepositService : IDepositService
    {
        private readonly ILogger<DepositService> _logger;
        private readonly IServiceBusPublisher<RegistrationUpdateMessage> _publisherLeadUpdated;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMapper _mapper;

        public DepositService(ILogger<DepositService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated,
            IRegistrationRepository registrationRepository, IMapper mapper)
        {
            _logger = logger;
            _publisherLeadUpdated = publisherLeadUpdated;
            _registrationRepository = registrationRepository;
            _mapper = mapper;
        }

        public async Task<Response<Deposit>> RegisterDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            try
            {
                request.ValidateEntity();

                var registration =
                    await _registrationRepository.GetRegistrationByIdAsync(request.TenantId,
                        request.RegistrationId.Value);
                UpdateStatus(registration, DepositUpdateMode.Automatically, RegistrationStatus.Deposited);

                await _registrationRepository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(_mapper.Map<RegistrationUpdateMessage>(registration));
                _logger.LogInformation("Sent deposit register to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = _mapper.Map<Deposit>(registration)
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
            try
            {
                _logger.LogInformation("UpdateDepositStatusAsync receive request : {@Request}", request);

                request.ValidateEntity();

                var registration =
                    await _registrationRepository.GetRegistrationByIdAsync(request.TenantId,
                        request.RegistrationId.Value);

                var oldStatus = registration.Status;
                if (request.NewStatus.Value == RegistrationStatus.Failed ||
                    oldStatus == request.NewStatus)
                    return new Response<Deposit>
                    {
                        Status = ResponseStatus.Ok,
                        Data = _mapper.Map<Deposit>(registration)
                    };

                UpdateStatus(registration, request.Mode.Value, request.NewStatus.Value);

                await _registrationRepository.SaveAsync(registration);

                await _registrationRepository.SaveStatusChangeLogAsync(new StatusChangeLog()
                {
                    Date = DateTime.UtcNow,
                    UserId = request.UserId.Value,
                    RegistrationId = request.RegistrationId.Value,
                    Mode = request.Mode.Value,
                    Comment = request.Comment,
                    OldStatus = oldStatus,
                    NewStatus = request.NewStatus.Value
                });

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = _mapper.Map<Deposit>(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating deposit status {@context}", request);
                return e.FailedResponse<Deposit>();
            }
        }

        public async Task<Response<List<StatusChangeLog>>> GetStatusChangeLogAsync(GetStatusChangeLogRequest request)
        {
            try
            {
                _logger.LogInformation("GetStatusChangeLogAsync receive request : {requestJson}",
                    JsonConvert.SerializeObject(request));

                var logs = await _registrationRepository.GetStatusChangeLogAsync(request);

                return new Response<List<StatusChangeLog>>
                {
                    Status = ResponseStatus.Ok,
                    Data = logs
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting status change log {@context}", request);
                return e.FailedResponse<List<StatusChangeLog>>();
            }
        }

        private static void UpdateStatus(
            Domain.Models.Registrations.Registration registration,
            DepositUpdateMode type,
            RegistrationStatus newStatus)
        {
            switch (newStatus)
            {
                case RegistrationStatus.Failed:
                    break;
                case RegistrationStatus.Registered:
                    registration.Status = RegistrationStatus.Registered;
                    registration.ApprovedType = type;
                    registration.ConversionDate = null;
                    registration.UpdatedAt = DateTime.UtcNow;
                    break;
                case RegistrationStatus.Deposited:
                    registration.Status = RegistrationStatus.Deposited;
                    registration.DepositDate = DateTime.UtcNow;
                    registration.UpdatedAt = DateTime.UtcNow;
                    break;
                case RegistrationStatus.Approved:
                    registration.Status = RegistrationStatus.Approved;
                    registration.ApprovedType = type;
                    registration.ConversionDate = DateTime.UtcNow;
                    registration.UpdatedAt = DateTime.UtcNow;
                    break;
                case RegistrationStatus.Declined:
                    registration.Status = RegistrationStatus.Declined;
                    registration.ApprovedType = type;
                    registration.ConversionDate = null;
                    registration.UpdatedAt = DateTime.UtcNow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
            }
        }
    }
}