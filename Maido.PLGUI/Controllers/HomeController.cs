using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// CAPA DE PRESENTACIÓN - CONTROLADOR PRINCIPAL Y CATÁLOGO PÚBLICO: HomeController
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. Renderizado de Vistas Parciales (`PartialView`):
///    El método `FiltrarMenu` retorna `PartialView("_PlatillosGrid", platillos)`.
///    En lugar de refrescar toda la estructura HTML de la página web (Navbar, Footer, CSS), 
///    devuelve únicamente el bloque HTML del catálogo de platillos. JavaScript reemplaza este fragmento 
///    logrando una experiencia de usuario fluida estilo SPA (Single Page Application).
/// 
/// 2. Transferencia de Estado a la Vista:
///    Combina un Modelo Fuertemente Tipado (`return View(destacados)`) con variables dinámicas de apoyo 
///    en `ViewBag` (`ViewBag.TotalCarrito`, `ViewBag.EsAdmin`, `ViewBag.Categorias`).
/// </summary>
public class HomeController : Controller
{
    private readonly IPlatilloService _platilloService;
    private readonly ICategoriaService _categoriaService;

    public HomeController(IPlatilloService platilloService, ICategoriaService categoriaService)
    {
        _platilloService = platilloService;
        _categoriaService = categoriaService;
    }

    /// <summary>
    /// [GET] Página de inicio del restaurante Maido (`/ Home / Index`).
    /// Carga los platillos destacados (banners / carruseles) y el estado del carrito.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var todos = await _platilloService.ListarPublicoAsync(null, null);
        var destacados = todos.Where(p => p.Destacado).Take(8);
        ViewBag.TotalCarrito = CarritoHelper.TotalItems(HttpContext.Session);
        ViewBag.EsAdmin = SesionHelper.EstaAutenticado(HttpContext.Session)
                          && SesionHelper.EsAdministrador(HttpContext.Session);
        return View(destacados);
    }

    /// <summary>
    /// [GET] Carta completa con filtrado por categoría y caja de búsqueda.
    /// </summary>
    public async Task<IActionResult> Menu(int? idCategoria, string? busqueda)
    {
        var platillos = await _platilloService.ListarPublicoAsync(idCategoria, busqueda);
        var categorias = await _categoriaService.ListarPublicasAsync();

        ViewBag.Categorias = categorias;
        ViewBag.IdCategoriaActual = idCategoria;
        ViewBag.Busqueda = busqueda;
        ViewBag.TotalCarrito = CarritoHelper.TotalItems(HttpContext.Session);
        ViewBag.EsAdmin = SesionHelper.EstaAutenticado(HttpContext.Session)
                                    && SesionHelper.EsAdministrador(HttpContext.Session);

        return View(platillos);
    }

    /// <summary>
    /// [GET AJAX] Endpoint invocado por JavaScript al escribir en la barra de búsqueda o cambiar de pestaña de categoría.
    /// Devuelve únicamente la vista parcial `_PlatillosGrid.cshtml`.
    /// </summary>
    public async Task<IActionResult> FiltrarMenu(int? idCategoria, string? busqueda)
    {
        var platillos = await _platilloService.ListarPublicoAsync(idCategoria, busqueda);
        ViewBag.EsAdmin = SesionHelper.EstaAutenticado(HttpContext.Session)
                          && SesionHelper.EsAdministrador(HttpContext.Session);
        return PartialView("_PlatillosGrid", platillos);
    }

    /// <summary>
    /// [GET] Vista de manejo de errores no controlados.
    /// </summary>
    public IActionResult Error()
    {
        return View();
    }
}

