﻿using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Messages.Registrations;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using ErrorType = MarketingBox.Registration.Service.Grpc.Models.Common.ErrorType;
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

        public async Task<DepositResponse> RegisterDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            try
            {
                var lead = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
                lead.Deposit(DateTimeOffset.UtcNow);

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent deposit register to service bus {@context}", request);

                return MapToGrpc(lead);
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
                var lead = await _repository.GetLeadByRegistrationIdAsync(request.TenantId, request.RegistrationId);
                lead.Approved(DateTimeOffset.UtcNow);

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent deposit approve to service bus {@context}", request);

                return MapToGrpc(lead);
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
                    UniqueId = registration.RegistrationInfo.UniqueId,
                    CrmCrmStatus = registration.RouteInfo.CrmStatus,
                    Status = registration.RouteInfo.Status.MapEnum<RegistrationStatus>(),
                    Country = registration.RegistrationInfo.Country,
                    ConversionDate = registration.RouteInfo.ConversionDate?.UtcDateTime,
                    DepositDate = registration.RouteInfo.DepositDate?.UtcDateTime,
                    UpdatedAt = registration.RegistrationInfo.UpdatedAt.UtcDateTime,
                }
                //Id = registration.RegistrationInfo.Id,
                //Message = $"Registration {registration.RegistrationInfo.Id} can be approved as depositor, current status " +
                //          $"{registration.RegistrationInfo.RouteInfoStatus.ToString()} at {registration.RegistrationInfo.RouteInfoDepositDate}",
            };
        }
    }
}
