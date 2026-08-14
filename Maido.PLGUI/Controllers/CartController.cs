using System.Text.RegularExpressions;
using Maido.Application.BL.BC.DTOs;
using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Maido.PLGUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// CAPA DE PRESENTACIÓN - CONTROLADOR DEL CARRITO DE COMPRAS: CartController
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. Interacción Asíncrona con AJAX y JSON (`[FromBody]` / `JsonResult`):
///    Métodos como `AgregarItem`, `ActualizarCantidad` y `EliminarItem` responden a peticiones asíncronas desde JavaScript (Fetch API / jQuery AJAX).
///    No recargan la página completa; responden objetos JSON (`Json(new { success = true, ... })`) permitiendo actualizar el DOM dinámicamente.
/// 
/// 2. Validaciones con Expresiones Regulares (Regex):
///    En el método `Checkout(CheckoutDto)`, se valida que el celular ingresado sea un número válido de Perú
///    comenzando con 9 y de exactamente 9 dígitos (`^9\d{8}$`).
/// 
/// 3. Orquestación del Checkout y Limpieza de Carrito:
///    Una vez validado el pedido y registrado transaccionalmente mediante `_pedidoService.RegistrarPedidoAsync`,
///    se ejecuta `CarritoHelper.LimpiarCarrito(HttpContext.Session)` para vaciar la sesión antes de redirigir a la confirmación.
/// </summary>
public class CartController : Controller
{
    private readonly IPlatilloService _platilloService;
    private readonly IPedidoService _pedidoService;
    private readonly IUsuarioService _usuarioService;

    /// <summary>
    /// Inyección de los servicios requeridos para el proceso de compra.
    /// </summary>
    public CartController(IPlatilloService platilloService, IPedidoService pedidoService, IUsuarioService usuarioService)
    {
        _platilloService = platilloService;
        _pedidoService = pedidoService;
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// [POST AJAX] Agrega un platillo al carrito persistido en la sesión HTTP.
    /// Responde JSON para actualizar los badges de la UI sin recargar la página.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AgregarItem([FromBody] AgregarCarritoRequest req)
    {
        var platillo = await _platilloService.ObtenerPorIdAsync(req.IdPlatillo);
        if (platillo is null)
            return Json(new { success = false, message = "Platillo no encontrado." });

        var item = new CarritoItem
        {
            IdPlatillo = platillo.IdPlatillo,
            Nombre = platillo.Nombre,
            Precio = platillo.Precio,
            Cantidad = req.Cantidad > 0 ? req.Cantidad : 1,
            ImagenUrl = platillo.ImagenUrl
        };
        CarritoHelper.AgregarItem(HttpContext.Session, item);

        return Json(new
        {
            success = true,
            totalItems = CarritoHelper.TotalItems(HttpContext.Session),
            subtotal = CarritoHelper.Subtotal(HttpContext.Session)
        });
    }

    /// <summary>
    /// [POST AJAX] Modifica la cantidad de porciones de un ítem en la sesión.
    /// </summary>
    [HttpPost]
    public IActionResult ActualizarCantidad([FromBody] ActualizarCantidadRequest req)
    {
        CarritoHelper.ActualizarCantidad(HttpContext.Session, req.IdPlatillo, req.Cantidad);
        var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);
        var subtotal = CarritoHelper.Subtotal(HttpContext.Session);
        var igv = Math.Round(subtotal * 0.18m, 2);
        var total = subtotal + igv;

        return Json(new
        {
            success = true,
            totalItems = CarritoHelper.TotalItems(HttpContext.Session),
            subtotal,
            igv,
            total
        });
    }

    /// <summary>
    /// [POST AJAX] Elimina un ítem del carrito en la sesión.
    /// </summary>
    [HttpPost]
    public IActionResult EliminarItem([FromBody] EliminarItemRequest req)
    {
        CarritoHelper.EliminarItem(HttpContext.Session, req.IdPlatillo);
        var subtotal = CarritoHelper.Subtotal(HttpContext.Session);
        var igv = Math.Round(subtotal * 0.18m, 2);
        var total = subtotal + igv;
        
        return Json(new
        {
            success = true,
            totalItems = CarritoHelper.TotalItems(HttpContext.Session),
            subtotal,
            igv,
            total
        });
    }

    /// <summary>
    /// [GET AJAX] Retorna el estado completo del carrito en formato JSON.
    /// </summary>
    [HttpGet]
    public IActionResult ObtenerCarrito()
    {
        var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);
        var subtotal = carrito.Sum(c => c.Subtotal);
        var igv = Math.Round(subtotal * 0.18m, 2);
        var total = subtotal + igv;

