namespace Sample.SP0.Client.Core.Entities
{
    public class ObvPoint
    {
        public string StockCode;
        public DateTime TradeDate;
        public decimal Obv;

        public ObvPoint()
        {
            StockCode = string.Empty;
        }
    }
}
