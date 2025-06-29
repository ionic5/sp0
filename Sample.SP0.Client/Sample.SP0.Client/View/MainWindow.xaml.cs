﻿using Sample.SP0.Client.Core;
using Sample.SP0.Client.Core.KiwoomApi;
using Sample.SP0.Client.Core.View;
using Sample.SP0.Client.Core.WebApi;
using Sample.SP0.Client.View;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sample.SP0.Client.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainScene
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        public ILoadingPanel ShowLoadingPanel()
        {
            LoadingPanel.Visibility = Visibility.Visible;
            StartPanel.Visibility = Visibility.Hidden;
            MainPanel.Visibility = Visibility.Hidden;

            return LoadingPanel;
        }

        public IMainPanel ShowMainPanel()
        {
            LoadingPanel.Visibility = Visibility.Hidden;
            StartPanel.Visibility = Visibility.Hidden;
            MainPanel.Visibility = Visibility.Visible;

            return MainPanel;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var logger = new Logger(DebugConsolePanel);

            var urlSet = new UrlSet("https://api.kiwoom.com");

            var restApiClient = new RestApiClient(logger);
            var tokenStore = new TokenStore();

            var typeTransformer = new TypeTransformer();

            var connStr = "Server=127.0.0.1;Database=stock_db;User ID=cat;Password=@rat108;SslMode=None;ConnectionLifeTime=300;";
            var updateLogRepo = new UpdateLogRepository(connStr, logger, typeTransformer);
            var stockItemInfoRepo = new StockItemInfoRepository(connStr, logger);

            var dailyCandlestickRepo = new DailyCandleStickRepository(new BasicRepository(connStr, logger));
            var maRepo = new MaPointRepository(new BasicRepository(connStr, logger));
            var mfiRepo = new MfiPointRepository(new BasicRepository(connStr, logger));
            var macdPtRepo = new MacdPointRepository(new BasicRepository(connStr, logger));
            var bbPtRepo = new BBPointRepository(new BasicRepository(connStr, logger));
            var rsiPtRepo = new RsiPointRepository(new BasicRepository(connStr, logger));
            var obvPtRepo = new ObvPointRepository(new BasicRepository(connStr, logger));

            var mainPnlCtrlFac = new MainPanelControllerFactory(dailyCandlestickRepo,
                maRepo, bbPtRepo, rsiPtRepo, mfiRepo, obvPtRepo, macdPtRepo, stockItemInfoRepo);

            var marketDataUpdater = new MarketDataUpdater(this, stockItemInfoRepo,
                logger, tokenStore, restApiClient, urlSet, typeTransformer, updateLogRepo,
                dailyCandlestickRepo, maRepo, mfiRepo, macdPtRepo, bbPtRepo, rsiPtRepo,
                obvPtRepo, mainPnlCtrlFac);

            var ctrl = new StartPanelController(StartPanel, restApiClient, logger, tokenStore,
                urlSet, marketDataUpdater, this, mainPnlCtrlFac);
            StartPanel.StartButtonClickedEvent += ctrl.OnStartButtonClickedEvent;
            StartPanel.StartOfflineButtonClickedEvent += ctrl.OnStartOfflineButtonClickedEvent;
        }
    }
}