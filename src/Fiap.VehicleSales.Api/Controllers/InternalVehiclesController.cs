using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.UseCases.Vehicles;
using Fiap.VehicleSales.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.VehicleSales.Api.Controllers;

[ApiController]
[Route("api/internal/vehicles")]
[Authorize(Policy = "InternalService")]
public sealed class InternalVehiclesController : ControllerBase
{
    private readonly SyncVehicleUseCase _syncVehicleUseCase;

    public InternalVehiclesController(SyncVehicleUseCase syncVehicleUseCase)
    {
        _syncVehicleUseCase = syncVehicleUseCase;
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Sync(Guid id, [FromBody] SyncVehicleRequest request)
    {
        try
        {
            var result = await _syncVehicleUseCase.ExecuteAsync(id, request);
            return result.Created
                ? CreatedAtAction(nameof(VehiclesController.GetById), "Vehicles", new { id }, result.Vehicle)
                : Ok(result.Vehicle);
        }
        catch (DomainException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
