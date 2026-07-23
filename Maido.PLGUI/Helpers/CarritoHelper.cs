using System.Text.Json;
using Maido.PLGUI.Models;
using Microsoft.AspNetCore.Http;

namespace Maido.PLGUI.Helpers;

/// <summary>
/// Helper para gestionar el carrito de compras en Session.
/// </summary>
public static class CarritoHelper
{
    private const string SessionKey = "Maido_Carrito";

    public static List<CarritoItem> ObtenerCarrito(ISession session)
    {
        var json = session.GetString(SessionKey);
        return string.IsNullOrEmpty(json)
            ? new List<CarritoItem>()
            : JsonSerializer.Deserialize<List<CarritoItem>>(json) ?? new List<CarritoItem>();
    }

    public static void GuardarCarrito(ISession session, List<CarritoItem> carrito)
    {
        session.SetString(SessionKey, JsonSerializer.Serialize(carrito));
    }

    public static void AgregarItem(ISession session, CarritoItem item)
    {
        var carrito = ObtenerCarrito(session);
        var existente = carrito.FirstOrDefault(c => c.IdPlatillo == item.IdPlatillo);
        if (existente is not null)
            existente.Cantidad += item.Cantidad;
        else
            carrito.Add(item);
        GuardarCarrito(session, carrito);
    }

    public static void ActualizarCantidad(ISession session, int idPlatillo, int cantidad)
    {
        var carrito = ObtenerCarrito(session);
        var item = carrito.FirstOrDefault(c => c.IdPlatillo == idPlatillo);
        if (item is not null)
        {
            if (cantidad <= 0)
                carrito.Remove(item);
            else
                item.Cantidad = cantidad;
        }
        GuardarCarrito(session, carrito);
    }

    public static void EliminarItem(ISession session, int idPlatillo)
    {
        var carrito = ObtenerCarrito(session);
        carrito.RemoveAll(c => c.IdPlatillo == idPlatillo);
        GuardarCarrito(session, carrito);
    }

    public static void LimpiarCarrito(ISession session)
    {
        session.Remove(SessionKey);
    }

    public static int TotalItems(ISession session)
        => ObtenerCarrito(session).Sum(c => c.Cantidad);

    public static decimal Subtotal(ISession session)
        => ObtenerCarrito(session).Sum(c => c.Subtotal);
}
