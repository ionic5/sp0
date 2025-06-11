using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core
{
    public class BBPointRepository
    {
        private readonly BasicRepository _basicRepository;

        public BBPointRepository(BasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        public IEnumerable<BBPoint> SelectAll(string stockCode)
        {
            return _basicRepository.SelectAll("bb_point", stockCode, reader => new BBPoint
            {
                StockCode = reader.GetString("stock_code"),
                TradeDate = reader.GetDateTime("trade_date"),
                Upper = reader.GetDecimal("upper"),
                Middle = reader.GetDecimal("middle"),
                Lower = reader.GetDecimal("lower")
            });
        }

        public void Insert(IEnumerable<BBPoint> entities)
        {
            string query = @"INSERT INTO bb_point (stock_code, trade_date, upper, middle, lower)
                         VALUES (@stock_code, @trade_date, @upper, @middle, @lower)";

            _basicRepository.Insert("bb_point", query, entities, (cmd, entity) =>
            {
                cmd.Parameters.AddWithValue("@stock_code", entity.StockCode);
                cmd.Parameters.AddWithValue("@trade_date", entity.TradeDate);
                cmd.Parameters.AddWithValue("@upper", entity.Upper);
                cmd.Parameters.AddWithValue("@middle", entity.Middle);
                cmd.Parameters.AddWithValue("@lower", entity.Lower);
            });
        }
    }
}
