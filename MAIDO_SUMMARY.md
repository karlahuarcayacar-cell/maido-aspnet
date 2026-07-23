# Maido Restaurante - Resumen del Proyecto

## Información General
- **Proyecto:** Sistema Web de Pedidos en Línea - Restaurante Maido (Cocina Nikkei)
- **Curso:** Desarrollo de Servicios Web I | Cibertec
- **Arquitectura:** Clean Architecture (4 Capas: Domain, Application, Infrastructure, PLGUI)
- **Tecnologías:** .NET 10, ASP.NET Core MVC, ADO.NET, SQL Server, Bootstrap 5, SweetAlert2.

## Credenciales de Acceso
Para probar el sistema, puedes utilizar los siguientes usuarios semilla:

| Rol | Email | Contraseña |
|---|---|---|
| Administrador | `admin@maido.pe` | `admin` |
| Cliente | *Puedes crear uno desde el formulario de registro* | - |

## Estructura de Capas
1. **Maido.Domain:** Entidades de negocio (`Platillo`, `Pedido`, etc.) e interfaces de repositorios (`IPlatilloRepository`, etc.). Sin dependencias.
2. **Maido.Application:** DTOs y Servicios con la lógica de negocio (`PlatilloService`, `CarritoHelper`). Referencia a Domain.
3. **Maido.Infrastructure:** Implementación de acceso a datos con ADO.NET y Stored Procedures (`PlatilloRepository`, etc.). Referencia a Domain.
4. **Maido.PLGUI:** Presentación (Controladores y Vistas Razor). Referencia a Application e Infrastructure.

## Base de Datos
El script completo de la base de datos se encuentra en la raíz del proyecto: `maido_db.sql`.
- Tablas: Roles, Usuarios, Categorias, Platillos, Pedidos, DetallePedido.
- Incluye Store Procedures para todo el CRUD y procesos transaccionales (registro de pedidos).
- Incluye datos semilla (usuarios, categorías y 13 platillos con imágenes de Unsplash).

## Registro de Cambios (Changelog)

### [23-07-2026] - Finalización de Vistas de Administración
- **Vistas Faltantes:** Se crearon las vistas `Categorias`, `CrearCategoria`, `EditarCategoria` y `Usuarios` en el panel de Administración. 
- **Estilos de Tablas:** Se implementó una corrección global en el archivo `maido.css` y las vistas Razor para forzar que el fondo de las tablas Bootstrap respete el modo oscuro (dark theme) del diseño.
- **Detalle de Pedidos (Fix):** Se ajustó el mapeo JSON en la base de datos y la función de repositorio en C# para sincronizar las columnas al procesar o ver resúmenes de pedidos, solucionando los errores de `IndexOutOfRangeException` y `NULL insertions`.

### [23-07-2026] - Rediseño de Landing Page
- **Home/Index:** Se reemplazó la vista principal por una *Landing Page* cinematográfica a pantalla completa, con métricas clave, sección de *Delivery* e imágenes inmersivas según el diseño provisto por el usuario.
- **Home/Menu:** La antigua cuadrícula del catálogo se migró a `/Home/Menu` para separar claramente la experiencia de introducción de la experiencia de compra (e-commerce). Se implementaron Transiciones de Página (Fade-In) y Búsqueda Instantánea con Skeleton Loaders sin recarga de página (AJAX).
- **Correcciones (Menu y DB):** Se corrigieron los enlaces de filtrado de categorías en `Menu.cshtml` para que no redirigieran erróneamente a `Index`. Además, se actualizó la URL rota de Unsplash del platillo "Dragon Roll" directamente en la base de datos y en el script `maido_db.sql`.

### [23-07-2026] - Rediseño Total del Carrito
- **Eliminación del Offcanvas:** Se eliminó el antiguo menú lateral del carrito en `_Layout.cshtml`. El navbar ahora dirige a la página principal del carrito.
- **Nueva Vista de Carrito (Cart/Index):** Se integró un diseño "Nikkei Noir" con "glassmorphism", proporcionando una interfaz inmersiva de doble columna, resumen sticky y "cross-selling" dinámico ("También podría gustarte").
- **Fixes de Carrito:** 
  - Se corrigió el Z-Index del panel de resumen (Sidebar) para evitar superposición con el Navbar al scrollear.
  - Se modificó la lógica de JS para recargar automáticamente al añadir platillos desde sugerencias cruzadas.

### [23-07-2026] - Refinamiento de Base de Datos e Interfaz
- **Monitor de Pedidos (Fix):** Se resolvió un error de `InvalidOperationException` al mapear correctamente la Tupla de pedidos hacia el modelo `PedidosPaginadoDto` en `AdminController`.
- **Gestión de Usuarios (Fix):** Se corrigió un `IndexOutOfRangeException` al listar usuarios, haciendo condicional el mapeo del `PasswordHash` en el repositorio, ya que el reporte administrativo no incluye dicha columna de seguridad.
