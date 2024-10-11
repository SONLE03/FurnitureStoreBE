using Npgsql;

namespace FurnitureStoreBE.Configurations
{
    public class ConnectionHelper
    {
        public static string GetDatabaseConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }
        public static string GetRedisConnectionString(IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");
            var redisHost = Environment.GetEnvironmentVariable("REDISHOST");
            var redisPort = Environment.GetEnvironmentVariable("REDISPORT") ?? "6379";
            var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");
            if (string.IsNullOrEmpty(redisHost))
            {
                Console.WriteLine("Using Redis connection string from configuration file.");
                Console.WriteLine(redisConnectionString);
                return redisConnectionString;
            }
            Console.WriteLine("Using Redis connection string from environment variables.");
            return BuildRedisConnectionString(redisHost, redisPort, redisPassword);
        }
        private static string BuildRedisConnectionString(string host, string port, string password)
        {
            var redisConnectionString = $"{host}:{port}";

            // Add password if available
            if (!string.IsNullOrEmpty(password))
            {
                redisConnectionString += $",password={password}";
            }

            Console.WriteLine($"Redis Connection String: {redisConnectionString}");
            return redisConnectionString;
        }

        // Build the connection string from the environment. i.e. Railway
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            Console.WriteLine($"Đang cố gắng kết nối bằng: {databaseUri}");
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }
    }
}
