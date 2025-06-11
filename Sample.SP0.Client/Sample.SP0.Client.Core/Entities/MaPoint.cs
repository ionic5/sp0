namespace Sample.SP0.Client.Core.Entities
{
    public class MaPoint
    {
        public string StockCode;
        public DateTime TradeDate;
        public decimal Ma5;
        public decimal Ma20;
        public decimal Ma60;

        public MaPoint()
        {
            StockCode = string.Empty;
        }
    }
}
