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

## Notas de Arquitectura y Decisiones de Diseño
- **Acceso a Datos Asíncrono:** Absolutamente todos los repositorios (en `Maido.Infrastructure`) emplean el paradigma asíncrono de C# (`Task` y `async`/`await`). Esto significa que los hilos del servidor (`Worker Threads`) no se bloquean esperando la respuesta de SQL Server, lo que permite escalar el sistema y soportar miles de usuarios concurrentes (fundamental en horarios de alta demanda de delivery).
- **Gestión de Conexiones:** Se utiliza `DbConnectionFactory` (inyectado como Singleton) para instanciar las conexiones `SqlConnection` de manera optimizada. Los repositorios abren y cierran (vía bloques `using`) la conexión asíncronamente.
  - *Nota Técnica:* Las clases heredadas `BDConexion` e `IBDConexion` (que abrían conexiones síncronas de manera inmediata) se mantienen en la carpeta `Persistence` como legado, pero han sido reemplazadas completamente por `DbConnectionFactory` en el contenedor de inyección de dependencias (`InfrastructureServiceExtensions.cs`) para favorecer el modelo asíncrono puro.

## Registro de Cambios (Changelog)

### [25-07-2026] - Mejoras UX y Corrección de Errores
- **Gestión de Usuarios (Fix):** Se corrigió un error en el panel de administración donde la columna "Registro" mostraba la fecha `01/01/0001`. El problema se debía a que el repositorio de datos (`UsuarioRepository`) omitía extraer la columna `FechaRegistro` del `SqlDataReader` al mapear la entidad, dejando la fecha en su valor por defecto.
- **Navegación (Fix):** Se corrigió un bug en `/Home/Menu` donde el formulario de búsqueda interceptaba el botón de "Cerrar Sesión", impidiendo que los usuarios pudieran salir de su cuenta. Se aplicó un ID específico al formulario en lugar de un selector genérico en JS.
- **Navegación (Auth):** Se agregó un botón directo de "Registrarse" junto al botón de "Ingresar" para mejorar la experiencia de nuevos usuarios.
- **Avatar Dinámico:** Se reemplazó el icono estático de usuario en el menú desplegable (Layout) por un círculo estilizado que extrae dinámicamente la inicial del usuario, con degradado "Nikkei Noir".
- **Botones (UI):** Se actualizó el estado de `hover` del botón secundario (`.btn-outline-accent`) para que ya no tome un color sólido, diferenciándolo visualmente del botón primario (Registrarse/Acción Principal).
- **Checkout (UI):** El selector `<select>` tradicional de "Método de Pago" fue rediseñado y reemplazado por un *Grid* moderno de tarjetas interactuables con efecto "Glow" dorado al ser seleccionadas, mejorando dramáticamente el Checkout.

### [23-07-2026] - Finalización de Vistas de Administración
- **Vistas Faltantes:** Se crearon las vistas `Categorias`, `CrearCategoria`, `EditarCategoria` y `Usuarios` en el panel de Administración. 
- **Estilos de Tablas:** Se implementó una corrección global en el archivo `maido.css` y las vistas Razor para forzar que el fondo de las tablas Bootstrap respete el modo oscuro (dark theme) del diseño.
- **Detalle de Pedidos (Fix):** Se ajustó el mapeo JSON en la base de datos y la función de repositorio en C# para sincronizar las columnas al procesar o ver resúmenes de pedidos, solucionando los errores de `IndexOutOfRangeException` y `NULL insertions`.

