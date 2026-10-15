# Sistema Cliente-Servidor basado en DCOM

**Materia:** Programación en Ambiente Cliente/Servidor  
**Tecnología:** .NET Framework 4.8 (C#) / PowerShell / COM+  
**Arquitectura:** Distributed Component Object Model (DCOM) Out-of-Process

## Descripción del Proyecto

Este proyecto universitario implementa una arquitectura distribuida basada en la tecnología DCOM de Microsoft. Demuestra la creación, registro y consumo de componentes COM alojados "Fuera de Proceso" (*Out-of-Process* en `dllhost.exe`), permitiendo la ejecución de métodos remotos y el paso de mensajes entre aplicaciones cliente y servidor a través de una red local.

## Características Principales

*   **Servidor DCOM (DcomServer):** Hereda de `ServicedComponent` para ejecutarse en el catálogo de COM+. Implementa operaciones aritméticas, análisis estadístico, descomposición de factores primos y hashing criptográfico (SHA-256).
*   **Contrato Compartido (DcomShared):** Interfaz `ICalculatorService` firmada criptográficamente (Strong Name Key) con soporte para *Early* y *Late Binding*.
*   **Cliente DCOM (DcomClient):** Aplicación de consola interactiva capaz de instanciar el componente de forma local (Localhost) o remota (mediante dirección IP o nombre de equipo). Cuenta con manejo de excepciones para traducir códigos `HRESULT` de red y seguridad.
*   **Automatización:** Scripts de PowerShell y herramientas del SDK para agilizar el registro en el sistema operativo y en la Caché Global de Ensamblados (GAC).

## Estructura de la Solución

```text
Actividad 4/
├── DcomSolution.sln                  # Solución principal
├── DcomKey.snk                       # Llave criptográfica (Strong Name Key)
├── Register-Server.ps1               # Script de automatización COM+
├── DCOM_CONFIGURATION_GUIDE.md       # Manual de despliegue en red
├── DcomShared/                       # Biblioteca de Contratos e Interfaces
│   └── ICalculatorService.cs
├── DcomServer/                       # Componente Servidor (DLL)
│   └── CalculatorService.cs
└── DcomClient/                       # Aplicación Cliente (EXE)
    └── Program.cs
```

## Requisitos Previos

*   Sistema Operativo Microsoft Windows.
*   .NET Framework 4.8 Developer Pack (SDK).
*   Privilegios de Administrador (requeridos únicamente para el registro del servidor).

## Instrucciones de Despliegue Local

### 1. Compilación
Abre tu consola en la raíz del proyecto y compila la solución completa:
```cmd
dotnet build
```

### 2. Instalación en la GAC
Para que el proceso anfitrión encuentre las librerías, deben registrarse en la Caché Global de Ensamblados. Abre el **Developer Command Prompt for Visual Studio como Administrador** y ejecuta:
```cmd
gacutil /i DcomServer\bin\Debug\net48\DcomShared.dll
gacutil /i DcomServer\bin\Debug\net48\DcomServer.dll
```

### 3. Registro en Servicios de Componentes (COM+)
Abre **PowerShell como Administrador** en la raíz del proyecto y ejecuta el script de registro:
```powershell
.\Register-Server.ps1
```

### 4. Ejecución del Cliente
Abre una terminal normal, navega a la carpeta del cliente y ejecútalo:
```cmd
cd DcomClient
dotnet run
```
*Deja la IP en blanco y presiona Enter para realizar la prueba en el entorno local (localhost).*

## Despliegue Remoto (Red)

Para invocar el componente desde otra computadora a través de la red, es necesario configurar permisos específicos en `dcomcnfg` y abrir el puerto TCP 135 en el Firewall de Windows del equipo servidor.

Por favor, consulta el archivo **[DCOM_CONFIGURATION_GUIDE.md](DCOM_CONFIGURATION_GUIDE.md)** incluido en este repositorio para ver el tutorial paso a paso.