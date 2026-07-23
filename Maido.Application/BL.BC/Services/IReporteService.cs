using Maido.Application.BL.BC.DTOs;

namespace Maido.Application.BL.BC.Services;

public interface IReporteService
{
    Task<IEnumerable<VentaPorFechaDto>> ReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<PlatilloMasVendidoDto>> PlatillosMasVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin, int top = 10);
}
