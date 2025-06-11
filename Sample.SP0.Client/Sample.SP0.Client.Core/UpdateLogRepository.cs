using MySqlConnector;
using Sample.SP0.Client.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core
{
    public class UpdateLogRepository
    {
        private readonly string connectionString;
        private readonly ILogger logger;
        private readonly TypeTransformer typeTransformer;

        public UpdateLogRepository(string connectionString, ILogger logger, TypeTransformer typeTransformer)
        {
            this.connectionString = connectionString;
            this.logger = logger;
            this.typeTransformer = typeTransformer;
        }

        public UpdateLog Select(RepositoryName repositoryName)
        {
            string repositoryKey = typeTransformer.ConvertToString(repositoryName);

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new MySqlCommand("SELECT repository_name, last_updated FROM update_log WHERE repository_name = @repository_name", conn))
                {
                    cmd.Parameters.AddWithValue("@repository_name", repositoryKey);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UpdateLog(
                                typeTransformer.ConvertToRepositoryName(reader.GetString(0)),  // repository_name
                                reader.GetDateTime(1) // last_updated
                            );
                        }
                    }
                }

                logger.Warn($"No update log found for repository '{repositoryKey}', creating a new entry.");

                using (var insertCmd = new MySqlCommand("INSERT INTO update_log (repository_name, last_updated) VALUES (@repository_name, @last_updated)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@repository_name", repositoryKey);
                    insertCmd.Parameters.AddWithValue("@last_updated", DateTime.MinValue); // 초기값 설정
                    insertCmd.ExecuteNonQuery();
                }

                return new UpdateLog(repositoryName, DateTime.MinValue);
            }
        }

        public void Update(UpdateLog updateLog)
        {
            if (updateLog == null)
            {
                logger.Warn("Update operation aborted: UpdateLog object is null.");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("UPDATE update_log SET last_updated = @last_updated WHERE repository_name = @repository_name", conn))
                {
                    cmd.Parameters.AddWithValue("@repository_name", updateLog.RepositoryName.ToString());
                    cmd.Parameters.AddWithValue("@last_updated", updateLog.LastUpdated);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        logger.Info($"Update log for '{updateLog.RepositoryName}' updated successfully.");
                    }
                    else
                    {
                        logger.Warn($"No existing update log found for '{updateLog.RepositoryName}', inserting a new entry.");
                        Insert(updateLog);
                    }
                }
            }
        }

        public void Insert(UpdateLog updateLog)
        {
            if (updateLog == null)
            {
                logger.Warn("Insert operation aborted: UpdateLog object is null.");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("INSERT INTO update_log (repository_name, last_updated) VALUES (@repository_name, @last_updated) ON DUPLICATE KEY UPDATE last_updated = @last_updated", conn))
                {
                    cmd.Parameters.AddWithValue("@repository_name", updateLog.RepositoryName.ToString());
                    cmd.Parameters.AddWithValue("@last_updated", updateLog.LastUpdated);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        logger.Info($"Update log for '{updateLog.RepositoryName}' inserted successfully.");
                    }
                    else
                    {
                        logger.Warn($"Failed to insert update log for '{updateLog.RepositoryName}'.");
                    }
                }
            }
        }
    }
}
