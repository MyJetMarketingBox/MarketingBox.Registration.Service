using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Domain.Models.TrafficEngine;

[DataContract]
public class BrandCandidate
{
    [DataMember(Order = 1)] public long BrandId { get; set; }
    [DataMember(Order = 2)] public int CountOfSent { get; set; }
    [DataMember(Order = 3)] public long DailyCapValue { get; set; }
    [DataMember(Order = 4)] public int SentByWeight { get; set; }
    [DataMember(Order = 5)] public int WeightCapValue { get; set; }
    [DataMember(Order = 6)] public bool Marked { get; set; }
    [DataMember(Order = 7)] public bool SuccessfullySent { get; set; }
}