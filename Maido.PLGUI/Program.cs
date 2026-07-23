using Maido.Application.BL.BC.Services;
using Maido.Infrastructure.DL.DALC.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────
// Servicios MVC
// ─────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ─────────────────────────────────────────────────────
// Sesiones (carrito + autenticación manual)
// ─────────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".Maido.Session";
});

// ─────────────────────────────────────────────────────
// Capas de Infraestructura y Aplicación
// ─────────────────────────────────────────────────────
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

// ─────────────────────────────────────────────────────
// HttpContextAccessor (para sesiones en servicios)
// ─────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ─────────────────────────────────────────────────────
// Pipeline HTTP
// ─────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

// Rutas
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Dashboard}/{id?}",
    defaults: new { controller = "Admin" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
