using System.Text.Json.Serialization;

namespace Sample.SP0.Client.Core.KiwoomApi
{
    public class ResponseBody
    {
        [JsonPropertyName("return_code")]
        public int ReturnCode { get; set; }

        [JsonPropertyName("return_msg")]
        public string ReturnMsg { get; set; }

        public ResponseBody()
        {
            ReturnMsg = string.Empty;
        }
    }
}
