# Guía de Configuración DCOM (Distributed Component Object Model)

Este documento detalla los pasos necesarios para configurar el sistema operativo Microsoft Windows y permitir la ejecución y comunicación remota del componente `DcomServerApp` a través de una red local.

## 1. Configuración de Servicios de Componentes (`dcomcnfg`)

Para que un cliente remoto pueda instanciar el componente, es necesario otorgar los permisos de lanzamiento y acceso a través de la herramienta de administración de DCOM.

1. Presiona `Win + R`, escribe `dcomcnfg` y presiona **Enter**.
2. En el panel izquierdo, navega por la siguiente ruta:
   `Servicios de componentes` > `Equipos` > `Mi PC` > `Configuración DCOM`.
3. En la lista de aplicaciones de la derecha, busca **DcomServerApp** (el nombre que asignamos mediante el atributo `[assembly: ApplicationName]`).
4. Haz clic derecho sobre **DcomServerApp** y selecciona **Propiedades**.

### 1.1 Permisos de Seguridad
1. Dentro de la ventana de Propiedades, dirígete a la pestaña **Seguridad**.
2. En la sección **Permisos de inicio y activación**, selecciona **Personalizar** y haz clic en **Editar**.
3. Agrega el grupo **Todos** (o "ANONYMOUS LOGON" si estás en un entorno de grupo de trabajo sin dominio) y marca las casillas de **Permitir** para:
   - Ejecución local
   - Ejecución remota
   - Activación local
   - Activación remota
4. Repite el mismo proceso en la sección **Permisos de acceso**, seleccionando **Personalizar**, haciendo clic en **Editar**, y otorgando permisos de **Permitir** a **Todos** (Acceso local y Acceso remoto).

### 1.2 Configuración de Identidad
Es crucial definir bajo qué cuenta de usuario de Windows se ejecutará el proceso anfitrión (`dllhost.exe`) cuando reciba una petición remota.

1. En la misma ventana de Propiedades, ve a la pestaña **Identidad**.
2. Selecciona la opción **Este usuario**.
3. Ingresa el nombre de usuario y la contraseña de una cuenta de administrador válida en la máquina servidor (por ejemplo, tu usuario actual).
4. Haz clic en **Aplicar** y **Aceptar**.

## 2. Configuración del Firewall de Windows

DCOM utiliza llamadas a procedimientos remotos (RPC). Esto requiere abrir el puerto TCP 135 (RPC Endpoint Mapper) para la negociación inicial.

Abre una consola de **PowerShell como Administrador** y ejecuta el siguiente comando para crear la regla de entrada:

```powershell
New-NetFirewallRule -DisplayName "Permitir DCOM (TCP 135)" -Direction Inbound -LocalPort 135 -Protocol TCP -Action Allow
```

*Nota: Dependiendo de la estrictez de la red, DCOM también negocia puertos dinámicos altos (rango 49152-65535). Si el puerto 135 está abierto pero la conexión se cuelga, asegúrate de que tu perfil de red esté configurado como "Privado" o permite el binario `dllhost.exe` en el Firewall.*

## 3. Solución de Problemas Comunes (HRESULT)

Si el cliente remoto experimenta errores al intentar conectar, revisa los siguientes códigos de error:

- **0x80070005 (E_ACCESSDENIED - Acceso Denegado):** 
  - Causa: El usuario remoto no tiene permisos de lanzamiento/acceso en `dcomcnfg`, o las políticas de seguridad local lo están bloqueando.
  - Solución: Verifica el paso 1.1. Si persiste, abre `secpol.msc` > *Directivas locales* > *Opciones de seguridad* > *Acceso a redes: modelo de uso compartido y seguridad...* y cámbialo de "Solo invitado" a **"Clásico - usuarios locales se autentican como ellos mismos"**.

- **0x80040154 (REGDB_E_CLASSNOTREG - Clase no registrada):**
  - Causa: El servidor DCOM no está instalado en la máquina objetivo.
  - Solución: Asegúrate de haber ejecutado el script `Register-Server.ps1` como administrador y de haber instalado los ensamblados en la GAC utilizando `gacutil`.

- **0x800706BA (RPC_S_SERVER_UNAVAILABLE - El servidor RPC no está disponible):**
  - Causa: El cliente no puede alcanzar al servidor a través de la red.
  - Solución: Verifica la dirección IP, asegúrate de que ambas máquinas puedan hacerse *ping*, y revisa que el puerto TCP 135 esté abierto en el Firewall del servidor.

## 4. Ejecución del Cliente Remoto

Una vez configurado el servidor:
1. Copia la carpeta de binarios del cliente (`DcomClient\bin\Debug\net48`) a otra computadora en la misma red local.
2. Ejecuta `DcomClient.exe`.
3. Cuando el programa solicite la IP, ingresa la dirección IP de la máquina servidor (Ej: `192.168.1.50`).