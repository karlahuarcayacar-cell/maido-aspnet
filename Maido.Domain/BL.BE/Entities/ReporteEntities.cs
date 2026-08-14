namespace Maido.Domain.BL.BE.Entities;

/// <summary>
/// CAPA DE DOMINIO - ENTIDADES DE REPORTE Y ESTADÍSTICA
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Estas clases de entidad son Objetos del Dominio especializados para proyecciones de analítica y BI (Business Intelligence).
/// No corresponden a tablas físicas directas, sino a los resultados devueltos por Stored Procedures de agregación (GROUP BY, SUM, COUNT).
/// </summary>

/// <summary>
/// Proyección del total agrupado de ventas obtenido por fecha (Día/Mes).
/// </summary>
public class ReporteVentas
{
    /// <summary>
    /// Día o fecha reportada.
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Cantidad de pedidos concretados en dicha fecha.
    /// </summary>
    public int TotalPedidos { get; set; }

    /// <summary>
    /// Sumatoria de montos facturados en dicha fecha (Sols).
    /// </summary>
    public decimal MontoTotal { get; set; }
}

/// <summary>
/// Proyección del Ranking de los platillos más vendidos en la plataforma.
/// </summary>
public class ReportePlatillos
{
    /// <summary>
    /// Identificador del platillo evaluado.
    /// </summary>
    public int IdPlatillo { get; set; }

    /// <summary>
    /// Nombre del platillo evaluado.
    /// </summary>
    public string NombrePlatillo { get; set; } = string.Empty;

    /// <summary>
    /// Total de porciones/platos vendidos.
    /// </summary>
    public int TotalUnidades { get; set; }

    /// <summary>
    /// Ingreso económico total acumulado por las ventas de este platillo específico.
    /// </summary>
    public decimal TotalIngresos { get; set; }
}

