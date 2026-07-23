namespace Maido.Domain.BL.BE.Entities;

/// <summary>
/// Resultado de reporte de ventas agrupado por fecha.
/// </summary>
public class ReporteVentas
{
    public DateTime Fecha { get; set; }
    public int TotalPedidos { get; set; }
    public decimal MontoTotal { get; set; }
}

/// <summary>
/// Resultado de reporte de platillos más vendidos.
/// </summary>
public class ReportePlatillos
{
    public int IdPlatillo { get; set; }
    public string NombrePlatillo { get; set; } = string.Empty;
    public int TotalUnidades { get; set; }
    public decimal TotalIngresos { get; set; }
}
