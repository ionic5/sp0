using System.Text.Json.Serialization;

namespace Sample.SP0.Client.Core.KiwoomApi
{
    public class DailyCandlestickBody : ResponseBody
    {
        [JsonPropertyName("stk_cd")]
        public string StkCd { get; set; }

        [JsonPropertyName("stk_dt_pole_chart_qry")]
        public List<DailyCandlestick> StkDtPoleChartQry { get; set; }

        public DailyCandlestickBody()
        {
            StkCd = string.Empty;
            StkDtPoleChartQry = new List<DailyCandlestick>();
        }
    }
}
