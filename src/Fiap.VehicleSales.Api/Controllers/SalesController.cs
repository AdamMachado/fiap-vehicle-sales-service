using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.UseCases.Sales;
using Fiap.VehicleSales.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Fiap.VehicleSales.Api.Controllers;
[ApiController]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
    private readonly PurchaseVehicleUseCase _purchaseVehicleUseCase;

    public SalesController(PurchaseVehicleUseCase purchaseVehicleUseCase)
    {
        _purchaseVehicleUseCase = purchaseVehicleUseCase;
    }

    [Authorize(Roles = "buyer")]
    [HttpPost]
    public async Task<IActionResult> Purchase([FromBody] PurchaseVehicleRequest request)
    {
        try
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(buyerId))
                return Unauthorized(new { message = "Comprador não identificado no token." });

            var response = await _purchaseVehicleUseCase.ExecuteAsync(request, buyerId);

            return Ok(response);
        }
        catch (AppException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}