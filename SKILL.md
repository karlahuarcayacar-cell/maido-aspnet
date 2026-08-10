---
name: aspnet-ui-polisher
description: Audita y mejora visualmente todas las vistas de una aplicación ASP.NET, corrigiendo inconsistencias de UI/UX, estilos, responsive design y componentes sin alterar la lógica de negocio.
---

# ASP.NET UI Polisher

## Objetivo

Actúa como un especialista senior en UI/UX y frontend para aplicaciones ASP.NET.

Tu trabajo consiste en revisar sistemáticamente toda la interfaz visual del proyecto y mejorarla para que tenga una apariencia:

- consistente
- profesional
- moderna
- limpia
- responsive
- accesible
- coherente entre todas las vistas

No debes limitarte a corregir una sola vista. Debes analizar el proyecto completo y detectar patrones visuales inconsistentes.

---

## 1. Inspección inicial obligatoria

Antes de modificar código:

1. Identifica la estructura del proyecto.
2. Localiza todas las vistas Razor:
   - `.cshtml`
3. Identifica:
   - `_Layout.cshtml`
   - `_ViewStart.cshtml`
   - `_ViewImports.cshtml`
   - partial views
   - componentes reutilizables
4. Localiza:
   - CSS
   - JavaScript
   - Bootstrap
   - Tailwind
   - librerías de iconos
   - fuentes
5. Identifica los controladores y modelos relacionados con las vistas.
6. Determina qué sistema visual utiliza actualmente el proyecto.

No introduzcas una nueva librería CSS si ya existe una solución funcional en el proyecto.

---

# 2. Auditoría visual global

Revisa todas las vistas y busca inconsistencias en:

### Colores

Verifica que:

- Los colores principales sean consistentes.
- Los botones utilicen una misma jerarquía.
- Los estados `success`, `warning`, `danger` e `info` sean coherentes.
- No existan colores arbitrarios utilizados únicamente en una vista.
- Los enlaces tengan un comportamiento visual consistente.

Si existe un sistema de colores, reutilízalo.

---

### Tipografía

Revisa:

- familia tipográfica
- tamaños
- pesos
- títulos
- subtítulos
- texto normal
- texto secundario
- labels
- mensajes de error

Establece una jerarquía clara.

Ejemplo conceptual:

```text
Título de página
  ↓
Subtítulo
  ↓
Contenido
  ↓
Texto secundario
```

Evita que cada vista utilice tamaños diferentes para elementos equivalentes.

---

# 3. Espaciado

Busca inconsistencias de:

- `margin`
- `padding`
- `gap`
- separación entre formularios
- separación entre cards
- separación entre secciones
- espacios alrededor de tablas
- espacios alrededor de botones

Utiliza un sistema de espaciado consistente.

Evita valores arbitrarios repetidos cuando pueda utilizarse una clase reutilizable.

---

# 4. Componentes

Identifica componentes repetidos y asegúrate de que tengan la misma apariencia.

Especialmente:

- botones
- inputs
- selects
- textareas
- tablas
- cards
- badges
- alertas
- modales
- paginación
- breadcrumbs
- navbar
- sidebar
- formularios
- títulos de sección

Por ejemplo, todos los botones principales deberían compartir:

- altura
- border-radius
- tipografía
- padding
- iconografía
- comportamiento hover
- comportamiento focus

---

# 5. Formularios

Revisa todos los formularios.

Verifica:

- alineación de labels
- tamaño de inputs
- separación vertical
- selects
- checkbox
- radio buttons
- mensajes de validación
- campos obligatorios
- botones de acción
- estados `focus`
- estados `disabled`

Los formularios deben sentirse como parte del mismo sistema de diseño.

No cambies los nombres de los campos ni los bindings Razor.

Conserva:

```razor
asp-for
asp-action
asp-controller
asp-route-*
name
id
value
```

salvo que exista un problema real que requiera corregirlos.

---

# 6. Tablas

Audita todas las tablas.

Mejora:

- encabezados
- alineación
- padding
- separación
- contraste
- hover
- botones de acciones
- badges
- responsive design

Las tablas deben funcionar correctamente en pantallas pequeñas.

Si una tabla es demasiado ancha, utiliza un contenedor responsive en lugar de romper el layout.

---

# 7. Cards y paneles

Las cards deben compartir:

- border-radius
- sombra
- padding
- borde
- encabezado
- separación interna

Evita mezclar estilos como:

```text
card cuadrada
card redondeada
card con sombra fuerte
card sin sombra
```

sin una razón funcional.

---

# 8. Navegación

Revisa:

- navbar
- sidebar
- menú móvil
- enlaces activos
- dropdowns
- breadcrumbs

El usuario debe poder identificar claramente:

- dónde está
- qué sección está utilizando
- cómo regresar
- qué acción es principal

---

# 9. Responsive design

Comprueba las vistas en:

### Desktop

- 1920px
- 1440px
- 1366px

### Tablet

- aproximadamente 768px

### Mobile

- aproximadamente 390px
- aproximadamente 360px

Busca:

- overflow horizontal
- botones que se salen del contenedor
- tablas demasiado anchas
- textos cortados
- formularios deformados
- navbar rota
- cards desbordadas
- imágenes sin adaptación

Corrige utilizando el sistema CSS existente.

---

# 10. Estados de interfaz

Busca que las vistas manejen correctamente:

### Estado normal

Contenido disponible.

### Estado vacío

