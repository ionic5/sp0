using Sample.SP0.Client.Core.KiwoomApi;
using Sample.SP0.Client.Core.View;
using Sample.SP0.Client.Core.WebApi;
using System.Globalization;

namespace Sample.SP0.Client.Core
{
    public class StartPanelController
    {
        private readonly IStartPanel _startPanel;
        private readonly RestApiClient _restApiClient;
        private readonly ILogger _logger;
        private readonly TokenStore _tokenStore;
        private readonly UrlSet _urlSet;
        private readonly MarketDataUpdater _marketDataUpdater;
        private readonly IMainScene _mainScene;
        private readonly MainPanelControllerFactory _mainPanelControllerFactory;

        public StartPanelController(IStartPanel startPanel, RestApiClient restApiClient, ILogger logger, TokenStore tokenStore, UrlSet urlSet, MarketDataUpdater marketDataUpdater, IMainScene mainScene, MainPanelControllerFactory mainPanelControllerFactory)
        {
            this._startPanel = startPanel;
            _restApiClient = restApiClient;
            _logger = logger;
            _tokenStore = tokenStore;
            _urlSet = urlSet;
            _marketDataUpdater = marketDataUpdater;
            _mainScene = mainScene;
            _mainPanelControllerFactory = mainPanelControllerFactory;
        }

        public async void OnStartButtonClickedEvent(object? sender, EventArgs args)
        {
            var appKey = _startPanel.GetAppKey();
            var secretKey = _startPanel.GetSecretKey();

            var requestBody = new
            {
                grant_type = "client_credentials",
                appkey = appKey,
                secretkey = secretKey
            };
            var response = await _restApiClient.SendRequestAsync<TokenIssuanceBody>(_urlSet.TokenIssuance,
                HttpMethod.Post, requestBody);

            if (response.Body == null || response.Body.ReturnCode != 0)
            {
                _logger.Warn("Failed to get token.");
                return;
            }

            SaveToken(response.Body);

            _marketDataUpdater.UpdateMarketData();
        }

        public void OnStartOfflineButtonClickedEvent(object? sender, EventArgs args)
        {
            var mainPanel = _mainScene.ShowMainPanel();

            var ctrl = _mainPanelControllerFactory.Create(mainPanel);
            ctrl.Setup();
        }

        private void SaveToken(TokenIssuanceBody body)
        {
            _tokenStore.Token = body.Token;
            _tokenStore.TokenType = body.TokenType;
            DateTime.TryParseExact(body.ExpiresDt, "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _tokenStore.ExpireDateTime);
        }

    }
}
