# Roda os testes E2E do LabPat (cria o ambiente na primeira vez).
# Uso, na pasta tests:  .\rodar.ps1          (ou .\rodar.ps1 -k "parte_5" para filtrar)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
if (-not (Test-Path .venv)) {
    python -m venv .venv
    .\.venv\Scripts\python -m pip install -r requirements.txt
    .\.venv\Scripts\python -m playwright install chromium
}
.\.venv\Scripts\python -m pytest @args
