# 🍱 MAIDO — Restaurante & Experiencia Gastronómica Nikkei

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue?style=for-the-badge&logo=microsoft)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-Pure%20Custom%20Design-1572B6?style=for-the-badge&logo=css3)

> Sistema web integral de gestión gastronómica y pedidos online para el restaurante **Maido**, desarrollado con arquitectura en capas (N-Tier) sobre **ASP.NET Core 10.0 MVC** y un sistema de diseño customizado en **CSS Puro** con estética oscura y glassmorphism.

---

## 🚀 Características Principales

### 🍽️ Vista Pública y Clientes
- **Hero & Menú Interactivo**: Exploración de catálogo con filtro instantáneo por categorías y buscador inteligente.
- **Carrito de Compras AJAX**: Gestión en tiempo real de ítems, cantidades (controles tipo cápsula) y desglose transparente de precios con cálculo automático de **Subtotal e IGV (18%)**.
- **Checkout & Métodos de Pago**: Proceso de compra con tarjeta simulación glassmorphism (Visa/Mastercard), POS y efectivo al entregar.
- **Autenticación**: Registro de clientes e Inicio de Sesión con validaciones integradas.
- **Mi Perfil y Mis Pedidos**: Seguimiento de estado de órdenes activas (Pendiente, En Preparación, En Camino, Entregado, Cancelado) e historial detallado.

### 🛡️ Panel de Administración (`/Admin`)
- **Dashboard Analítico**: Métricas principales, resumen de ventas diarias y estado general del sistema.
- **Gestión de Pedidos en Tiempo Real**: Cambio de estado directo desde la lista de órdenes (*Aceptar/Cocinar*, *Enviar Moto*, *Entregado*, *Cancelar*) y vista extendida de detalle.
- **Gestión de Platillos y Categorías**: Operaciones CRUD completas con modales de confirmación interactivos de **SweetAlert2**.
- **Gestión de Usuarios**: Cambio de estado de cuentas (Activo/Inactivo) y control de roles.
- **Reportes Gastronómicos**: Estadísticas y balances filtrados por fechas.

---

## 🛠️ Tecnologías y Arquitectura

El proyecto sigue una arquitectura limpia orientada a dominio (Clean Architecture / N-Tier):

```text
Maido Solution/
├── Maido.Domain/          # Entidades de Dominio (Usuario, Platillo, Categoria, Pedido, DetallePedido)
├── Maido.Application/     # DTOs, Interfaces y Servicios de Lógica de Negocio
├── Maido.Infrastructure/  # Acceso a Datos y Repositorios con ADO.NET / Dapper
└── Maido.PLGUI/           # Presentación (Controladores MVC, Vistas Razor, CSS Puro y Scripts AJAX)
```

- **Framework**: .NET 10.0 (C# 13)
- **Base de Datos**: Microsoft SQL Server
- **Estilos y UI**: CSS3 Puro sin frameworks externos (Variables CSS, Flexbox, Grid, Glassmorphism, Micro-animaciones).
- **Librerías Complementarias**: SweetAlert2 para modales y notificaciones flotantes.

---

## 📋 Requisitos Previos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server/) (LocalDB, Express o Enterprise)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) o Azure Data Studio.

---

## ⚡ Instalación y Configuración

1. **Clonar el Repositorio**:
   ```bash
   git clone https://github.com/karlahuarcayacar-cell/maido-aspnet.git
   cd maido-aspnet
   ```

2. **Base de Datos**:
   - Ejecuta el script SQL `maido_db.sql` ubicado en la raíz del proyecto dentro de SQL Server para crear la base de datos `maido_db` con sus tablas y datos semilla.

3. **Configurar la Cadena de Conexión**:
   - Abre `Maido.PLGUI/appsettings.json` y actualiza `ConnectionStrings:DefaultConnection` según las credenciales de tu servidor SQL local:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=maido_db;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

4. **Compilar y Ejecutar**:
   ```bash
   cd Maido.PLGUI
   dotnet run
   ```
   Abre tu navegador en `http://localhost:5046`.

---

## 🔑 Credenciales de Prueba

| Rol | Correo Electrónico | Contraseña |
|---|---|---|
| **Administrador** | `admin@maido.pe` | `admin123` |
| **Cliente** | `cliente@gmail.com` | `cliente123` |

---

## 📄 Licencia

Desarrollado para la asignatura **Desarrollo de Servicios Web I**. Todos los derechos reservados.
