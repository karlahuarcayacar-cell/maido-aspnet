using System.Data;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;
using Maido.Infrastructure.DL.DALC.Persistence;
using Microsoft.Data.SqlClient;

namespace Maido.Infrastructure.DL.DALC.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly DbConnectionFactory _factory;

    public ReporteRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<ReporteVentas>> ReporteVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var lista = new List<ReporteVentas>();
        using var conn = _factory.CreateConnection();
        using var cmd  = new SqlCommand("sp_ReporteVentasPorFecha", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
        cmd.Parameters.AddWithValue("@FechaFin",    fechaFin.Date);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new ReporteVentas
            {
                Fecha        = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                TotalPedidos = reader.GetInt32(reader.GetOrdinal("TotalPedidos")),
                MontoTotal   = reader.GetDecimal(reader.GetOrdinal("MontoTotal"))
            });
        }
        return lista;
    }

    public async Task<IEnumerable<ReportePlatillos>> ReportePlatillosMasVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin, int top)
    {
        var lista = new List<ReportePlatillos>();
        using var conn = _factory.CreateConnection();
        using var cmd  = new SqlCommand("sp_ReportePlatillosMasVendidos", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@FechaInicio", (object?)fechaInicio?.Date ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaFin",    (object?)fechaFin?.Date    ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Top",         top);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new ReportePlatillos
            {
                IdPlatillo     = reader.GetInt32(reader.GetOrdinal("IdPlatillo")),
                NombrePlatillo = reader.GetString(reader.GetOrdinal("NombrePlatillo")),
                TotalUnidades  = reader.GetInt32(reader.GetOrdinal("TotalUnidades")),
                TotalIngresos  = reader.GetDecimal(reader.GetOrdinal("TotalIngresos"))
            });
        }
        return lista;
    }
}
