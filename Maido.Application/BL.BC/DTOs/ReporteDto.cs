namespace Maido.Application.BL.BC.DTOs;

/// <summary>
/// CAPA DE APLICACIÓN - DTOs DE REPORTES
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Estructuras DTO para transportar datos analíticos a los gráficos del Dashboard de Administración 
/// y al generador de reportes ejecutivos PDF (QuestPDF).
/// </summary>

/// <summary>
/// DTO de proyección para gráficos de ventas agrupadas por día.
/// </summary>
public class VentaPorFechaDto
{
    public DateTime Fecha { get; set; }
    public int TotalPedidos { get; set; }
    public decimal MontoTotal { get; set; }
}

/// <summary>
/// DTO de proyección para el ranking de platillos más vendidos.
/// </summary>
public class PlatilloMasVendidoDto
{
    public int IdPlatillo { get; set; }
    public string NombrePlatillo { get; set; } = string.Empty;
    public int TotalUnidades { get; set; }
    public decimal TotalIngresos { get; set; }
}

/// <summary>
/// DTO que captura los filtros seleccionados por el Administrador en la pantalla de Reportes.
/// </summary>
public class FiltroReporteDto
{
    public DateTime FechaInicio { get; set; } = DateTime.Today.AddMonths(-1);
    public DateTime FechaFin { get; set; } = DateTime.Today;
    public int Top { get; set; } = 10;
}

