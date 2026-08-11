using System.ComponentModel.DataAnnotations;

namespace Persistence.PersistenceOptions
{
    public sealed class DatabaseOptions
    {
        public const string EnvironmentKey   = "DB_CONNECTION_STRING";
        [Required(AllowEmptyStrings =  false)]
        public string ConnectionString { get; set; } = string.Empty;

    }
}