Cuando no existen registros.

Debe existir un mensaje amigable en lugar de simplemente mostrar una tabla vacía.

### Error

Los errores deben mostrarse de forma clara y consistente.

### Validación

Los errores de validación deben aparecer cerca del campo correspondiente.

### Confirmación

Las operaciones importantes deberían proporcionar feedback visual.

---

# 11. Accesibilidad

Revisa:

- contraste
- labels asociados a inputs
- `alt` en imágenes
- navegación mediante teclado
- focus visible
- botones correctamente definidos
- enlaces claramente identificables
- jerarquía de headings

No elimines atributos de accesibilidad existentes.

---

# 12. Iconografía

Si el proyecto utiliza una librería de iconos existente, reutilízala.

No mezcles innecesariamente:

```text
Font Awesome
Bootstrap Icons
Material Icons
SVG personalizados
emoji
```

Elige el sistema ya utilizado por el proyecto y mantenlo consistente.

---

# 13. CSS

Antes de crear CSS nuevo:

1. Busca estilos existentes.
2. Reutiliza clases existentes.
3. Detecta reglas duplicadas.
4. Detecta estilos contradictorios.
5. Consolida estilos cuando sea seguro hacerlo.

Evita llenar las vistas con:

```html
style="..."
```

si el estilo puede pertenecer al stylesheet.

No introduzcas `!important` salvo que sea estrictamente necesario.

---

# 14. Razor y lógica

La mejora visual NO debe romper la funcionalidad.

Conserva:

- Razor syntax
- Model binding
- `asp-for`
- `asp-controller`
- `asp-action`
- `asp-route-*`
- formularios
- validaciones
- endpoints
- partial views
- ViewData
- ViewBag
- TempData

No modifiques controladores, servicios o modelos únicamente por razones estéticas.

Si detectas un problema funcional durante la auditoría, sepáralo de las mejoras visuales.

---

# 15. Consistencia entre vistas

Después de revisar individualmente cada vista, realiza una segunda revisión global.

Compara vistas equivalentes.

Por ejemplo:

```text
Lista de alumnos
Lista de docentes
Lista de cursos
Lista de usuarios
```

Si todas son CRUD, deberían compartir un patrón visual similar.

Ejemplo:

```text
[Título]

[Descripción]

[+ Nuevo registro]

┌─────────────────────────────┐
│ Tabla                       │
│                             │
└─────────────────────────────┘
```

No permitas que cada CRUD tenga una estructura visual completamente diferente sin una razón.

---

# 16. Priorización

Clasifica los problemas encontrados:

### CRÍTICO

- elementos que se superponen
- contenido ilegible
- responsive roto
- botones inaccesibles
- formularios inutilizables

### ALTO

- inconsistencias importantes
- layouts desalineados
- componentes visualmente contradictorios
- mala jerarquía visual

### MEDIO

- spacing inconsistente
- tamaños diferentes
- pequeños problemas de alineación

### BAJO

- detalles cosméticos
- pequeñas mejoras de UX
- microinteracciones

Corrige primero los problemas críticos y altos.

---

# 17. Reglas de diseño

Prioriza:

- simplicidad
- consistencia
- legibilidad
- jerarquía visual
- espacio en blanco
- contraste
- feedback visual
- responsive design

Evita:

- exceso de sombras
- exceso de colores
- gradientes innecesarios
- animaciones excesivas
- bordes exagerados
- botones gigantes
- interfaces sobrecargadas
- estilos diferentes para elementos equivalentes

---

# 18. Proceso de modificación

Trabaja en este orden:

```text
1. Inspeccionar proyecto
        ↓
2. Identificar sistema visual
        ↓
3. Revisar Layout
        ↓
4. Revisar componentes globales
        ↓
5. Revisar vistas
        ↓
6. Detectar inconsistencias
        ↓
7. Crear/ajustar estilos reutilizables
        ↓
8. Aplicar mejoras
        ↓
9. Revisar responsive
        ↓
10. Revisar consistencia global
        ↓
11. Verificar que la lógica siga intacta
```

No hagas cambios aleatorios vista por vista sin considerar el diseño global.

---

# 19. Criterio de calidad

Una vista puede considerarse terminada cuando:

- tiene una jerarquía visual clara
- utiliza correctamente el sistema de diseño existente
- sus componentes son consistentes
- funciona en desktop y móvil
- no presenta overflow inesperado
- los formularios son claros
- los botones tienen una jerarquía adecuada
- los estados vacíos y errores son comprensibles
- no contiene estilos innecesariamente duplicados
- mantiene intacta su funcionalidad Razor

---

# 20. Resultado final

Al terminar:

1. Verifica qué archivos fueron modificados.
2. Comprueba que no se hayan eliminado funcionalidades.
3. Comprueba que no existan errores Razor evidentes.
4. Comprueba que las rutas y formularios sigan funcionando.
5. Resume los cambios realizados.

Entrega un informe final con:

```text
## UI Audit

### Cambios globales
- ...

### Vistas mejoradas
- ...

### Componentes unificados
- ...

### Responsive
- ...

### Problemas detectados
- ...

### Archivos modificados
- ...

### Recomendaciones futuras
- ...
```

## Regla principal

**No diseñes cada vista como si fuera una aplicación diferente.**

La aplicación debe sentirse como un único producto coherente.

Primero comprende el diseño existente, después identifica sus inconsistencias y finalmente mejóralo de forma sistemática.