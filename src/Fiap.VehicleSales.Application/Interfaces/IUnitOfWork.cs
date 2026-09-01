namespace Fiap.VehicleSales.Application.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync();
}