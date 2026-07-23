using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

public class ReporteService : IReporteService
{
    private readonly IReporteRepository _reporteRepository;

    public ReporteService(IReporteRepository reporteRepository)
    {
        _reporteRepository = reporteRepository;
    }

    public async Task<IEnumerable<VentaPorFechaDto>> ReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var ventas = await _reporteRepository.ReporteVentasPorFechaAsync(fechaInicio, fechaFin);
        
        return ventas.Select(v => new VentaPorFechaDto
        {
            Fecha = v.Fecha,
            TotalPedidos = v.TotalPedidos,
            MontoTotal = v.MontoTotal
        });
    }

    public async Task<IEnumerable<PlatilloMasVendidoDto>> PlatillosMasVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin, int top = 10)
    {
        var platillos = await _reporteRepository.ReportePlatillosMasVendidosAsync(fechaInicio, fechaFin, top);
        
        return platillos.Select(p => new PlatilloMasVendidoDto
        {
            IdPlatillo = p.IdPlatillo,
            NombrePlatillo = p.NombrePlatillo,
            TotalUnidades = p.TotalUnidades,
            TotalIngresos = p.TotalIngresos
        });
    }
}
