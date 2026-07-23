# Guía de Diseño Visual y Estilos (Maido) para ASP.NET C#

Este documento describe a profundidad como debe ser la arquitectura visual, los estilos, el diseño de interfaz y la estructura de componentes del proyecto 

---

## 1. Concepto Visual y Temática

El proyecto utiliza un sistema de diseño propio altamente estilizado inspirado en la **alta cocina Nikkei (Fusión Peruano-Japonesa)**. El concepto transmite elegancia, sofisticación y modernidad.

*   **Tema Principal:** Modo oscuro (Dark Mode) exclusivo. No hay modo claro, ya que los fondos oscuros resaltan los colores vibrantes de los platillos.
*   **Sensación (Feel):** Premium, minimalista, con micro-animaciones suaves y efectos de "glow" (resplandor) sutiles.

---

## 2. Paleta de Colores (Paleta Nikkei)

El diseño se basa en tres pilares de color, definidos a través de variables CSS:

*   **Fondos (Negro Carbón):**
    *   Fondo principal (`--bg-primary`): `#0F0F11` (Negro muy profundo).
    *   Fondo secundario (`--bg-secondary`): `#18181C` (Ligeramente más claro, para paneles laterales o inputs).
    *   Fondo de Tarjetas (`--bg-card`): `#1E1E24` (Gris oscuro para resaltar sobre el fondo principal).
*   **Acentos:**
    *   **Rojo Carmesí (`--accent-red`):** `#D9381E`. Usado para llamadas a la acción principales (botones de compra, iconos importantes). Representa la pasión y elementos japoneses.
    *   **Dorado Bronce (`--accent-gold`):** `#E0A96D`. Usado para detalles de lujo, precios, y botones secundarios.
*   **Texto:**
    *   Principal (`--text-primary`): `#F4F4F6` (Blanco hueso/grisáceo suave para no cansar la vista).
    *   Secundario/Muted (`--text-muted`): `#A0A0A8`.
    *   Tenue (`--text-dim`): `#606068`.
*   **Bordes:** `#2a2a32` (Líneas sutiles para separar elementos sin ser intrusivas).

---

## 3. Tipografía

El diseño hace uso de Google Fonts combinando dos fuentes para lograr el aspecto moderno pero tradicional:

1.  **Fuente de Cuerpo (Body):** `'Outfit', sans-serif`. Es geométrica, limpia y muy legible. Se usa para descripciones, botones, inputs y navegación.
2.  **Fuente de Exhibición (Display):** `'Noto Serif JP', serif`. Se utiliza específicamente para títulos principales o elementos destacados para darle ese toque tradicional japonés.

---

## 4. Sistema de Diseño (UI Kit)

### Botones (`.btn`)
Tienen un borde redondeado sutil (`6px`). 
*   **Primario (`.btn-primary`):** Fondo rojo carmesí. Al hacer *hover*, se eleva ligeramente (`transform: translateY(-2px)`) y emite un resplandor rojo (`box-shadow: 0 6px 20px rgba(217,56,30,0.4)`).
*   **Secundario (`.btn-secondary`):** Transparente con borde dorado. Al hacer *hover*, el fondo toma un tono dorado casi transparente.
*   **Fantasma (`.btn-ghost`):** Botones grises translúcidos para acciones menos importantes.

### Tarjetas (`.card`)
Usadas intensivamente en el catálogo de platillos.
*   Fondo `#1E1E24`, bordes finos. 
*   **Interacción:** Al pasar el ratón, el borde se vuelve ligeramente rojo, la tarjeta se eleva (`-4px`) y adquiere una sombra profunda (`--shadow-card`).

### Formularios (`.form-input`)
Diseño de campos de texto oscuro (fondo `#18181C`). Cuando el usuario hace clic (estado `:focus`), el borde se vuelve rojo y aparece un anillo rojo alrededor (`box-shadow: 0 0 0 3px rgba(217,56,30,0.15)`).

### Otros Elementos Visuales
*   **Badges:** Etiquetas redondeadas (`border-radius: 99px`) con fondos translúcidos para estados (éxito, error, advertencia) o categorías.
*   **Línea Decorativa (`.accent-line`):** Una barra horizontal delgada con un gradiente de rojo a dorado, usada debajo de los títulos de sección.
*   **Efecto Blur (Glassmorphism):** Usado en fondos modales (`backdrop-filter: blur(4px)`) para oscurecer el fondo sin perder el contexto.

---

## 5. Diseño de Páginas (Estructura)

### A. Lado Público (Para Clientes)
Layout tradicional centrado.
*   **Navegación:** Header superior.
*   **Páginas Principales:**
    *   **Home/Catálogo:** Utiliza clases de cuadrícula (`.grid-3`, `.grid-4`) para mostrar las tarjetas de platillos.
    *   **Detalle de Platillo:** Suele usar un diseño de 2 columnas (`.grid-2`).
    *   **Carrito/Checkout:** Listados de tabla (`.table-wrapper`) o listas con resumen de compra a la derecha.
    *   **Login/Registro:** Formularios centrados en tarjetas sobre un fondo oscuro, limpios y sin distracciones.

### B. Lado Administrativo (Para el Restaurante)
Layout de Dashboard.
*   **Estructura (`.admin-layout`):** Un diseño a pantalla completa.
*   **Sidebar (`.sidebar`):** Panel lateral izquierdo fijo de 250px. Enlaces de navegación que al hacer *hover* o estar activos se pintan con un fondo rojo translúcido y texto rojo.
*   **Contenido (`.admin-content`):** Área derecha principal donde se ubican las tablas de Pedidos, Gestión de Platillos y Reportes.
