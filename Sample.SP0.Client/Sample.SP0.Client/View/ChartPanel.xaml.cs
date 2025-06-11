using Sample.SP0.Client.Core.Entities;
using Sample.SP0.Client.Core.View;
using ScottPlot;
using System.Windows;
using System.Windows.Controls;

namespace Sample.SP0.Client.View
{
    /// <summary>
    /// ChartPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChartPanel : UserControl
    {
        public event EventHandler<CheckBoxChangedEventArgs> MACheckBoxChangedEvent;
        public event EventHandler<CheckBoxChangedEventArgs> BBCheckBoxChangedEvent;

        public event EventHandler VolumeRadioButtonCheckedEvent;
        public event EventHandler RsiRadioButtonCheckedEvent;
        public event EventHandler MfiRadioButtonCheckedEvent;
        public event EventHandler MacdRadioButtonCheckedEvent;
        public event EventHandler ObvRadioButtonCheckedEvent;

        private readonly List<ScottPlot.Plottables.Scatter> maLines;
        private readonly List<ScottPlot.Plottables.Scatter> bbLines;

        public ChartPanel()
        {
            InitializeComponent();

            MACheckBoxChangedEvent = (sender, args) => { };
            BBCheckBoxChangedEvent = (sender, args) => { };
            VolumeRadioButtonCheckedEvent = (sender, args) => { };
            RsiRadioButtonCheckedEvent = (sender, args) => { };
            MfiRadioButtonCheckedEvent = (sender, args) => { };
            MacdRadioButtonCheckedEvent = (sender, args) => { };
            ObvRadioButtonCheckedEvent = (sender, args) => { };

            maLines = new List<ScottPlot.Plottables.Scatter>();
            bbLines = new List<ScottPlot.Plottables.Scatter>();
        }

        public void DrawCandlestickChart(IEnumerable<DailyCandlestick> candleSticks)
        {
            var ohlcs = new List<OHLC>();
            foreach (var candlestick in candleSticks)
            {
                ohlcs.Add(new OHLC(Convert.ToDouble(candlestick.OpenPrice),
                    Convert.ToDouble(candlestick.HighPrice),
                    Convert.ToDouble(candlestick.LowPrice),
                    Convert.ToDouble(candlestick.ClosePrice),
                    candlestick.TradeDate, TimeSpan.FromDays(1)));
            }

            var plot = MainChartPlot.Plot.Add.Candlestick(ohlcs);
            plot.RisingColor = Colors.Red;
            plot.FallingColor = Colors.Blue;

            RefreshMainChart();
        }

        public void DrawBBChart(IEnumerable<BBPoint> points)
        {
            var uppers = new List<double>();
            var means = new List<double>();
            var lowers = new List<double>();
            var dates = new List<double>();
            foreach (var pt in points)
            {
                uppers.Add(Convert.ToDouble(pt.Upper));
                means.Add(Convert.ToDouble(pt.Middle));
                lowers.Add(Convert.ToDouble(pt.Lower));
                dates.Add(pt.TradeDate.ToOADate());
            }

            AddBollingerBandLine(dates, means, "Middle", LinePattern.Solid);
            AddBollingerBandLine(dates, uppers, "Upper", LinePattern.Dotted);
            AddBollingerBandLine(dates, lowers, "Lower", LinePattern.Dotted);

            RefreshMainChart();
        }

        private void AddBollingerBandLine(List<double> dates, List<double> yData, string legendText, LinePattern linePattern)
        {
            var plot = MainChartPlot.Plot.Add.Scatter(dates, yData);
            plot.MarkerSize = 0;
            plot.Color = Colors.Navy;
            plot.LineWidth = 2;
            plot.LinePattern = linePattern;

            bbLines.Add(plot);
        }

        public void ClearBBChart()
        {
            foreach (var line in bbLines)
            {
                line.LegendText = string.Empty;
                MainChartPlot.Plot.Remove(line);
            }
            bbLines.Clear();

            MainChartPlot.Refresh();
        }

        public void DrawMAChart(IEnumerable<MaPoint> points)
        {
            var dates = new Dictionary<int, List<double>>
            {
                { 5, new List<double>() },
                { 20, new List<double>() },
                { 60, new List<double>() }
            };

            var maValues = new Dictionary<int, List<double>>
            {
                { 5, new List<double>() },
                { 20, new List<double>() },
                { 60, new List<double>() }
            };

            foreach (var pt in points)
            {
                if (pt.Ma5 > 0)
                {
                    dates[5].Add(pt.TradeDate.ToOADate());
                    maValues[5].Add(Convert.ToDouble(pt.Ma5));
                }
                if (pt.Ma20 > 0)
                {
                    dates[20].Add(pt.TradeDate.ToOADate());
                    maValues[20].Add(Convert.ToDouble(pt.Ma20));
                }
                if (pt.Ma60 > 0)
                {
                    dates[60].Add(pt.TradeDate.ToOADate());
                    maValues[60].Add(Convert.ToDouble(pt.Ma60));
                }
            }

            var colors = new Dictionary<int, Color>
            {
                { 5, Colors.Green },
                { 20, Colors.DarkRed },
                { 60, Colors.Orange }
            };

            foreach (var windowSize in maValues.Keys)
            {
                ScottPlot.Plottables.Scatter plotMa = MainChartPlot.Plot.Add.Scatter(dates[windowSize], maValues[windowSize]);
                plotMa.LegendText = $"MA {windowSize}";
                plotMa.MarkerSize = 0;
                plotMa.LineWidth = 2;
                plotMa.Color = colors[windowSize];

                maLines.Add(plotMa);
            }

            MainChartPlot.Plot.ShowLegend();

            RefreshMainChart();
        }

        public void ClearMAChart()
        {
            foreach (var line in maLines)
            {
                line.LegendText = string.Empty;
                MainChartPlot.Plot.Remove(line);
            }
            maLines.Clear();

            MainChartPlot.Refresh();
        }

        public void ClearCharts()
        {
            maLines.Clear();
            bbLines.Clear();

            MainChartPlot.Plot.Clear();
            MainChartPlot.Refresh();

            ClearSubChart();
        }

        private void RefreshMainChart()
        {
            MainChartPlot.Plot.Axes.Bottom.IsVisible = false;
            MainChartPlot.Plot.Axes.AutoScale();
            MainChartPlot.Plot.Axes.DateTimeTicksBottom();

            MainChartPlot.Plot.Layout.Fixed(new PixelPadding(100, 10, 0, 0));

            MainChartPlot.Refresh();
        }

        public void DrawVolumeChart(IEnumerable<DailyCandlestick> candleSticks)
        {
            ClearSubChart();

            var volumes = new List<double>();
            var dates = new List<double>();
            foreach (var item in candleSticks)
            {
                volumes.Add(Convert.ToDouble(item.Volume));
                dates.Add(item.TradeDate.ToOADate());
            }

            SubChartPlot.Plot.Add.Bars(dates, volumes);

            SubChartPlot.Plot.Axes.Left.MinimumSize = 0;

            RefreshSubChart();
        }

        public void ClearSubChart()
        {
            MainChartPlot.Plot.Axes.Unlink(MainChartPlot.Plot.Axes.Bottom);
            SubChartPlot.Plot.Axes.Unlink(SubChartPlot.Plot.Axes.Bottom);

            SubChartPlot.Plot.Clear();
            SubChartPlot.Refresh();
        }

        private void RefreshSubChart()
        {
            SubChartPlot.Plot.Axes.DateTimeTicksBottom();
            SubChartPlot.Plot.Axes.Margins(bottom: 0, top: 0);
            SubChartPlot.Plot.Axes.Title.IsVisible = false;
            SubChartPlot.Plot.Axes.Top.IsVisible = true;
            SubChartPlot.Plot.Layout.Fixed(new PixelPadding(100, 10, 30, 0));

            MainChartPlot.Plot.Axes.Link(MainChartPlot.Plot.Axes.Bottom, SubChartPlot.Plot.Axes.Bottom, SubChartPlot.Plot);
            SubChartPlot.Plot.Axes.Link(SubChartPlot.Plot.Axes.Bottom, MainChartPlot.Plot.Axes.Bottom, MainChartPlot.Plot);

            SubChartPlot.Plot.Axes.AutoScale();

            SubChartPlot.Refresh();
        }

        public void DrawRsiChart(IEnumerable<RsiPoint> points)
        {
            ClearSubChart();

            var plot = SubChartPlot.Plot;

            var values = new List<double>();
            var dates = new List<double>();
            foreach (var pt in points)
            {
                values.Add(Convert.ToDouble(pt.Rsi));
                dates.Add(pt.TradeDate.ToOADate());
            }

            var sp1 = plot.Add.Scatter(dates, values);
            sp1.MarkerSize = 0;
            sp1.Color = Colors.Blue;

            var tline = plot.Add.HorizontalLine(70);
            tline.LineStyle.Pattern = LinePattern.Dotted;
            tline.Color = ScottPlot.Colors.Green;

            var bline = plot.Add.HorizontalLine(30);
            bline.LineStyle.Pattern = LinePattern.Dotted;
            bline.Color = ScottPlot.Colors.Red;

            RefreshSubChart();
        }

        internal void AddMFIPoints(IEnumerable<MfiPoint> points)
        {
            ClearSubChart();

            var plot = SubChartPlot.Plot;

            var values = new List<double>();
            var dates = new List<double>();
            foreach (var pt in points)
            {
                values.Add(Convert.ToDouble(pt.Mfi));
                dates.Add(pt.TradeDate.ToOADate());
            }

            var sp1 = plot.Add.Scatter(dates, values);
            sp1.MarkerSize = 0;
            sp1.Color = Colors.Blue;

            var tline = plot.Add.HorizontalLine(80);
            tline.LineStyle.Pattern = LinePattern.Dotted;
            tline.Color = ScottPlot.Colors.Green;

            var bline = plot.Add.HorizontalLine(20);
            bline.LineStyle.Pattern = LinePattern.Dotted;
            bline.Color = ScottPlot.Colors.Red;

            RefreshSubChart();
        }

        public void AddOBVPoints(IEnumerable<ObvPoint> points)
        {
            ClearSubChart();

            var plot = SubChartPlot.Plot;

            var values = new List<double>();
            var dates = new List<double>();
            foreach (var pt in points)
            {
                values.Add(Convert.ToDouble(pt.Obv));
                dates.Add(pt.TradeDate.ToOADate());
            }

            var sp1 = plot.Add.Scatter(dates, values);
            sp1.MarkerSize = 0;
            sp1.Color = Colors.Blue;

            RefreshSubChart();
        }

        public void AddMACDPoints(IEnumerable<MacdPoint> points)
        {
            ClearSubChart();

            var histograms = new List<double>();
            var macds = new List<double>();
            var signals = new List<double>();
            var dates = new List<double>();
            foreach (var pt in points)
            {
                histograms.Add(Convert.ToDouble(pt.Histogram));
                macds.Add(Convert.ToDouble(pt.Macd));
                signals.Add(Convert.ToDouble(pt.Signal));
                dates.Add(pt.TradeDate.ToOADate());
            }

            var plot = SubChartPlot.Plot;

            var macdLine = plot.Add.Scatter(dates, macds);
            macdLine.MarkerSize = 0;
            macdLine.Color = ScottPlot.Colors.Blue;
            macdLine.LegendText = $"Macd";

            var signalLine = plot.Add.Scatter(dates, signals);
            signalLine.MarkerSize = 0;
            signalLine.Color = ScottPlot.Colors.Orange;
            signalLine.LegendText = $"Signal";

            var histogramBars = plot.Add.Bars(dates, histograms);
            for (var i = 0; i < histogramBars.Bars.Count(); i++)
            {
                var bar = histogramBars.Bars[i];
                bar.FillColor = histograms[i] >= 0 ? ScottPlot.Colors.Green.WithAlpha(.7) : ScottPlot.Colors.Red.WithAlpha(.7);
            }

            var tline = plot.Add.HorizontalLine(0);
            tline.LineStyle.Pattern = LinePattern.Dotted;
            tline.Color = ScottPlot.Colors.LightGray;

            SubChartPlot.Plot.ShowLegend();

            RefreshSubChart();
        }

        private void OnMaToggleClicked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
                return;

            bool isChecked = checkBox.IsChecked ?? false;
            MACheckBoxChangedEvent(this, new CheckBoxChangedEventArgs(isChecked));
        }

