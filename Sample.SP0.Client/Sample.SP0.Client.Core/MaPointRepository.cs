using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core
{
    public class MaPointRepository
    {
        private readonly BasicRepository _basicRepository;

        public MaPointRepository(BasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        public IEnumerable<MaPoint> SelectAll(string stockCode)
        {
            return _basicRepository.SelectAll("ma_point", stockCode, reader => new MaPoint
            {
                StockCode = reader.GetString("stock_code"),
                TradeDate = reader.GetDateTime("trade_date"),
                Ma5 = reader.GetDecimal("ma5"),
                Ma20 = reader.GetDecimal("ma20"),
                Ma60 = reader.GetDecimal("ma60")
            });
        }

        public void Insert(IEnumerable<MaPoint> entities)
        {
            string query = @"INSERT INTO ma_point (stock_code, trade_date, ma5, ma20, ma60)
                         VALUES (@stock_code, @trade_date, @ma5, @ma20, @ma60)";

            _basicRepository.Insert("ma_point", query, entities, (cmd, entity) =>
            {
                cmd.Parameters.AddWithValue("@stock_code", entity.StockCode);
                cmd.Parameters.AddWithValue("@trade_date", entity.TradeDate);
                cmd.Parameters.AddWithValue("@ma5", entity.Ma5);
                cmd.Parameters.AddWithValue("@ma20", entity.Ma20);
                cmd.Parameters.AddWithValue("@ma60", entity.Ma60);
            });
        }
    }
}
