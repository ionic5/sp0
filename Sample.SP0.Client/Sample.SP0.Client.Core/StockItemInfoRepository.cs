using MySqlConnector;
using Sample.SP0.Client.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core
{
    public class StockItemInfoRepository
    {
        private readonly string connectionString;
        private readonly ILogger logger;

        public StockItemInfoRepository(string connectionString, ILogger logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }

        public void Update(IEnumerable<StockItemInfo> entities)
        {
            if (entities == null || !entities.Any())
            {
                logger.Warn("Update operation aborted: No valid entities provided.");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (var truncateCmd = new MySqlCommand("TRUNCATE TABLE stock_item_info", conn, transaction))
                        {
                            truncateCmd.ExecuteNonQuery();
                            logger.Info("All rows have been removed from stock_item_info.");
                        }

                        using (var insertCmd = new MySqlCommand("INSERT INTO stock_item_info (company_name, stock_code, market_cap) VALUES (@company_name, @stock_code, @market_cap)", conn, transaction))
                        {
                            insertCmd.Parameters.Add("@company_name", MySqlDbType.VarChar);
                            insertCmd.Parameters.Add("@stock_code", MySqlDbType.VarChar);
                            insertCmd.Parameters.Add("@market_cap", MySqlDbType.Int64);

                            foreach (var entity in entities)
                            {
                                insertCmd.Parameters["@company_name"].Value = entity.CompanyName;
                                insertCmd.Parameters["@stock_code"].Value = entity.StockCode;
                                insertCmd.Parameters["@market_cap"].Value = entity.MarketCap;
                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        logger.Info($"{entities.Count()} records updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        logger.Warn($"Update operation failed: {ex.Message}");
                    }
                }
            }
        }

        public bool IsEmpty()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM stock_item_info";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn($"Failed to check table emptiness: {ex.Message}");
                    return false;
                }
            }
        }

        public IEnumerable<StockItemInfo> SelectTop100ByMarketCap()
        {
            var stockItems = new List<StockItemInfo>();

            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT company_name, stock_code, market_cap FROM stock_item_info ORDER BY market_cap DESC LIMIT 100";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stockItems.Add(new StockItemInfo
                            {
                                CompanyName = reader.GetString(0),
                                StockCode = reader.GetString(1),
                                MarketCap = reader.GetInt64(2)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn($"Failed to retrieve stock item info: {ex.Message}");
                }
            }

            return stockItems;
        }
    }
}
