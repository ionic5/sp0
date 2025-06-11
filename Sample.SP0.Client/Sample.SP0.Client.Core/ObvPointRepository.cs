using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core
{
    public class ObvPointRepository
    {
        private readonly BasicRepository _basicRepository;

        public ObvPointRepository(BasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        public IEnumerable<ObvPoint> SelectAll(string stockCode)
        {
            return _basicRepository.SelectAll("obv_point", stockCode, reader => new ObvPoint
            {
                StockCode = reader.GetString("stock_code"),
                TradeDate = reader.GetDateTime("trade_date"),
                Obv = reader.GetDecimal("obv")
            });
        }

        public void Insert(IEnumerable<ObvPoint> entities)
        {
            string query = @"INSERT INTO obv_point (stock_code, trade_date, obv)
                         VALUES (@stock_code, @trade_date, @obv)";

            _basicRepository.Insert("obv_point", query, entities, (cmd, entity) =>
            {
                cmd.Parameters.AddWithValue("@stock_code", entity.StockCode);
                cmd.Parameters.AddWithValue("@trade_date", entity.TradeDate);
                cmd.Parameters.AddWithValue("@obv", entity.Obv);
            });
        }
    }
}
