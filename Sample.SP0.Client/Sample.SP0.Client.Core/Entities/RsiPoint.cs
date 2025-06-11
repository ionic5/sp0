namespace Sample.SP0.Client.Core.Entities
{
    public class RsiPoint
    {
        public string StockCode;
        public DateTime TradeDate;
        public decimal Rsi;

        public RsiPoint()
        {
            StockCode = string.Empty;
        }
    }
}
