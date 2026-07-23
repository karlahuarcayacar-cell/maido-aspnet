namespace Maido.Application.BL.BC.DTOs;

public class VentaPorFechaDto
{
    public DateTime Fecha { get; set; }
    public int TotalPedidos { get; set; }
    public decimal MontoTotal { get; set; }
}

public class PlatilloMasVendidoDto
{
    public int IdPlatillo { get; set; }
    public string NombrePlatillo { get; set; } = string.Empty;
    public int TotalUnidades { get; set; }
    public decimal TotalIngresos { get; set; }
}

public class FiltroReporteDto
{
    public DateTime FechaInicio { get; set; } = DateTime.Today.AddMonths(-1);
    public DateTime FechaFin { get; set; } = DateTime.Today;
    public int Top { get; set; } = 10;
}
