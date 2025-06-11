using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core
{
    public class RsiPointRepository
    {
        private readonly BasicRepository _basicRepository;

        public RsiPointRepository(BasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        public IEnumerable<RsiPoint> SelectAll(string stockCode)
        {
            return _basicRepository.SelectAll("rsi_point", stockCode, reader => new RsiPoint
            {
                StockCode = reader.GetString("stock_code"),
                TradeDate = reader.GetDateTime("trade_date"),
                Rsi = reader.GetDecimal("rsi")
            });
        }

        public void Insert(IEnumerable<RsiPoint> entities)
        {
            string query = @"INSERT INTO rsi_point (stock_code, trade_date, rsi)
                         VALUES (@stock_code, @trade_date, @rsi)";

            _basicRepository.Insert("rsi_point", query, entities, (cmd, entity) =>
            {
                cmd.Parameters.AddWithValue("@stock_code", entity.StockCode);
                cmd.Parameters.AddWithValue("@trade_date", entity.TradeDate);
                cmd.Parameters.AddWithValue("@rsi", entity.Rsi);
            });
        }

    }
}
