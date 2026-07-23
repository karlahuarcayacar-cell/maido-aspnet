namespace Maido.Domain.BL.BE.Entities;
public class Platillo
{
    public int IdPlatillo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public int IdCategoria { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public bool Disponible { get; set; } = true;
    public bool Destacado { get; set; } = false;
    public DateTime FechaAlta { get; set; }
}
