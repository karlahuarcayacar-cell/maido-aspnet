/* AdminCss.md - Global CSS Classes */

/* Maido UI System Classes */
.maido-card {
    background: var(--bg-card);
    border: 1px solid var(--border-subtle);
    border-radius: var(--radius-lg, 12px);
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.maido-table {
    color: var(--text-primary);
    border-collapse: separate;
    border-spacing: 0;
    width: 100%;
}

.maido-table thead th {
    background: rgba(24, 24, 28, 0.8);
    border-bottom: 1px solid var(--border);
    color: var(--text-secondary);
    font-weight: 600;
    text-transform: uppercase;
    padding: 1.25rem 1rem;
}

.maido-table tbody tr {
    transition: all 0.2s ease;
}

.maido-table tbody tr td {
    border-bottom: 1px solid rgba(255, 255, 255, 0.03);
    padding: 1rem;
}

.maido-table tbody tr:hover td {
    background-color: rgba(255, 255, 255, 0.02);
}

.maido-input {
    background-color: var(--bg-input, #1a1a20);
    border: 1px solid var(--border, #2a2a32);
    color: var(--text-primary, #fff);
    border-radius: 8px;
    padding: 0.75rem 1rem;
}

.maido-input:focus {
    background-color: var(--bg-input-focus, #222);
    border-color: var(--accent-gold, #e0a96d);
    box-shadow: 0 0 0 0.25rem rgba(224, 169, 109, 0.25);
    color: var(--text-primary, #fff);
}

.badge-estado {
    padding: 0.35rem 0.65rem;
    border-radius: 6px;
    font-size: 0.75rem;
    font-weight: 600;
    text-transform: uppercase;
}

.badge-activo {
    background: rgba(46, 204, 113, 0.1);
    color: #2ecc71;
    border: 1px solid rgba(46, 204, 113, 0.3);
}

.badge-inactivo {
    background: rgba(217, 56, 30, 0.1);
    color: #d9381e;
    border: 1px solid rgba(217, 56, 30, 0.3);
}

.btn-accent {
    background-color: var(--accent-red, #D9381E);
    color: white;
    border: none;
    border-radius: 8px;
}

.btn-accent:hover {
    background-color: #B22915;
    color: white;
}

.btn-outline-accent {
    background-color: transparent;
    color: var(--accent-red, #D9381E);
    border: 1px solid var(--accent-red, #D9381E);
    border-radius: 8px;
}

.btn-outline-accent:hover {
    background-color: var(--accent-red, #D9381E);
    color: white;
}

.btn-gold {
    background-color: var(--accent-gold, #E0A96D);
    color: #111;
    border: none;
    border-radius: 8px;
}

.btn-gold:hover {
    background-color: #C8955B;
    color: #111;
}
