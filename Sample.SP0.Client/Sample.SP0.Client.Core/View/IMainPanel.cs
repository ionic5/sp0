
using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core.View
{
    public interface IMainPanel
    {
        event EventHandler<ListItemChangedEventArgs> StockItemSelectedEvent;

        event EventHandler<CheckBoxChangedEventArgs> MACheckBoxChangedEvent;
        event EventHandler<CheckBoxChangedEventArgs> BBCheckBoxChangedEvent;

        event EventHandler VolumeRadioButtonCheckedEvent;
        event EventHandler RsiRadioButtonCheckedEvent;
        event EventHandler MfiRadioButtonCheckedEvent;
        event EventHandler MacdRadioButtonCheckedEvent;
        event EventHandler ObvRadioButtonCheckedEvent;

        void DrawCandlestickChart(IEnumerable<DailyCandlestick> dailyCandlesticks);

        void DrawMAChart(IEnumerable<MaPoint> points);
        void ClearMAChart();
        void DrawBBChart(IEnumerable<BBPoint> points);
        void ClearBBChart();

        bool IsOverlayChartEnabled(OverlayChartType chartType);

        void DrawMacdChart(IEnumerable<MacdPoint> points);
        void DrawMfiChart(IEnumerable<MfiPoint> points);
        void DrawObvChart(IEnumerable<ObvPoint> points);
        void DrawRsiChart(IEnumerable<RsiPoint> points);
        void DrawVolumeChart(IEnumerable<DailyCandlestick> candleSticks);

        void ClearSubChart();
        SubChartType GetEnabledSubChartType();

        void SelectFirstItem();
        void SetStockItmes(IEnumerable<StockItemInfo> items);
        void SelectVolumeRadioButton();
        void ClearCharts();
    }
}
