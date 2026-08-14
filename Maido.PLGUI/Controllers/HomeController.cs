using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

public class HomeController : Controller
{
    private readonly IPlatilloService _platilloService;
    private readonly ICategoriaService _categoriaService;

    public HomeController(IPlatilloService platilloService, ICategoriaService categoriaService)
    {
        _platilloService = platilloService;
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index()
    {
        var todos = await _platilloService.ListarPublicoAsync(null, null);
        var destacados = todos.Where(p => p.Destacado).Take(8);
        ViewBag.TotalCarrito = CarritoHelper.TotalItems(HttpContext.Session);
        ViewBag.EsAdmin = SesionHelper.EstaAutenticado(HttpContext.Session)
                          && SesionHelper.EsAdministrador(HttpContext.Session);
        return View(destacados);
    }

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

    public async Task<IActionResult> FiltrarMenu(int? idCategoria, string? busqueda)
    {
        var platillos = await _platilloService.ListarPublicoAsync(idCategoria, busqueda);
        ViewBag.EsAdmin = SesionHelper.EstaAutenticado(HttpContext.Session)
                          && SesionHelper.EsAdministrador(HttpContext.Session);
        return PartialView("_PlatillosGrid", platillos);
    }

    public IActionResult Error()
    {
        return View();
    }
}
