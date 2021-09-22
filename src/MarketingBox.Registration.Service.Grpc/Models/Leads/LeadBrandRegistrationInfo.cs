﻿using System.Runtime.Serialization;
using Destructurama.Attributed;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    //"data": {
    //    "accountName": null,
    //    "accountPassword": null,
    //    "brokerName": null,
    //    "brokerUrl": null,
    //    "balance": 0,
    //    "country": null,
    //    "countryCode": null,
    //    "currency": "USD",
    //    "customerId": "PLAYER-be3d12f5-66fb-4ad5-8aa1-70416a91f0fd",
    //    "email": "yuriy.test.2@mailinator.com",
    //    "firstDepositDate": null,
    //    "firstName": "xxx",
    //    "lastName": "xxx",
    //    "token": null,
    //    "uniqueid": "4a8615a7e29881eb66e4871a6dfbadb2",
    //    "loginURLIsForm": null,
    //    "loginURL": "https://simpleway.one/authorization/#/eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJ5dXJpeS50ZXN0LjJAbWFpbGluYXRvci5jb20iLCJyb2xlIjoiUExBWUVSIiwic2Vzc2lvbiI6IjcwYjljOTYzLTcxNDQtNDQ5Ni05OGI4LTA4ZjVkY2Y1MDQ0NCIsImJyYW5kSWQiOiJzaW1wbGV3YXkiLCJpc3MiOiJPdmVyb25peCBBdXRoIiwiZGVwYXJ0bWVudCI6IlBMQVlFUiIsImV4cCI6MTYzMTYxMTYzNiwidXVpZCI6IlBMQVlFUi1iZTNkMTJmNS02NmZiLTRhZDUtOGFhMS03MDQxNmE5MWYwZmQiLCJpYXQiOjE2MzE2MTE1NzZ9.S-l5yKW55W_h-WrtZFhTenvTU7jsF6ZpMc4FKhCLhJBf_urtCBJXab1CURkQVdj5id5inXZaYlzXM-aAb7_FJg?locale=en",
    //    "broker": "Simpleway",
    //    "id": "PLAYER-be3d12f5-66fb-4ad5-8aa1-70416a91f0fd"
    //}    
    [DataContract]
    public class LeadBrandRegistrationInfo
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public string UniqueId { get; set; }

        [DataMember(Order = 5)]
        public string LoginUrl { get; set; }

        [DataMember(Order = 6)]
        public string Broker { get; set; }
    }
}