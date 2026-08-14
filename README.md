<div align="center">

# 🍣 MAIDO

### Sistema Web de Pedidos Online — Restaurante Nikkei

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue?style=for-the-badge&logo=microsoft)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-Pure%20Custom%20Design-1572B6?style=for-the-badge&logo=css3)

> Sistema web integral de gestión gastronómica y pedidos online para el restaurante **Maido**, desarrollado bajo arquitectura en capas sobre **ASP.NET Core 10.0 MVC**, **SQL Server** con **Transacciones JSON (OPENJSON)** y un sistema de diseño customizado en **CSS Puro** con estética oscura *Nikkei Noir*.

## 🚀 Características Principales

### 🍽️ Vista Pública y Clientes
- **Hero & Menú Interactivo**: Exploración de catálogo con filtro instantáneo por categorías y buscador inteligente en tiempo real (AJAX).
- **Carrito de Compras AJAX**: Gestión en tiempo real de ítems, cantidades y desglose transparente de precios con cálculo automático de **Subtotal e IGV (18%)**.
- **Checkout & Métodos de Pago**: Proceso de compra atómico con simulación de tarjeta glassmorphism (Visa/Mastercard), POS y efectivo al entregar.
- **Autenticación**: Registro de clientes e Inicio de Sesión seguro.
- **Mi Perfil y Mis Pedidos**: Seguimiento de estado de órdenes activas (*Pendiente*, *En Preparación*, *En Camino*, *Entregado*, *Cancelado*) e historial de compras.

### 🛡️ Panel de Administración (`/Admin`)
- **Dashboard Analítico & KPIs en Tiempo Real**: Métricas operativas en vivo (*Ingresos del día, Ingresos históricos, Pedidos activos, Platillos agotados con alerta roja*).
- **Gestión de Pedidos & Historial por Cliente**: Cambio de estado de órdenes en vivo y filtrado de pedidos por cliente específico (`idUsuario`).
- **Gestión de Platillos y Categorías con Toggle Switches**: Control de stock e inhabilitación con 1-clic directo desde las tablas, borrado lógico de seguridad (*Soft Delete*) y ordenamiento.
- **Gestión de Usuarios**: Buscador instantáneo client-side, chips por rol/estado, *toggle switches* de acceso y protección de cuenta administrativa.
- **Reportes Gastronómicos en PDF**: Generación y exportación de balances ejecutivos por rango de fechas mediante **QuestPDF**.

---

## 🛠️ Tecnologías y Arquitectura

El proyecto sigue una arquitectura en capas bien definida (N-Tier / Clean Architecture):

```text
Maido Solution/
├── Maido.Domain/          # Entidades de Dominio e Interfaces de Repositorio
├── Maido.Application/     # DTOs, Interfaces de Servicio y Lógica de Negocio
├── Maido.Infrastructure/  # Acceso a Datos (ADO.NET, SqlCommand, DbConnectionFactory)
└── Maido.PLGUI/           # Presentación (Controladores MVC, Vistas Razor, CSS Puro y AJAX)
```

- **Framework**: .NET 10.0 (C# 13)
- **Base de Datos**: Microsoft SQL Server (LocalDB / Express / Enterprise)
- **Acceso a Datos**: ADO.NET con Stored Procedures y `OPENJSON` para transacciones atómicas.
- **Estilos y UI**: CSS3 Custom Design System (*Nikkei Noir*, Glassmorphism, Micro-animaciones).
- **Librerías Complementarias**: QuestPDF para reportes y SweetAlert2 para modales interactivos.

---

## 📋 Requisitos Previos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server/) (LocalDB, Express o Server)
- SQL Server Management Studio (SSMS) o Azure Data Studio.

---

## ⚡ Instalación y Configuración

1. **Clonar el Repositorio**:
   ```bash
   git clone https://github.com/karlahuarcayacar-cell/maido-aspnet.git
   cd maido-aspnet
   ```

2. **Base de Datos**:
   - Ejecuta el script SQL `maido_db.sql` dentro de tu instancia de SQL Server para crear la base de datos `maido_db` con sus procedimientos almacenados y datos iniciales.

3. **Configurar Cadena de Conexión**:
   - Revisa `Maido.PLGUI/appsettings.json` y asegura que `ConnectionStrings:maido_db` apunte a tu servidor local.

4. **Compilar y Ejecutar**:
   ```bash
   dotnet run --project Maido.PLGUI/Maido.PLGUI.csproj
   ```
   Abre tu navegador en `http://localhost:5046`.

---

## 🔑 Credenciales de Prueba

| Rol | Correo Electrónico | Contraseña |
|---|---|---|
| **Administrador** | `admin@maido.pe` | `admin` |
| **Cliente** | `cliente@gmail.com` | `cliente123` |

---

## 📄 Licencia

Desarrollado by Capibara HDP. Todos los derechos reservados.
