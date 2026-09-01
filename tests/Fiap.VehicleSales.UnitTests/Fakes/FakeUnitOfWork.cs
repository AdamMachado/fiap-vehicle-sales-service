using Fiap.VehicleSales.Application.Interfaces;

namespace Fiap.VehicleSales.UnitTests.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public bool WasCommitted { get; private set; }

    public void Reset()
    {
        WasCommitted = false;
    }

    public Task CommitAsync()
    {
        WasCommitted = true;

        return Task.CompletedTask;
    }
}
