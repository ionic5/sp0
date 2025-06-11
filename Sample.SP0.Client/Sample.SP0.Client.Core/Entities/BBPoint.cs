namespace Sample.SP0.Client.Core.Entities
{
    public class BBPoint
    {
        public string StockCode;
        public DateTime TradeDate;
        public decimal Upper;
        public decimal Middle;
        public decimal Lower;

        public BBPoint()
        {
            StockCode = string.Empty;
        }
    }
}
