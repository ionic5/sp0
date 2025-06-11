using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core
{
    public class MacdPointRepository
    {
        private readonly BasicRepository _basicRepository;

        public MacdPointRepository(BasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        public IEnumerable<MacdPoint> SelectAll(string stockCode)
        {
            return _basicRepository.SelectAll("macd_point", stockCode, reader => new MacdPoint
            {
                StockCode = reader.GetString("stock_code"),
                TradeDate = reader.GetDateTime("trade_date"),
                Macd = reader.GetDecimal("macd"),
                Signal = reader.GetDecimal("signal"),
                Histogram = reader.GetDecimal("histogram")
            });
        }

        public void Insert(IEnumerable<MacdPoint> entities)
        {
            string query = @"INSERT INTO macd_point (stock_code, trade_date, macd, `signal`, histogram)
                         VALUES (@stock_code, @trade_date, @macd, @signal, @histogram)";

            _basicRepository.Insert("macd_point", query, entities, (cmd, entity) =>
            {
                cmd.Parameters.AddWithValue("@stock_code", entity.StockCode);
                cmd.Parameters.AddWithValue("@trade_date", entity.TradeDate);
                cmd.Parameters.AddWithValue("@macd", entity.Macd);
                cmd.Parameters.AddWithValue("@signal", entity.Signal);
                cmd.Parameters.AddWithValue("@histogram", entity.Histogram);
            });
        }
    }
}
