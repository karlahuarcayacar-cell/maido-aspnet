using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Maido.Infrastructure.DL.DALC.Persistence;

/// <summary>
/// CAPA DE INFRAESTRUCTURA - PERSISTENCIA: DbConnectionFactory
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. Patrón Factory (Fábrica):
///    Encapsula la creación del objeto de conexión a SQL Server (`SqlConnection`).
///    Centraliza la lectura de la cadena de conexión "maido_db" definida en `appsettings.json`.
/// 
/// 2. Gestión de Conexiones SQL (ADO.NET Connection Pooling):
///    `CreateConnection()` devuelve un nuevo objeto `SqlConnection` cerrado.
///    Cuando el repositorio usa `using var connection = _connectionFactory.CreateConnection()`, 
///    se garantiza que la conexión se abra y cierre eficientemente aprovechando el Pool de Conexiones de ADO.NET.
/// </summary>
public class DbConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Lee la cadena de conexión desde `appsettings.json` mediante la interfaz `IConfiguration`.
    /// </summary>
    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("maido_db")
            ?? throw new InvalidOperationException("Connection string 'maido_db' not found.");
    }

    /// <summary>
    /// Instancia una nueva conexión a SQL Server con la cadena configurada.
    /// </summary>
    public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
}

