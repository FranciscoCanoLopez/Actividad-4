using System;
using System.Runtime.InteropServices;
using DcomShared;

namespace DcomClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Cliente DCOM Iniciado ===");
            Console.Write("Ingrese la IP o Nombre del Servidor (Deje en blanco para Localhost): ");
            string serverName = Console.ReadLine();

            try
            {
                // 1. Obtener el tipo del componente a través de su ProgID
                Type dcomType = string.IsNullOrWhiteSpace(serverName) 
                    ? Type.GetTypeFromProgID("DcomServer.Calculator") 
                    : Type.GetTypeFromProgID("DcomServer.Calculator", serverName, throwOnError: true);

                if (dcomType == null)
                {
                    Console.WriteLine("Error: No se encontró el ProgID en el registro. ¿Registraste el servidor?");
                    return;
                }

                // 2. Activar la instancia (Esto lanza la petición RPC hacia DCOM)
                Console.WriteLine("\n[+] Activando instancia COM...");
                object dcomObject = Activator.CreateInstance(dcomType);

                // 3. Early Binding: Castear el objeto a nuestra interfaz conocida
                ICalculatorService calc = (ICalculatorService)dcomObject;

                // 4. Pruebas de invocación
                Console.WriteLine("\n=== Ejecutando Métodos Remotos ===");
                
                Console.WriteLine($"Telemetría: {calc.GetServerTelemetry()}");
                Console.WriteLine($"Suma (5 + 7): {calc.Add(5, 7)}");
                Console.WriteLine($"Factores de 126: {calc.GetPrimeFactors(126)}");
                Console.WriteLine($"SHA-256 ('Hola DCOM'): {calc.ComputeSha256("Hola DCOM")}");
                
                double[] stats = { 10.5, 20.2, 30.8 };
                Console.WriteLine($"Media estadística: {calc.CalculateMean(stats)}");
            }
            catch (COMException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Error DCOM] Código HRESULT: 0x{ex.ErrorCode:X}");
                Console.WriteLine($"Mensaje: {ex.Message}");
                
                if (ex.ErrorCode == unchecked((int)0x80070005))
                    Console.WriteLine("Sugerencia: Acceso Denegado. Revisa los permisos en dcomcnfg.");
                else if (ex.ErrorCode == unchecked((int)0x80040154))
                    Console.WriteLine("Sugerencia: Clase no registrada. Ejecuta el script de registro del servidor.");
                
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error General]: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}