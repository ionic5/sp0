using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core
{
    public class MfiPointRepository
    {
        private readonly BasicRepository _basicRepository;

        public MfiPointRepository(BasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        public IEnumerable<MfiPoint> SelectAll(string stockCode)
        {
            return _basicRepository.SelectAll("mfi_point", stockCode, reader => new MfiPoint
            {
                StockCode = reader.GetString("stock_code"),
                TradeDate = reader.GetDateTime("trade_date"),
                Mfi = reader.GetDecimal("mfi")
            });
        }

        public void Insert(IEnumerable<MfiPoint> entities)
        {
            string query = @"INSERT INTO mfi_point (stock_code, trade_date, mfi)
                         VALUES (@stock_code, @trade_date, @mfi)";

            _basicRepository.Insert("mfi_point", query, entities, (cmd, entity) =>
            {
                cmd.Parameters.AddWithValue("@stock_code", entity.StockCode);
                cmd.Parameters.AddWithValue("@trade_date", entity.TradeDate);
                cmd.Parameters.AddWithValue("@mfi", entity.Mfi);
            });
        }
    }

}
