using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Domain.Models.Attributes;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Registration.Service.Grpc.Requests.Registration;

[DataContract]
public class RegistrationsGetByDateRequest : ValidatableEntity
{
    [DataMember(Order = 1)]
    [Required]
    [Range(1, long.MaxValue)]
    public long? AffiliateId { get; set; }

    [DataMember(Order = 2)]
    [Required]
    public string ApiKey { get; set; }
    
    [DataMember(Order = 3)]
    [Required]
    public DateTime? From { get; set; }

    [DataMember(Order = 4)]
    [Required]
    [DateTimeNotLessThan(nameof(From),nameof(From))]
    public DateTime? To { get; set; }

    [DataMember(Order = 5)]
    [Required]
    public RegistrationsReportType? Type { get; set; }
}