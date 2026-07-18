# Installe et lance Bouchon Universel (binaire autonome, sans .NET installé).
#   irm https://raw.githubusercontent.com/74nu5/bouchon-universel/master/install.ps1 | iex
$ErrorActionPreference = 'Stop'

$repo = '74nu5/bouchon-universel'
$rid = 'win-x64'
$dir = if ($env:BOUCHON_DIR) { $env:BOUCHON_DIR } else { Join-Path $env:LOCALAPPDATA 'bouchon-universel' }

New-Item -ItemType Directory -Force -Path (Join-Path $dir 'data/files') | Out-Null

$url = "https://github.com/$repo/releases/latest/download/bouchon-universel-$rid.tar.gz"
$archive = Join-Path $dir 'bouchon.tar.gz'

Write-Host "Téléchargement de $url ..."
Invoke-WebRequest -Uri $url -OutFile $archive
tar -xzf $archive -C $dir
Remove-Item $archive -Force

$env:ASPNETCORE_URLS = 'http://+:8080'
$env:ASPNETCORE_ENVIRONMENT = 'Production'
$env:ConnectionStrings__BDDConnection = (Join-Path $dir 'data/BouchonUniversel.db')
$env:Bouchon__CheminFichiers = (Join-Path $dir 'data/files')

Write-Host 'Lancement de Bouchon Universel sur http://localhost:8080 (Ctrl+C pour arrêter)'
& (Join-Path $dir 'BouchonUniversel.exe')
