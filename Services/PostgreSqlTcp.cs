using Npgsql;

public class PostgreSqlTcp
    {
    private readonly IHostEnvironment _hostEnvironment;

    public PostgreSqlTcp(IHostEnvironment hostEnvironment) {
        _hostEnvironment = hostEnvironment;
    }
        public NpgsqlConnectionStringBuilder NewPostgreSqlTCPConnectionString()
        {
            // Equivalent connection string:
            // "Uid=<DB_USER>;Pwd=<DB_PASS>;Host=<INSTANCE_HOST>;Database=<DB_NAME>;"
            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                // Note: Saving credentials in environment variables is convenient, but not
                // secure - consider a more secure solution such as
                // Cloud Secret Manager (https://cloud.google.com/secret-manager) to help
                // keep secrets safe.
                // Host = Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME"),     // e.g. '127.0.0.1'
                Host = _hostEnvironment.IsDevelopment() ? "127.0.0.1" : Environment.GetEnvironmentVariable("INSTANCE_HOST"),
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                Username = Environment.GetEnvironmentVariable("DB_USER"), // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                // TrustServerCertificate = false,
                SslMode = SslMode.Disable,
            };
            if (!_hostEnvironment.IsDevelopment())
            {
                connectionString.SslMode = SslMode.VerifyCA;
                connectionString.RootCertificate = Environment.GetEnvironmentVariable("DB_SERVER_CA");
                connectionString.SslCertificate = Environment.GetEnvironmentVariable("DB_CLIENT_CERT");
                connectionString.SslKey = Environment.GetEnvironmentVariable("DB_CLIENT_KEY");
            }
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
        }
    }
