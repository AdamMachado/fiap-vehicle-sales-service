using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.UseCases.Vehicles;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.VehicleSales.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController : ControllerBase
{
    private readonly GetVehicleByIdUseCase _getVehicleByIdUseCase;
    private readonly ListAvailableVehiclesUseCase _listAvailableVehiclesUseCase;
    private readonly ListSoldVehiclesUseCase _listSoldVehiclesUseCase;

    public VehiclesController(
        GetVehicleByIdUseCase getVehicleByIdUseCase,
        ListAvailableVehiclesUseCase listAvailableVehiclesUseCase,
        ListSoldVehiclesUseCase listSoldVehiclesUseCase)
    {
        _getVehicleByIdUseCase = getVehicleByIdUseCase;
        _listAvailableVehiclesUseCase = listAvailableVehiclesUseCase;
        _listSoldVehiclesUseCase = listSoldVehiclesUseCase;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _getVehicleByIdUseCase.ExecuteAsync(id);

            return Ok(response);
        }
        catch (AppException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var response = await _listAvailableVehiclesUseCase.ExecuteAsync();

        return Ok(response);
    }

    [HttpGet("sold")]
    public async Task<IActionResult> GetSold()
    {
        var response = await _listSoldVehiclesUseCase.ExecuteAsync();

        return Ok(response);
    }
}
