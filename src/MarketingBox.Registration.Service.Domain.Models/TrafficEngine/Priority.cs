using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Domain.Models.TrafficEngine;

[DataContract]
public class Priority
{
    [DataMember(Order = 1)]
    public int PriorityValue { get; set; }
    
    [DataMember(Order = 2)]
    public List<BrandCandidate> Brands { get; set; }
}