using System;
using System.Runtime.InteropServices;

namespace DcomShared
{
    [ComVisible(true)]
    [Guid("E3A2F9A1-7B8D-4C5E-9F1A-2B3C4D5E6F7A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ICalculatorService
    {
        double Add(double a, double b);
        double Subtract(double a, double b);
        double Multiply(double a, double b);
        double Divide(double a, double b);
        double CalculateMean(double[] numbers);
        string GetPrimeFactors(int number);
        string ComputeSha256(string payload);
        string GetServerTelemetry();
    }
}