        private void OnBBToggleClicked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
                return;

            bool isChecked = checkBox.IsChecked ?? false;
            BBCheckBoxChangedEvent(this, new CheckBoxChangedEventArgs(isChecked));
        }

        private void OnVolumeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            VolumeRadioButtonCheckedEvent(this, EventArgs.Empty);
        }

        private void OnRSIRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            RsiRadioButtonCheckedEvent(this, EventArgs.Empty);
        }

        private void OnMFIRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            MfiRadioButtonCheckedEvent(this, EventArgs.Empty);
        }

        private void OnOBVRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            ObvRadioButtonCheckedEvent(this, EventArgs.Empty);
        }

        private void OnMACDRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            MacdRadioButtonCheckedEvent(this, EventArgs.Empty);
        }

        public SubChartType GetEnabledSubChartType()
        {
            if (MACDRadioButton.IsChecked == true)
                return SubChartType.Macd;
            if (VolumeRadioButton.IsChecked == true)
                return SubChartType.Volume;
            if (RSIRadioButton.IsChecked == true)
                return SubChartType.Rsi;
            if (MFIRadioButton.IsChecked == true)
                return SubChartType.Mfi;
            if (OBVRadioButton.IsChecked == true)
                return SubChartType.Obv;
            return SubChartType.Void;
        }

        public bool IsOverlayChartEnabled(OverlayChartType chartType)
        {
            if (BBToggle.IsChecked == true && chartType == OverlayChartType.BB)
                return true;

            if (MAToggle.IsChecked == true && chartType == OverlayChartType.MA)
                return true;

            return false;
        }

        public void SelectVolumeRadioButton()
        {
            VolumeRadioButton.IsChecked = true;
        }
    }
}
