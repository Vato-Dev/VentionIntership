namespace ApiGateway.Extensions
{
    public static  class StringExtensions
    {
        public static string? FromEnv(this string key) => 
            Environment.GetEnvironmentVariable(key);

        public static string FromEnv(this string key, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public static string FromEnvRequired(this string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Required environment variable is missing or empty: '{key}'");
            }
            return value;
        }
    }
}
