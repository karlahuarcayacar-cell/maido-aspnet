$dir = "C:\Users\antwn\Desktop\WS DSWI\Maido\Maido.PLGUI\Views\Admin"

$files = Get-ChildItem -Path $dir -Filter "*.cshtml" -File

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw

    # Tables
    $content = $content -replace '<table class="table table-dark table-hover mb-0" style="background-color: transparent;">', '<table class="table maido-table table-hover mb-0">'
    $content = $content -replace '<table class="table table-hover mb-0" style="color:var\(--text-primary\);">', '<table class="table maido-table table-hover mb-0">'
    $content = $content -replace '<table class="table(?! maido-table)', '<table class="table maido-table'
    $content = $content -replace 'class="maido-table">\s*<table class="table maido-table', 'class="table-responsive">\s*<table class="table maido-table'
    
    # Cards
    $content = $content -replace '<div class="card(?! maido-card)', '<div class="card maido-card'

    # Inputs
    $content = $content -replace 'class="form-control(?! maido-input)', 'class="form-control maido-input'
    
    # Buttons
    $content = $content -replace 'btn-primary', 'btn-accent'
    $content = $content -replace 'btn-outline-primary', 'btn-outline-accent'
    $content = $content -replace 'btn-warning', 'btn-gold'

    # Badges
    $content = $content -replace 'class="badge bg-success bg-opacity-10 text-success border border-success border-opacity-25"', 'class="badge-estado badge-activo"'
    $content = $content -replace 'class="badge bg-danger bg-opacity-10 text-danger border border-danger border-opacity-25"', 'class="badge-estado badge-inactivo"'

    Set-Content -Path $file.FullName -Value $content
}
