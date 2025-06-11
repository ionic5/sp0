using MySqlConnector;

namespace Sample.SP0.Client.Core
{
    public class BasicRepository
    {
        private readonly string connectionString;
        private readonly ILogger logger;

        public BasicRepository(string connectionString, ILogger logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }

        public IEnumerable<T> SelectAll<T>(string tableName, string stockCode, Func<MySqlDataReader, T> mapEntity)
        {
            var results = new List<T>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand($"SELECT * FROM {tableName} WHERE stock_code = @stock_code ORDER BY trade_date", conn))
                {
                    cmd.Parameters.Add("@stock_code", MySqlDbType.VarChar).Value = stockCode;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(mapEntity(reader));
                        }
                    }
                }
            }

            return results;
        }

        public void Insert<T>(string tableName, string insertQuery, IEnumerable<T> entities, Action<MySqlCommand, T> populateParameters)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (var insertCmd = new MySqlCommand(insertQuery, conn, transaction))
                        {
                            foreach (var entity in entities)
                            {
                                insertCmd.Parameters.Clear();

                                populateParameters(insertCmd, entity);
                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        logger.Info($"{entities.Count()} records inserted successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        logger.Warn($"Insert operation failed: {ex.Message}");
                    }
                }
            }
        }
    }

}