### [07-08-2026] - Módulo "Mi Perfil"
- **Gestión de Perfil:** Se implementó una nueva sección "Mi Perfil" que permite a los usuarios autenticados actualizar su Nombre, Apellido, Teléfono y Dirección de entrega predeterminada (su correo es de solo lectura).
- **Backend:** Se creó el procedimiento almacenado `sp_ActualizarPerfilUsuario` y se expandió el ciclo de vida (Repository -> Service -> Controller) con el nuevo `PerfilDto`.
- **UI/UX (Navbar y Perfil):** Se enriqueció el menú desplegable del usuario en el Navbar (`_Layout.cshtml`) separando visualmente las acciones de navegación (Mi Perfil, Mis Pedidos) de la acción destructiva (Cerrar Sesión). El menú principal ahora luce mucho más limpio tras mover "Mis Pedidos" a esta sección. Se diseñó la vista de Perfil con estilo `glassmorphism`.
- **UI/UX (Checkout):** Se rediseñó la página de Confirmación de Pedido (`Checkout.cshtml`) adoptando por completo el estilo *Nikkei Noir*. Se aplicó un diseño de tarjetas de cristal (`glass-card`), formularios oscuros (`bg-dark`), selección de delivery/recojo interactiva mediante tarjetas y un panel de resumen pegadizo (`position-sticky`) con imágenes de los platillos.
- **UI/UX (Cart Index):** Se rediseñó el carrito de compras principal (`Cart/Index.cshtml`) implementando manipulaciones directas del DOM (AJAX) para cambiar cantidades o quitar platos sin recargar la página. Se mejoró el *Empty State* con un ícono brillante y se aplicó la clase `glass-card` y `shadow-glow` a las tarjetas y botones. Además, los platillos sugeridos ahora se mezclan verdaderamente al azar usando `Guid.NewGuid()`.
- **UI/UX (Mis Pedidos):** Se reemplazó la antigua tabla HTML estática del historial de pedidos (`Cliente/MisPedidos.cshtml`) por un diseño de cuadrícula moderna (Grid) basada en `glass-card`. Se implementó un saludo personalizado y badges dinámicos con colores neón según el estado de la orden.
- **UI/UX (Menú Público):** Se modernizó el Hero del menú (`Home/Menu.cshtml`) envolviendo el buscador en un `glass-card` translúcido. Se añadió un efecto de desenfoque (`backdrop-filter: blur`) a la barra de categorías adhesiva (sticky nav) y se suavizaron las animaciones del *Skeleton Loader* para ajustarse al radio de borde del diseño premium.
- **UI/UX (Login & Registro):** Se rediseñaron completamente las páginas de Autenticación (`Account/Login.cshtml` y `Account/Register.cshtml`). Se sustituyeron las tarjetas blancas simples por un diseño inmersivo *Split-Screen* / *Hero* con fotografías premium de fondo oscurecidas, tarjetas `.glass-card` con animación `fade-in-up`, y grupos de inputs integrados visualmente (seamless borders). En el Registro, se añadió el botón para visualizar contraseñas también al campo de "Confirmar contraseña" y se estandarizó la tipografía MAIDO con etiquetas en mayúscula. Se ocultó el footer global en estas vistas para evitar gaps visuales.
- **UI/UX (Admin Platillos):** Se rediseñó la tabla de gestión de platillos (`Admin/Platillos.cshtml`) para alinearla al diseño "Premium Dark". Se implementaron filtros estilo Dashboard Widget dentro de un `glass-card`, reemplazando el tradicional `<select>` por una **barra de chips** (botones cápsula) interactivos para filtrar categorías. Se incluyeron avatares de platillos circulares, botones fantasma (`ghost buttons`) con efectos hover neón para editar/eliminar, e indicadores de estado circulares de alta tecnología (leds verde/rojo).
- **CSS Design System:** Se implementó estrictamente el sistema de diseño "Premium Dark / Nikkei Noir" en `maido.css`, actualizando las variables `:root` (fuentes Outfit, Noto Serif JP, fondos `--bg-primary`, `--bg-secondary`, `--bg-card` y acentos neón). Se rediseñaron globalmente componentes como `.btn-accent`, `.maido-card`, el *Scrollbar*, y `.form-control`. Se restauraron las variables `--text-secondary`, `--bg-elevated`, y `--radius-sm` para asegurar retrocompatibilidad con las descripciones opacas que fallaban en la página principal.
- **Bugfixes de Sesión y Carrito:** Se corrigió el error donde la dirección actualizada del usuario no se reflejaba en su perfil ni autocompletaba el Checkout. Se debió a la falta de mapeo del campo en el DataReader (`UsuarioRepository.cs`) y la falta de inyección al `CheckoutDto` en `CartController.cs`. Además, se solucionó un bug en `CartController.cs` donde el botón de "Proceder al Pago" se bloqueaba erróneamente al eliminar un plato, porque `EliminarItem` no estaba devolviendo los campos `igv` y `total` en el JSON de respuesta.

### [24-07-2026] - Ajustes Visuales
- **Home/Index (Stats):** Se ajustó el fondo de la sección de métricas ("15+ Años de excelencia") para que sea transparente, integrándose perfectamente con el fondo profundo oscuro del resto de la página.
- **Home/Index (Hero Carousel):** Se reemplazó la imagen estática de fondo principal por un carrusel dinámico de 4 imágenes en alta resolución que cambia suavemente (Fade) cada 4 segundos, dándole mucha más vida a la página de inicio.

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

### [06-08-2026] - Checkout, Panel Admin y Reportes en PDF

| Módulo | Cambio |
|---|---|
| Checkout | Precarga el teléfono del usuario logueado (editable) y ya no se pierden los datos del formulario si falla la validación |
| Menú | El botón "Agregar" se oculta cuando el usuario logueado es Administrador |
| Dashboard | La columna "Cliente" ahora muestra el nombre completo en vez de "—" |
| Categorías | Ícono de respaldo si no tiene uno asignado, confirmación de eliminar con SweetAlert2, botón "Editar" en gris, y se agregó `_ValidationScriptsPartial.cshtml` faltante |
| Usuarios | Badge "Admin" con texto blanco (antes ilegible) y confirmaciones de activar/desactivar con SweetAlert2 |
| Reportes | Nuevo botón "Generar" que descarga un PDF ejecutivo (QuestPDF) con indicadores, gráficos y tablas de detalle |