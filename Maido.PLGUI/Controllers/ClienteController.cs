using Maido.Application.BL.BC.DTOs;
using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// Controlador del área de clientes autenticados.
/// </summary>
public class ClienteController : Controller
{
    private readonly IPedidoService _pedidoService;

    public ClienteController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    // ─────────────────────────────────────────────────────
    // GET: Mis Pedidos
    // ─────────────────────────────────────────────────────
    public async Task<IActionResult> MisPedidos()
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login", "Account");

        var idUsuario = SesionHelper.ObtenerIdUsuario(HttpContext.Session)!.Value;
        var pedidos   = await _pedidoService.ListarPorUsuarioAsync(idUsuario);

        ViewBag.NombreUsuario = SesionHelper.ObtenerNombre(HttpContext.Session);
        return View(pedidos);
    }

    // ─────────────────────────────────────────────────────
    // GET: Detalle de pedido
    // ─────────────────────────────────────────────────────
    public async Task<IActionResult> DetallePedido(int id)
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login", "Account");

        var pedido = await _pedidoService.ObtenerDetalleAsync(id);
        if (pedido is null)
            return RedirectToAction("MisPedidos");

        return View(pedido);
    }
}
