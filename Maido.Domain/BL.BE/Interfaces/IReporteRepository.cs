using Maido.Domain.BL.BE.Entities;

namespace Maido.Domain.BL.BE.Interfaces;

/// <summary>
/// CAPA DE DOMINIO - INTERFAZ DE REPOSITORIO: IReporteRepository
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Contrato especializado en consultas analíticas de Business Intelligence (BI).
/// Proporciona métodos para obtener indicadores de ventas agregados por fecha y los platillos con mayor rotación/ingreso.
/// </summary>
public interface IReporteRepository
{
    /// <summary>
    /// Ejecuta el reporte agregativo de ventas entre un rango de fechas especificado.
    /// </summary>
    Task<IEnumerable<ReporteVentas>> ReporteVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene el ranking "Top N" de los platillos más vendidos según las unidades solicitadas e ingresos generados.
    /// </summary>
    Task<IEnumerable<ReportePlatillos>> ReportePlatillosMasVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin, int top);
}

