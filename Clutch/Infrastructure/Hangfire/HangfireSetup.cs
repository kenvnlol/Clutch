using Microsoft.Data.SqlClient;

namespace Clutch.Infrastructure.Hangfire
{
    public static class HangfireSetup
    {
        public static void EnsureHangfireDatabaseExists(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            var databaseName = builder.InitialCatalog;

            builder.InitialCatalog = "master";

            using var connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $@"
                IF DB_ID('{databaseName}') IS NULL
                CREATE DATABASE [{databaseName}]";

            command.ExecuteNonQuery();
        }
    }
}