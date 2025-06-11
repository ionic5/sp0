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
            StockItemSelectedEvent = (sender, args) => { };

            StockItemListPanel.StockItemSelectedEvent += (sender, args) => { StockItemSelectedEvent?.Invoke(sender, args); };
        }

        public void SetStockItmes(IEnumerable<StockItemInfo> items)
        {
            StockItemListPanel.SetStockItmes(items);
        }

        public void SelectFirstItem()
        {
            StockItemListPanel.SelectFirstItem();
        }

        public void DrawCandlestickChart(IEnumerable<DailyCandlestick> dailyCandlesticks)
        {
            throw new NotImplementedException();
        }

        public void DrawMAChart(IEnumerable<MaPoint> points)
        {
            throw new NotImplementedException();
        }

        public void ClearMAChart()
        {
            throw new NotImplementedException();
        }

        public void DrawBBChart(IEnumerable<BBPoint> points)
        {
            throw new NotImplementedException();
        }

        public void ClearBBChart()
        {
            throw new NotImplementedException();
        }

        public bool IsOverlayChartEnabled(OverlayChartType chartType)
        {
            throw new NotImplementedException();
        }

        public void DrawMacdChart(IEnumerable<MacdPoint> points)
        {
            throw new NotImplementedException();
        }

        public void DrawMfiChart(IEnumerable<MfiPoint> points)
        {
            throw new NotImplementedException();
        }

        public void DrawObvChart(IEnumerable<ObvPoint> points)
        {
            throw new NotImplementedException();
        }

        public void DrawRsiChart(IEnumerable<RsiPoint> points)
        {
            throw new NotImplementedException();
        }

        public void DrawVolumeChart(IEnumerable<DailyCandlestick> candleSticks)
        {
            throw new NotImplementedException();
        }

        public void ClearSubChart()
        {
            throw new NotImplementedException();
        }

        public SubChartType GetEnabledSubChartType()
        {
            throw new NotImplementedException();
        }

        public void SelectVolumeRadioButton()
        {
            throw new NotImplementedException();
        }

        public void ClearCharts()
        {
            throw new NotImplementedException();
        }
    }
}
