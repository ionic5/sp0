using Sample.SP0.Client.Core.Entities;

namespace Sample.SP0.Client.Core
{
    public class DailyCandleStickRepository
    {
        private readonly BasicRepository _basicRepository;

        public DailyCandleStickRepository(BasicRepository basicRepository)
        {
            _basicRepository = basicRepository;
        }

        public IEnumerable<DailyCandlestick> SelectAll(string stockCode)
        {
            return _basicRepository.SelectAll("daily_candle_stick", stockCode, reader => new DailyCandlestick
            {
                StockCode = reader.GetString("stock_code"),
                TradeDate = reader.GetDateTime("trade_date"),
                OpenPrice = reader.GetDecimal("open_price"),
                ClosePrice = reader.GetDecimal("close_price"),
                HighPrice = reader.GetDecimal("high_price"),
                LowPrice = reader.GetDecimal("low_price"),
                Volume = reader.GetDecimal("volume")
            });
        }

        public void Insert(IEnumerable<DailyCandlestick> entities)
        {
            string query = @"INSERT INTO daily_candle_stick (stock_code, trade_date, open_price, close_price, high_price, low_price, volume)
                         VALUES (@stock_code, @trade_date, @open_price, @close_price, @high_price, @low_price, @volume)";

            _basicRepository.Insert("daily_candle_stick", query, entities, (cmd, entity) =>
            {
                cmd.Parameters.AddWithValue("@stock_code", entity.StockCode);
                cmd.Parameters.AddWithValue("@trade_date", entity.TradeDate);
                cmd.Parameters.AddWithValue("@open_price", entity.OpenPrice);
                cmd.Parameters.AddWithValue("@close_price", entity.ClosePrice);
                cmd.Parameters.AddWithValue("@high_price", entity.HighPrice);
                cmd.Parameters.AddWithValue("@low_price", entity.LowPrice);
                cmd.Parameters.AddWithValue("@volume", entity.Volume);
            });
        }
    }

}