        return Json(new { items = carrito, subtotal, igv, total });
    }

    /// <summary>
    /// [GET] Muestra la vista principal del Carrito de Compras (`/Cart/Index`).
    /// Envía datos mediante `ViewBag` y carga sugerencias aleatorias de platillos destacados.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);
        var subtotal = carrito.Sum(c => c.Subtotal);
        var igv = Math.Round(subtotal * 0.18m, 2);

        var todos = await _platilloService.ListarPublicoAsync(null, null);
        var sugerencias = todos.Where(p => p.Destacado && !carrito.Any(c => c.IdPlatillo == p.IdPlatillo))
                               .OrderBy(x => Guid.NewGuid())
                               .Take(2);

        ViewBag.Carrito = carrito;
        ViewBag.Subtotal = subtotal;
        ViewBag.IGV = igv;
        ViewBag.Total = subtotal + igv;
        ViewBag.Sugerencias = sugerencias;

        return View();
    }

    /// <summary>
    /// [GET] Muestra la pantalla de Checkout para ingresar teléfono, dirección y forma de pago.
    /// Exige que el usuario esté autenticado (`SesionHelper.EstaAutenticado`).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login", "Account", new { returnUrl = "/Cart/Checkout" });

        var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);
        if (!carrito.Any())
            return RedirectToAction("Index", "Home");

        var subtotal = carrito.Sum(c => c.Subtotal);
        var igv = Math.Round(subtotal * 0.18m, 2);
        var total = subtotal + igv;

        ViewBag.Carrito = carrito;
        ViewBag.Subtotal = subtotal;
        ViewBag.IGV = igv;
        ViewBag.Total = total;

        var emailUsuario = HttpContext.Session.GetString("Maido_Email");
        var perfil = emailUsuario != null ? await _usuarioService.ObtenerPerfilPorEmailAsync(emailUsuario) : null;

        var checkout = new CheckoutDto
        {
            Telefono = perfil?.Telefono,
            DireccionEntrega = perfil?.Direccion
        };

        return View(checkout);
    }

    /// <summary>
    /// [POST] Procesa la orden de compra en la base de datos.
    /// Valida el número telefónico con Expresiones Regulares (`Regex`) y la dirección en caso de ser Delivery.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutDto checkout)
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login", "Account");

        var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);
        if (!carrito.Any())
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction("Index", "Home");
        }

        // Validación estricta con Expresión Regular para celulares de Perú (9 dígitos, inicia con 9)
        if (string.IsNullOrWhiteSpace(checkout.Telefono))
        {
            ModelState.AddModelError("Telefono", "El teléfono es obligatorio.");
        }
        else if (!Regex.IsMatch(checkout.Telefono.Trim(), @"^9\d{8}$"))
        {
            ModelState.AddModelError("Telefono", "Ingresa un celular válido: 9 dígitos, solo números, empieza con 9.");
        }

        if (checkout.TipoPedido == "Delivery" && string.IsNullOrWhiteSpace(checkout.DireccionEntrega))
        {
            ModelState.AddModelError("DireccionEntrega", "La dirección de entrega es obligatoria para Delivery.");
        }
       
        if (!ModelState.IsValid)
        {
            var subtotalError = carrito.Sum(c => c.Subtotal);
            var igvError = Math.Round(subtotalError * 0.18m, 2);
            var totalError = subtotalError + igvError;

            ViewBag.Carrito = carrito;
            ViewBag.Subtotal = subtotalError;
            ViewBag.IGV = igvError;
            ViewBag.Total = totalError;

            return View(checkout);
        }

        var idUsuario = SesionHelper.ObtenerIdUsuario(HttpContext.Session)!.Value;
        var items = carrito.Select(c => new DetallePedidoDto
        {
            IdPlatillo = c.IdPlatillo,
            Nombre = c.Nombre,
            Precio = c.Precio,
            Cantidad = c.Cantidad
        }).ToList();

        // Ejecutar el procedimiento almacenado transaccional en SQL Server
        var idPedido = await _pedidoService.RegistrarPedidoAsync(idUsuario, items, checkout);

        if (idPedido > 0)
        {
            // Limpiar la clave de sesión del carrito tras una compra exitosa
            CarritoHelper.LimpiarCarrito(HttpContext.Session);
            return RedirectToAction("Confirmacion", new { id = idPedido });
        }

        TempData["Error"] = "Ocurrió un error al procesar el pedido. Intente nuevamente.";
        return RedirectToAction("Checkout");
    }

    /// <summary>
    /// [GET] Muestra la boleta o constancia de confirmación del pedido recién generado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Confirmacion(int id)
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login", "Account");

        var pedido = await _pedidoService.ObtenerDetalleAsync(id);
        if (pedido is null)
            return RedirectToAction("Index", "Home");

        return View(pedido);
    }
}

// Records fuertemente tipados de C# para deserialización automática de payloads JSON enviados por AJAX
public record AgregarCarritoRequest(int IdPlatillo, int Cantidad);
public record ActualizarCantidadRequest(int IdPlatillo, int Cantidad);
public record EliminarItemRequest(int IdPlatillo);