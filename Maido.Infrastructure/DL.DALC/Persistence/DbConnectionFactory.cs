using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Maido.Infrastructure.DL.DALC.Persistence;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("maido_db")
            ?? throw new InvalidOperationException("Connection string 'maido_db' not found.");
    }

    public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
}
