using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

/// <summary>
/// CAPA DE APLICACIÓN - SERVICIO DE NEGOCIO: ReporteService
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Servicio encargado de orquestar la generación de reportes comerciales y estadísticas 
/// para el Dashboard de Administración y la generación de documentos PDF.
/// </summary>
public class ReporteService : IReporteService
{
    private readonly IReporteRepository _reporteRepository;

    public ReporteService(IReporteRepository reporteRepository)
    {
        _reporteRepository = reporteRepository;
    }

    /// <summary>
    /// Genera la lista de ventas agrupadas por fecha en un periodo seleccionado.
    /// </summary>
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

    /// <summary>
    /// Genera el ranking Top N de los platillos con mayor rotación e ingresos generados.
    /// </summary>
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

