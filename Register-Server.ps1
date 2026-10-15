# Register-Server.ps1
Write-Host "=== Registro de Servidor DCOM (COM+) ===" -ForegroundColor Cyan

# Rutas de la DLL y la herramienta de registro de .NET Framework
$dllPath = ".\DcomServer\bin\Debug\net48\DcomServer.dll"
$regsvcsPath = "$env:windir\Microsoft.NET\Framework64\v4.0.30319\regsvcs.exe"

# 1. Verificar privilegios de Administrador (Requisito estricto para COM+)
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-Not $isAdmin) {
    Write-Warning "ERROR: Este script DEBE ejecutarse como Administrador."
    Write-Host "Por favor, abre una nueva terminal de PowerShell como Administrador e inténtalo de nuevo." -ForegroundColor Yellow
    exit
}

# 2. Verificar que el servidor esté compilado
if (-Not (Test-Path $dllPath)) {
    Write-Error "No se encontró DcomServer.dll en la ruta esperada ($dllPath)."
    Write-Host "Asegúrate de haber compilado el proyecto DcomServer."
    exit
}

# 3. Ejecutar el registro en COM+
Write-Host "[+] Registrando DcomServer en los Servicios de Componentes (COM+)..."
try {
    # Ejecuta regsvcs para instalar la aplicación COM+
    $process = Start-Process -FilePath $regsvcsPath -ArgumentList "`"$dllPath`"" -Wait -NoNewWindow -PassThru
    
    if ($process.ExitCode -eq 0) {
        Write-Host "[EXITO] Componente registrado correctamente en COM+." -ForegroundColor Green
        Write-Host "Ahora puedes ejecutar el cliente DcomClient para probar la conexión local." -ForegroundColor Green
    } else {
        Write-Error "Fallo el registro en COM+. Código de salida: $($process.ExitCode)"
    }
}
catch {
    Write-Error "Ocurrió un error inesperado al intentar registrar la DLL: $_"
}