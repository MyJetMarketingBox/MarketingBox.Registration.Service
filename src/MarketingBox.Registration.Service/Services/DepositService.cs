using System;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Models.Registrations.Deposit;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Deposits;
using MarketingBox.Registration.Service.Messages.Registrations;
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
                
                var registration = await _registrationRepository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                UpdateStatus(registration, UpdateMode.Automatically, RegistrationStatus.Deposited);

                await _registrationRepository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(_mapper.Map<RegistrationUpdateMessage>(registration));
                _logger.LogInformation("Sent deposit register to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = _mapper.Map<Deposit>(registration) //MapToGrpc(registration)
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
                _logger.LogInformation("UpdateDepositStatusAsync receive request : {requestJson}", 
                    JsonConvert.SerializeObject(request));
                
                request.ValidateEntity();
                
                var registration =
                    await _registrationRepository.GetRegistrationByIdAsync(request.TenantId, request.RegistrationId);

                var oldStatus = registration.Status;

                UpdateStatus(registration, request.Mode, request.NewStatus);
                
                await _registrationRepository.SaveAsync(registration);
                
                await _registrationRepository.SaveStatusChangeLogAsync(new StatusChangeLog()
                {
                    Date = DateTime.UtcNow,
                    UserId = request.UserId,
                    RegistrationId = request.RegistrationId,
                    Mode = request.Mode,
                    Comment = request.Comment,
                    OldStatus = oldStatus,
                    NewStatus = request.NewStatus
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

        public async Task<Response<StatusChangeLog>> GetStatusChangeLogAsync(GetStatusChangeLogRequest request)
        {
            try
            {
                _logger.LogInformation("GetStatusChangeLogAsync receive request : {requestJson}", 
                    JsonConvert.SerializeObject(request));

                var logs = await _registrationRepository.GetStatusChangeLogAsync(request);
                
                return new Response<StatusChangeLog>
                {
                    Status = ResponseStatus.Ok,
                    Data = _mapper.Map<StatusChangeLog>(logs)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting status change log {@context}", request);
                return e.FailedResponse<StatusChangeLog>();
            }
        }

        private static void UpdateStatus(
            RegistrationEntity registrationEntity,
            UpdateMode type,
            RegistrationStatus newStatus)
        {
            switch (newStatus)
            {
                case RegistrationStatus.Created:
                    break;
                case RegistrationStatus.Registered:
                    registrationEntity.Status = RegistrationStatus.Registered;
                    registrationEntity.ApprovedType = type;
                    registrationEntity.ConversionDate = null;
                    registrationEntity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
                case RegistrationStatus.Deposited:
                    registrationEntity.Status = RegistrationStatus.Deposited;
                    registrationEntity.DepositDate = DateTimeOffset.UtcNow;
                    registrationEntity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
                case RegistrationStatus.Approved:
                    registrationEntity.Status = RegistrationStatus.Approved;
                    registrationEntity.ApprovedType = type;
                    registrationEntity.ConversionDate = DateTimeOffset.UtcNow;
                    registrationEntity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
                case RegistrationStatus.Declined:
                    registrationEntity.Status = RegistrationStatus.Declined;
                    registrationEntity.ApprovedType = type;
                    registrationEntity.ConversionDate = null;
                    registrationEntity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
            }
        }
    }
}