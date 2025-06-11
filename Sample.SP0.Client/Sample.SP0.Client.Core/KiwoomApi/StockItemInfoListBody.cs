using System.Text.Json.Serialization;

namespace Sample.SP0.Client.Core.KiwoomApi
{
    public class StockItemInfoListBody
    {
        [JsonPropertyName("list")]
        public List<StockItemInfo> StockItemInfos { get; set; }

        public StockItemInfoListBody()
        {
            StockItemInfos = new List<StockItemInfo>();
        }
    }
}
