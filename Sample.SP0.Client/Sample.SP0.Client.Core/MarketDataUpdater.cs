using HtmlAgilityPack;
using Sample.SP0.Client.Core.Entities;
using Sample.SP0.Client.Core.KiwoomApi;
using Sample.SP0.Client.Core.View;
using Sample.SP0.Client.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core
{
    public class MarketDataUpdater
    {
        private readonly IMainScene _mainScene;
        private readonly StockItemInfoRepository _stockItemInfoRepository;
        private readonly ILogger _logger;
        private readonly TokenStore _tokenStore;
        private readonly RestApiClient _restApiClient;
        private readonly UrlSet _urlSet;
        private readonly TypeTransformer _typeTransformer;
        private readonly UpdateLogRepository _updateLogRepository;

        public MarketDataUpdater(IMainScene mainScene, StockItemInfoRepository stockItemInfoRepository, ILogger logger,
            TokenStore tokenStore, RestApiClient restApiClient, UrlSet urlSet, TypeTransformer typeTransformer,
            UpdateLogRepository updateLogRepository)
        {
            _mainScene = mainScene;
            _stockItemInfoRepository = stockItemInfoRepository;
            _logger = logger;
            _tokenStore = tokenStore;
            _restApiClient = restApiClient;
            _urlSet = urlSet;
            _typeTransformer = typeTransformer;
            _updateLogRepository = updateLogRepository;
        }

        public async void UpdateMarketData()
        {
            var loadingPanel = _mainScene.ShowLoadingPanel();

            loadingPanel.SetMessage("Update stock item information.");
            await TryUpdateStockItemInfoRepository();
        }

        private async Task TryUpdateStockItemInfoRepository()
        {
            if (_stockItemInfoRepository.IsEmpty() || IsUpdateRequired(RepositoryName.StockItemInfo))
                await UpdateStockItemInfoRepository();
        }

        private async Task UpdateStockItemInfoRepository()
        {
            var stockNameToCodeMap = await GetStockNameToCodeMap();

            string url = "https://stock.richinfohub.com/kospi200info/";

            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            var rows = doc.DocumentNode.SelectNodes("//table//tr");

            var entities = new List<Entities.StockItemInfo>();
            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns != null && columns.Count > 1)
                {
                    var companyName = columns[1].InnerText.Trim();
                    if (!stockNameToCodeMap.ContainsKey(companyName))
                    {
                        _logger.Info($"Failed to find stock code for {companyName}.");
                        continue;
                    }

                    var marketCap = _typeTransformer.ConvertToNumber(columns[4].InnerText.Trim());
                    var entity = new Entities.StockItemInfo();
                    entity.CompanyName = companyName;
                    entity.StockCode = stockNameToCodeMap[companyName];
                    entity.MarketCap = marketCap;
                    entities.Add(entity);
                }
            }

            _stockItemInfoRepository.Update(entities);
            _updateLogRepository.Update(new UpdateLog(RepositoryName.StockItemInfo, DateTime.Now));

            _logger.Info("StockItemInfo updated.");
        }

        private async Task<Dictionary<string, string>> GetStockNameToCodeMap()
        {
            var headers = new Dictionary<string, string>();
            headers["authorization"] = $"{_tokenStore.TokenType} {_tokenStore.Token}";
            headers["api-id"] = "ka10099";
            var requestBody = new
            {
                mrkt_tp = "0"
            };
            var response = await _restApiClient.SendRequestAsync<StockItemInfoListBody>(_urlSet.StockInfo,
                HttpMethod.Post, requestBody, headers);

            var result = new Dictionary<string, string>();
            foreach (var item in response.Body.StockItemInfos)
                result[item.Name] = item.Code;
            return result;
        }

        private bool IsUpdateRequired(RepositoryName repositoryName)
        {
            var entity = _updateLogRepository.Select(repositoryName);
            return entity.LastUpdated.Date < DateTime.Now.Date;
        }
    }
}
