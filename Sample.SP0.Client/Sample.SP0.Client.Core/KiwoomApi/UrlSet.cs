namespace Sample.SP0.Client.Core.KiwoomApi
{
    public class UrlSet
    {
        private readonly string _baseUrl;

        public UrlSet(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string TokenIssuance => $"{_baseUrl}/oauth2/token";
        public string StockInfo => $"{_baseUrl}/api/dostk/stkinfo";
        public string Chart => $"{_baseUrl}/api/dostk/chart";
    }
}
