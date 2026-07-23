using Maido.Domain.BL.BE.Entities;

namespace Maido.Domain.BL.BE.Interfaces;

public interface IReporteRepository
{
    Task<IEnumerable<ReporteVentas>> ReporteVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<ReportePlatillos>> ReportePlatillosMasVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin, int top);
}
