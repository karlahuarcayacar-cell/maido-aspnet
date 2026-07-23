/**
 * carrito.js — Lógica del carrito de compras AJAX
 * Maido Restaurante | Desarrollo de Servicios Web I
 */

document.addEventListener('DOMContentLoaded', () => {
    actualizarCarritoUI();
});

// ─────────────────────────────────────────────────────
// Agregar al carrito
// ─────────────────────────────────────────────────────
async function agregarAlCarrito(idPlatillo, cantidad = 1) {
    try {
        const res = await fetch('/Cart/AgregarItem', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ idPlatillo, cantidad })
        });
        const data = await res.json();
        if (data.success) {
            actualizarBadge(data.totalItems);
            mostrarToast('¡Agregado al pedido!', 'success');
            actualizarCarritoUI();
            
            // Si estamos en la página del carrito, recargar para mostrar el nuevo item
            if (window.location.pathname.toLowerCase() === '/cart' || window.location.pathname.toLowerCase() === '/cart/index') {
                setTimeout(() => location.reload(), 500);
            }
        }
    } catch (e) {
        mostrarToast('Error al agregar el producto.', 'error');
    }
}

// ─────────────────────────────────────────────────────
// Actualizar cantidad
// ─────────────────────────────────────────────────────
async function actualizarCantidad(idPlatillo, cantidad) {
    try {
        const res = await fetch('/Cart/ActualizarCantidad', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ idPlatillo, cantidad })
        });
        const data = await res.json();
        if (data.success) {
            actualizarBadge(data.totalItems);
            actualizarResumenCarrito(data.subtotal, data.igv, data.total);
            if (cantidad <= 0) actualizarCarritoUI();
        }
    } catch (e) {
        console.error('Error actualizando cantidad:', e);
    }
}

// ─────────────────────────────────────────────────────
// Eliminar item
// ─────────────────────────────────────────────────────
async function eliminarDelCarrito(idPlatillo) {
    try {
        const res = await fetch('/Cart/EliminarItem', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ idPlatillo })
        });
        const data = await res.json();
        if (data.success) {
            actualizarBadge(data.totalItems);
            actualizarCarritoUI();
        }
    } catch (e) {
        console.error('Error eliminando item:', e);
    }
}

// ─────────────────────────────────────────────────────
// Actualizar UI del carrito (badge global)
// ─────────────────────────────────────────────────────
async function actualizarCarritoUI() {
    try {
        const res  = await fetch('/Cart/ObtenerCarrito');
        const data = await res.json();
        actualizarBadge(data.items.reduce((s, i) => s + i.cantidad, 0));
    } catch (e) {
        console.error('Error cargando carrito:', e);
    }
}

// ─────────────────────────────────────────────────────
// Helpers de UI
// ─────────────────────────────────────────────────────
function actualizarBadge(total) {
    const badges = document.querySelectorAll('.cart-badge');
    badges.forEach(b => {
        b.textContent = total;
        if (total > 0) {
            b.classList.add('pulse');
            setTimeout(() => b.classList.remove('pulse'), 300);
        }
    });
}

function actualizarResumenCarrito(subtotal, igv, total) {
    const fmt = n => `S/ ${Number(n).toFixed(2)}`;
    const s = document.getElementById('carritoSubtotal');
    const g = document.getElementById('carritoIGV');
    const t = document.getElementById('carritoTotal');
    if (s) s.textContent = fmt(subtotal);
    if (g) g.textContent = fmt(igv);
    if (t) t.textContent = fmt(total);
}

function mostrarToast(mensaje, tipo = 'success') {
    const Toast = Swal.mixin({
        toast: true,
        position: 'bottom-end',
        showConfirmButton: false,
        timer: 2500,
        timerProgressBar: true,
        background: tipo === 'success' ? 'rgba(46,125,50,0.95)' : 'rgba(211,47,47,0.95)',
        color: '#fff',
        iconColor: '#fff'
    });
    Toast.fire({ icon: tipo, title: mensaje });
}

// ─────────────────────────────────────────────────────
// Confirmación SweetAlert para eliminar (admin)
// ─────────────────────────────────────────────────────
function confirmarEliminacion(formId) {
    Swal.fire({
        title: '¿Confirmar eliminación?',
        text: 'Esta acción no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#D9381E',
        cancelButtonColor: '#333',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar',
        background: '#18181C',
        color: '#F4F4F6'
    }).then(result => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
}

function confirmarEstado(formId, accion) {
    Swal.fire({
        title: `¿${accion}?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#D9381E',
        cancelButtonColor: '#333',
        confirmButtonText: 'Confirmar',
        cancelButtonText: 'Cancelar',
        background: '#18181C',
        color: '#F4F4F6'
    }).then(result => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
}
