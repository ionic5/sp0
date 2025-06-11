using Sample.SP0.Client.Core.Entities;
using Sample.SP0.Client.Core.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sample.SP0.Client.View
{
    /// <summary>
    /// MainPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPanel : UserControl, IMainPanel
    {
        public event EventHandler<CheckBoxChangedEventArgs> MACheckBoxChangedEvent;
        public event EventHandler<CheckBoxChangedEventArgs> BBCheckBoxChangedEvent;
        public event EventHandler VolumeRadioButtonCheckedEvent;
        public event EventHandler RsiRadioButtonCheckedEvent;
        public event EventHandler MfiRadioButtonCheckedEvent;
        public event EventHandler MacdRadioButtonCheckedEvent;
        public event EventHandler ObvRadioButtonCheckedEvent;
        public event EventHandler<ListItemChangedEventArgs> StockItemSelectedEvent;

        public MainPanel()
        {
            InitializeComponent();

            MACheckBoxChangedEvent = (sender, args) => { };
            BBCheckBoxChangedEvent = (sender, args) => { };

            VolumeRadioButtonCheckedEvent = (sender, args) => { };
            RsiRadioButtonCheckedEvent = (sender, args) => { };
            MfiRadioButtonCheckedEvent = (sender, args) => { };
            MacdRadioButtonCheckedEvent = (sender, args) => { };
            ObvRadioButtonCheckedEvent = (sender, args) => { };

            StockItemSelectedEvent = (sender, args) => { };

            ChartPanel.MACheckBoxChangedEvent += (sender, args) => { MACheckBoxChangedEvent?.Invoke(sender, args); };
            ChartPanel.BBCheckBoxChangedEvent += (sender, args) => { BBCheckBoxChangedEvent?.Invoke(sender, args); };

            ChartPanel.VolumeRadioButtonCheckedEvent += (sender, args) => { VolumeRadioButtonCheckedEvent?.Invoke(sender, args); };
            ChartPanel.RsiRadioButtonCheckedEvent += (sender, args) => { RsiRadioButtonCheckedEvent?.Invoke(sender, args); };
            ChartPanel.MfiRadioButtonCheckedEvent += (sender, args) => { MfiRadioButtonCheckedEvent?.Invoke(sender, args); };
            ChartPanel.MacdRadioButtonCheckedEvent += (sender, args) => { MacdRadioButtonCheckedEvent?.Invoke(sender, args); };
            ChartPanel.ObvRadioButtonCheckedEvent += (sender, args) => { ObvRadioButtonCheckedEvent?.Invoke(sender, args); };

            StockItemListPanel.StockItemSelectedEvent += (sender, args) => { StockItemSelectedEvent?.Invoke(sender, args); };
        }

        public void DrawCandlestickChart(IEnumerable<DailyCandlestick> candlesticks)
        {
            ChartPanel.DrawCandlestickChart(candlesticks);
        }

        public void DrawMAChart(IEnumerable<MaPoint> points)
        {
            ChartPanel.DrawMAChart(points);
        }

        public void DrawBBChart(IEnumerable<BBPoint> points)
        {
            ChartPanel.DrawBBChart(points);
        }

        public void ClearMAChart()
        {
            ChartPanel.ClearMAChart();
        }

        public void ClearBBChart()
        {
            ChartPanel.ClearBBChart();
        }

        public void DrawVolumeChart(IEnumerable<DailyCandlestick> candleSticks)
        {
            ChartPanel.DrawVolumeChart(candleSticks);
        }

        public void ClearSubChart()
        {
            ChartPanel.ClearSubChart();
        }

        public void DrawRsiChart(IEnumerable<RsiPoint> points)
        {
            ChartPanel.DrawRsiChart(points);
        }

        public void DrawMfiChart(IEnumerable<MfiPoint> points)
        {
            ChartPanel.AddMFIPoints(points);
        }

        public void DrawObvChart(IEnumerable<ObvPoint> points)
        {
            ChartPanel.AddOBVPoints(points);
        }

        public void DrawMacdChart(IEnumerable<MacdPoint> points)
        {
            ChartPanel.AddMACDPoints(points);
        }

        public void SetStockItmes(IEnumerable<StockItemInfo> items)
        {
            StockItemListPanel.SetStockItmes(items);
        }

        public void SelectFirstItem()
        {
            StockItemListPanel.SelectFirstItem();
        }

        public SubChartType GetEnabledSubChartType()
        {
            return ChartPanel.GetEnabledSubChartType();
        }

        public bool IsOverlayChartEnabled(OverlayChartType chartType)
        {
            return ChartPanel.IsOverlayChartEnabled(chartType);
        }

        public void SelectVolumeRadioButton()
        {
            ChartPanel.SelectVolumeRadioButton();
        }

        public void ClearCharts()
        {
            ChartPanel.ClearCharts();
        }
    }
}
