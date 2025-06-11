using Sample.SP0.Client.Core.View;

namespace Sample.SP0.Client.Core
{
    public class MainPanelController
    {
        private readonly IMainPanel _mainPanel;

        private readonly DailyCandleStickRepository _dailyCandleStickRepository;
        private readonly MaPointRepository _maPointRepository;
        private readonly BBPointRepository _bbPointRepository;
        private readonly RsiPointRepository _rsiPointRepository;
        private readonly MfiPointRepository _mfiPointRepository;
        private readonly ObvPointRepository _obvPointRepository;
        private readonly MacdPointRepository _macdPointRepository;
        private readonly StockItemInfoRepository _stockItemInfoRepository;

        private string _stockCode;

        private readonly Dictionary<OverlayChartType, Action> _drawOverlayChartActions;
        private readonly Dictionary<SubChartType, Action> _drawSubChartActions;

        public MainPanelController(IMainPanel mainPanel, DailyCandleStickRepository dailyCandleStickRepository, MaPointRepository maPointRepository, BBPointRepository bbPointRepository, RsiPointRepository rsiPointRepository, MfiPointRepository mfiPointRepository, ObvPointRepository obvPointRepository, MacdPointRepository macdPointRepository, StockItemInfoRepository stockItemInfoRepository)
        {
            _mainPanel = mainPanel;
            _dailyCandleStickRepository = dailyCandleStickRepository;
            _maPointRepository = maPointRepository;
            _bbPointRepository = bbPointRepository;
            _rsiPointRepository = rsiPointRepository;
            _mfiPointRepository = mfiPointRepository;
            _obvPointRepository = obvPointRepository;
            _macdPointRepository = macdPointRepository;
            _stockItemInfoRepository = stockItemInfoRepository;
            _stockCode = string.Empty;

            _drawOverlayChartActions = new Dictionary<OverlayChartType, Action>();
            _drawOverlayChartActions[OverlayChartType.BB] = DrawBBChart;
            _drawOverlayChartActions[OverlayChartType.MA] = DrawMAChart;

            _drawSubChartActions = new Dictionary<SubChartType, Action>();
            _drawSubChartActions[SubChartType.Volume] = DrawVolumeChart;
            _drawSubChartActions[SubChartType.Mfi] = DrawMfiChart;
            _drawSubChartActions[SubChartType.Macd] = DrawMacdChart;
            _drawSubChartActions[SubChartType.Rsi] = DrawRsiChart;
            _drawSubChartActions[SubChartType.Obv] = DrawObvChart;
        }

        public void Setup()
        {
            _mainPanel.MACheckBoxChangedEvent += OnMACheckBoxChangedEvent;
            _mainPanel.BBCheckBoxChangedEvent += OnBBCheckBoxChangedEvent;
            _mainPanel.VolumeRadioButtonCheckedEvent += OnVolumeRadioButtonCheckedEvent;
            _mainPanel.RsiRadioButtonCheckedEvent += OnRsiRadioButtonCheckedEvent;
            _mainPanel.MfiRadioButtonCheckedEvent += OnMfiRadioButtonCheckedEvent;
            _mainPanel.ObvRadioButtonCheckedEvent += OnObvRadioButtonCheckedEvent;
            _mainPanel.MacdRadioButtonCheckedEvent += OnMacdRadioButtonCheckedEvent;
            _mainPanel.StockItemSelectedEvent += OnStockItemSelectedEvent;

            var items = _stockItemInfoRepository.SelectTop100ByMarketCap();
            _mainPanel.SetStockItmes(items);
            _mainPanel.SelectFirstItem();
        }

        public void OnMACheckBoxChangedEvent(object? sender, CheckBoxChangedEventArgs eventArgs)
        {
            if (eventArgs.IsEnabled)
                DrawMAChart();
            else
                _mainPanel.ClearMAChart();
        }

        public void OnBBCheckBoxChangedEvent(object? sender, CheckBoxChangedEventArgs eventArgs)
        {
            if (eventArgs.IsEnabled)
                DrawBBChart();
            else
                _mainPanel.ClearBBChart();
        }


        public void OnVolumeRadioButtonCheckedEvent(object? sender, EventArgs eventArgs)
        {
            DrawVolumeChart();
        }

        public void OnRsiRadioButtonCheckedEvent(object? sender, EventArgs eventArgs)
        {
            DrawRsiChart();
        }

        public void OnMfiRadioButtonCheckedEvent(object? sender, EventArgs eventArgs)
        {
            DrawMfiChart();
        }

        public void OnMacdRadioButtonCheckedEvent(object? sender, EventArgs eventArgs)
        {
            DrawMacdChart();
        }

        public void OnObvRadioButtonCheckedEvent(object? sender, EventArgs eventArgs)
        {
            DrawObvChart();
        }

        public void OnStockItemSelectedEvent(object? sender, ListItemChangedEventArgs eventArgs)
        {
            var index = eventArgs.SelectedIndex;

            var items = _stockItemInfoRepository.SelectTop100ByMarketCap();
            var item = items.ElementAt(index);
            _stockCode = item.StockCode;

            _mainPanel.ClearCharts();

            var candleSticks = _dailyCandleStickRepository.SelectAll(_stockCode);
            _mainPanel.DrawCandlestickChart(candleSticks);

            TryDrawOverlayCharts();
            TryDrawSubChart();
        }

        private void TryDrawSubChart()
        {
            var chartType = _mainPanel.GetEnabledSubChartType();

            if (chartType == SubChartType.Void)
            {
                _mainPanel.SelectVolumeRadioButton();
                return;
            }

            if (_drawSubChartActions.TryGetValue(chartType, out Action? action))
                action.Invoke();
        }

        private void TryDrawOverlayCharts()
        {
            foreach (OverlayChartType chartType in Enum.GetValues(typeof(OverlayChartType)))
                if (_mainPanel.IsOverlayChartEnabled(chartType) && _drawOverlayChartActions.TryGetValue(chartType, out Action? action))
                    action.Invoke();
        }

        private void DrawVolumeChart()
        {
            var points = _dailyCandleStickRepository.SelectAll(_stockCode);
            _mainPanel.DrawVolumeChart(points);
        }

        private void DrawMfiChart()
        {
            var points = _mfiPointRepository.SelectAll(_stockCode);
            _mainPanel.DrawMfiChart(points);
        }

        private void DrawRsiChart()
        {
            var points = _rsiPointRepository.SelectAll(_stockCode);
            _mainPanel.DrawRsiChart(points);
        }

        private void DrawMacdChart()
        {
            var points = _macdPointRepository.SelectAll(_stockCode);
            _mainPanel.DrawMacdChart(points);
        }

        private void DrawMAChart()
        {
            var points = _maPointRepository.SelectAll(_stockCode);
            _mainPanel.DrawMAChart(points);
        }

        private void DrawBBChart()
        {
            var points = _bbPointRepository.SelectAll(_stockCode);
            _mainPanel.DrawBBChart(points);
        }

        private void DrawObvChart()
        {
            var points = _obvPointRepository.SelectAll(_stockCode);
            _mainPanel.DrawObvChart(points);
        }
    }
}
