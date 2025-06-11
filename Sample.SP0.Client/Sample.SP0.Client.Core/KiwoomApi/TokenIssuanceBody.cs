using System.Text.Json.Serialization;

namespace Sample.SP0.Client.Core.KiwoomApi
{
    public class TokenIssuanceBody : ResponseBody
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_dt")]
        public string ExpiresDt { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        public TokenIssuanceBody()
        {
            TokenType = string.Empty;
            ExpiresDt = string.Empty;
            Token = string.Empty;
        }
    }
}
