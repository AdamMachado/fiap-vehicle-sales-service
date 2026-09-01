using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.UseCases.Vehicles;
using Fiap.VehicleSales.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.VehicleSales.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController : ControllerBase
{
    private readonly CreateVehicleUseCase _createVehicleUseCase;
    private readonly UpdateVehicleUseCase _updateVehicleUseCase;
    private readonly GetVehicleByIdUseCase _getVehicleByIdUseCase;
    private readonly ListAvailableVehiclesUseCase _listAvailableVehiclesUseCase;
    private readonly ListSoldVehiclesUseCase _listSoldVehiclesUseCase;

    public VehiclesController(
        CreateVehicleUseCase createVehicleUseCase,
        UpdateVehicleUseCase updateVehicleUseCase,
        GetVehicleByIdUseCase getVehicleByIdUseCase,
        ListAvailableVehiclesUseCase listAvailableVehiclesUseCase,
        ListSoldVehiclesUseCase listSoldVehiclesUseCase)
    {
        _createVehicleUseCase = createVehicleUseCase;
        _updateVehicleUseCase = updateVehicleUseCase;
        _getVehicleByIdUseCase = getVehicleByIdUseCase;
        _listAvailableVehiclesUseCase = listAvailableVehiclesUseCase;
        _listSoldVehiclesUseCase = listSoldVehiclesUseCase;
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
    {
        try
        {
            var response = await _createVehicleUseCase.ExecuteAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleRequest request)
    {
        try
        {
            var response = await _updateVehicleUseCase.ExecuteAsync(id, request);

            return Ok(response);
        }
        catch (AppException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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