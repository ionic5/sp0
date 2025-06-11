using System.Text.Json.Serialization;

namespace Sample.SP0.Client.Core.KiwoomApi
{
    public class DailyCandlestick
    {
        [JsonPropertyName("cur_prc")]
        public string CurPrc { get; set; }

        [JsonPropertyName("trde_qty")]
        public string TrdeQty { get; set; }

        [JsonPropertyName("trde_prica")]
        public string TrdePrica { get; set; }

        [JsonPropertyName("dt")]
        public string Dt { get; set; }

        [JsonPropertyName("open_pric")]
        public string OpenPric { get; set; }

        [JsonPropertyName("high_pric")]
        public string HighPric { get; set; }

        [JsonPropertyName("low_pric")]
        public string LowPric { get; set; }

        [JsonPropertyName("upd_stkpc_tp")]
        public string UpdStkpcTp { get; set; }

        [JsonPropertyName("upd_rt")]
        public string UpdRt { get; set; }

        [JsonPropertyName("bic_inds_tp")]
        public string BicIndsTp { get; set; }

        [JsonPropertyName("sm_inds_tp")]
        public string SmIndsTp { get; set; }

        [JsonPropertyName("stk_infr")]
        public string StkInfr { get; set; }

        [JsonPropertyName("upd_stkpc_event")]
        public string UpdStkpcEvent { get; set; }

        [JsonPropertyName("pred_close_pric")]
        public string PredClosePric { get; set; }

        public DailyCandlestick()
        {
            CurPrc = string.Empty;
            TrdeQty = string.Empty;
            TrdePrica = string.Empty;
            Dt = string.Empty;
            OpenPric = string.Empty;
            HighPric = string.Empty;
            LowPric = string.Empty;
            UpdStkpcTp = string.Empty;
            UpdRt = string.Empty;
            BicIndsTp = string.Empty;
            SmIndsTp = string.Empty;
            StkInfr = string.Empty;
            UpdStkpcEvent = string.Empty;
            PredClosePric = string.Empty;
        }
    }
}
