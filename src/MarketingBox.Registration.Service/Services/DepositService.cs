using System;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Registration.Service.Domain.Models.Common;
using MarketingBox.Registration.Service.Domain.Models.Deposit;
using MarketingBox.Registration.Service.Domain.Models.Entities.Registration;
using MarketingBox.Registration.Service.Domain.Repositories;
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
        private readonly IMapper _mapper;

        public DepositService(ILogger<DepositService> logger,
            IServiceBusPublisher<RegistrationUpdateMessage> publisherLeadUpdated,
            IRegistrationRepository repository, IMapper mapper)
        {
            _logger = logger;
            _publisherLeadUpdated = publisherLeadUpdated;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<Deposit>> RegisterDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            try
            {
                request.ValidateEntity();
                
                var registration = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                UpdateStatus(registration, DepositUpdateMode.Automatically, RegistrationStatus.Deposited);

                await _repository.SaveAsync(registration);

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
            _logger.LogInformation("Approving a deposit {@context}", request);
            try
            {
                request.ValidateEntity();
                
                var registration =
                    await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId.Value);
                UpdateStatus(registration, request.Mode.Value, request.NewStatus.Value);
                await _repository.SaveAsync(registration);

                await _publisherLeadUpdated.PublishAsync(_mapper.Map<RegistrationUpdateMessage>(registration));
                _logger.LogInformation("Sent deposit approve to service bus {@context}", request);

                return new Response<Deposit>
                {
                    Status = ResponseStatus.Ok,
                    Data = _mapper.Map<Deposit>(registration) //MapToGrpc(registration)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error approving a deposit {@context}", request);

                return e.FailedResponse<Deposit>();
            }
        }

        private static void UpdateStatus(
            RegistrationEntity registrationEntity,
            DepositUpdateMode type,
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