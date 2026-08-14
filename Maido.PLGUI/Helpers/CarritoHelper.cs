using System.Text.Json;
using Maido.PLGUI.Models;
using Microsoft.AspNetCore.Http;

namespace Maido.PLGUI.Helpers;

/// <summary>
/// CAPA DE PRESENTACIÓN - HELPER DEL CARRITO DE COMPRAS: CarritoHelper
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. Carrito Persistido en Sesión mediante JSON:
///    `ISession` solo almacena tipos primitivos (string, int, byte[]).
///    Para guardar objetos complejos como `List<CarritoItem>`, utilizamos `JsonSerializer.Serialize` 
///    para transformar la lista C# a texto JSON en la sesión ("Maido_Carrito") y `JsonSerializer.Deserialize` para reconstruirla.
/// 
/// 2. Ventaja Operativa:
///    El cliente puede explorar el menú y agregar productos al carrito sin necesidad de guardar borradores temporales 
///    en la base de datos SQL Server, reduciendo la carga del servidor de base de datos.
/// </summary>
public static class CarritoHelper
{
    private const string SessionKey = "Maido_Carrito";

    /// <summary>
    /// Deserializa la lista de ítems del carrito guardada en la sesión del cliente actual.
    /// </summary>
    public static List<CarritoItem> ObtenerCarrito(ISession session)
    {
        var json = session.GetString(SessionKey);
        return string.IsNullOrEmpty(json)
            ? new List<CarritoItem>()
            : JsonSerializer.Deserialize<List<CarritoItem>>(json) ?? new List<CarritoItem>();
    }

    /// <summary>
    /// Serializa la lista C# de ítems a JSON y la guarda en la variable de sesión "Maido_Carrito".
    /// </summary>
    public static void GuardarCarrito(ISession session, List<CarritoItem> carrito)
    {
        session.SetString(SessionKey, JsonSerializer.Serialize(carrito));
    }

    /// <summary>
    /// Agrega un nuevo producto al carrito. Si el producto ya existía, incrementa su cantidad.
    /// </summary>
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

    /// <summary>
    /// Actualiza la cantidad de porciones de un ítem. Si la cantidad cae a <= 0, remueve el ítem.
    /// </summary>
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

    /// <summary>
    /// Quita completamente un producto del carrito por su IdPlatillo.
    /// </summary>
    public static void EliminarItem(ISession session, int idPlatillo)
    {
        var carrito = ObtenerCarrito(session);
        carrito.RemoveAll(c => c.IdPlatillo == idPlatillo);
        GuardarCarrito(session, carrito);
    }

    /// <summary>
    /// Vacía por completo la clave de sesión del carrito al finalizar el Checkout exitosamente.
    /// </summary>
    public static void LimpiarCarrito(ISession session)
    {
        session.Remove(SessionKey);
    }

    /// <summary>
    /// Retorna la sumatoria total de unidades físicas agregadas al carrito (Badge del Navbar).
    /// </summary>
    public static int TotalItems(ISession session)
        => ObtenerCarrito(session).Sum(c => c.Cantidad);

    /// <summary>
    /// Retorna la suma de los subtotales de todos los productos en el carrito.
    /// </summary>
    public static decimal Subtotal(ISession session)
        => ObtenerCarrito(session).Sum(c => c.Subtotal);
}

