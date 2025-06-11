using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core.KiwoomApi
{
    public class StockItemInfo
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("listCount")]
        public string ListCount { get; set; }

        [JsonPropertyName("auditInfo")]
        public string AuditInfo { get; set; }

        [JsonPropertyName("regDay")]
        public string RegDay { get; set; }

        [JsonPropertyName("lastPrice")]
        public string LastPrice { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("marketCode")]
        public string MarketCode { get; set; }

        [JsonPropertyName("marketName")]
        public string MarketName { get; set; }

        [JsonPropertyName("upName")]
        public string UpName { get; set; }

        [JsonPropertyName("upSizeName")]
        public string UpSizeName { get; set; }

        [JsonPropertyName("companyClassName")]
        public string CompanyClassName { get; set; }

        [JsonPropertyName("orderWarning")]
        public string OrderWarning { get; set; }

        [JsonPropertyName("nxtEnable")]
        public string NxtEnable { get; set; }

        public StockItemInfo()
        {
            Code = string.Empty;
            Name = string.Empty;
            ListCount = string.Empty;
            AuditInfo = string.Empty;
            RegDay = string.Empty;
            LastPrice = string.Empty;
            State = string.Empty;
            MarketCode = string.Empty;
            MarketName = string.Empty;
            UpName = string.Empty;
            UpSizeName = string.Empty;
            CompanyClassName = string.Empty;
            OrderWarning = string.Empty;
            NxtEnable = string.Empty;
        }
    }
}
