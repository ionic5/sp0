namespace Sample.SP0.Client.Core.Entities
{
    public class MfiPoint
    {
        public string StockCode;
        public DateTime TradeDate;
        public decimal Mfi;

        public MfiPoint()
        {
            StockCode = string.Empty;
        }
    }
}
