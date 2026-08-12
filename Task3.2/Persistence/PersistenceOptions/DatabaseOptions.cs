using System.ComponentModel.DataAnnotations;
using Npgsql;

namespace Persistence.PersistenceOptions
{
    public sealed class DatabaseOptions
    {
        public const string EnvironmentKey   = "DB_CONNECTION_STRING";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5432;
        public string Database { get; set; } = string.Empty;
        [Required(AllowEmptyStrings =  false)]
        public string Username { get; set; } = string.Empty;
        [Required(AllowEmptyStrings =  false)]
        public string Password { get; set; } = string.Empty;
        public string? ConnectionString { get; set; } = string.Empty;
        public string BuildConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(ConnectionString))
            {
                return ConnectionString;
            }

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = Host,
                Port = Port,
                Database = Database,
                Username = Username,
                Password = Password,
                Pooling = true
            };
            return builder.ConnectionString;
        }

    }
}
