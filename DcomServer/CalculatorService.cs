using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.EnterpriseServices;
using DcomShared;

// Configuración global para el catálogo COM+
[assembly: ApplicationName("DcomServerApp")]
[assembly: ApplicationActivation(ActivationOption.Server)] // "Server" = Out-of-Process (dllhost.exe)
[assembly: ApplicationAccessControl(false)] // Para simplificar pruebas locales

namespace DcomServer
{
    [ComVisible(true)]
    [Guid("A1B2C3D4-E5F6-4A1B-8C9D-0E1F2A3B4C5D")] // GUID único para la clase (CLSID)
    [ProgId("DcomServer.Calculator")] // Nombre amigable para inicializar el objeto
    [ClassInterface(ClassInterfaceType.None)] // Obliga a usar ICalculatorService
    public class CalculatorService : ServicedComponent, ICalculatorService
    {
        public double Add(double a, double b) => a + b;
        public double Subtract(double a, double b) => a - b;
        public double Multiply(double a, double b) => a * b;
        public double Divide(double a, double b) => b != 0 ? a / b : throw new DivideByZeroException();

        public double CalculateMean(double[] numbers)
        {
            if (numbers == null || numbers.Length == 0) return 0;
            double sum = 0;
            foreach (var num in numbers) sum += num;
            return sum / numbers.Length;
        }

        public string GetPrimeFactors(int number)
        {
            if (number <= 1) return "Ninguno";
            var factors = new System.Collections.Generic.List<int>();
            for (int div = 2; div <= number; div++)
            {
                while (number % div == 0)
                {
                    factors.Add(div);
                    number /= div;
                }
            }
            return string.Join(" x ", factors);
        }

        public string ComputeSha256(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return string.Empty;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GetServerTelemetry()
        {
            return $"Host: {Environment.MachineName} | PID Anfitrión: {Process.GetCurrentProcess().Id} | Usuario: {Environment.UserName}";
        }
    }
}