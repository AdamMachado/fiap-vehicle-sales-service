namespace Fiap.VehicleSales.Application.Exceptions;

public sealed class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }
}