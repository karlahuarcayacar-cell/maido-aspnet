using System.Text.RegularExpressions;
using Maido.Application.BL.BC.DTOs;
using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Maido.PLGUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

public class CartController : Controller
{
    private readonly IPlatilloService _platilloService;
    private readonly IPedidoService _pedidoService;
    private readonly IUsuarioService _usuarioService;

    public CartController(IPlatilloService platilloService, IPedidoService pedidoService, IUsuarioService usuarioService)
    {
        _platilloService = platilloService;
        _pedidoService = pedidoService;
        _usuarioService = usuarioService;
    }

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

    [HttpGet]
    public IActionResult ObtenerCarrito()
    {
        var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);
        var subtotal = carrito.Sum(c => c.Subtotal);
        var igv = Math.Round(subtotal * 0.18m, 2);
        var total = subtotal + igv;

        return Json(new { items = carrito, subtotal, igv, total });
    }

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

        var idPedido = await _pedidoService.RegistrarPedidoAsync(idUsuario, items, checkout);

        if (idPedido > 0)
        {
            CarritoHelper.LimpiarCarrito(HttpContext.Session);
            return RedirectToAction("Confirmacion", new { id = idPedido });
        }

        TempData["Error"] = "Ocurrió un error al procesar el pedido. Intente nuevamente.";
        return RedirectToAction("Checkout");
    }

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

public record AgregarCarritoRequest(int IdPlatillo, int Cantidad);
public record ActualizarCantidadRequest(int IdPlatillo, int Cantidad);
public record EliminarItemRequest(int IdPlatillo);