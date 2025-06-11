namespace Sample.SP0.Client.Core.Entities
{
    public class DailyCandlestick
    {
        public string StockCode;
        public DateTime TradeDate;
        public decimal OpenPrice;
        public decimal ClosePrice;
        public decimal HighPrice;
        public decimal LowPrice;
        public decimal Volume;

        public DailyCandlestick()
        {
            StockCode = string.Empty;
        }
    }
}
