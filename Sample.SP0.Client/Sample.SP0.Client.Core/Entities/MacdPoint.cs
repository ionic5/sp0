namespace Sample.SP0.Client.Core.Entities
{
    public class MacdPoint
    {
        public string StockCode;
        public DateTime TradeDate;
        public decimal Macd;
        public decimal Signal;
        public decimal Histogram;

        public MacdPoint()
        {
            StockCode = string.Empty;
        }
    }
}
