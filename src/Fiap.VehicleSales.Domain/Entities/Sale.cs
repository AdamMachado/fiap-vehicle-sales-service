using Fiap.VehicleSales.Domain.Common;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Domain.Exceptions;

namespace Fiap.VehicleSales.Domain.Entities;

public sealed class Sale : Entity
{
    public Guid VehicleId { get; private set; }
    public string BuyerId { get; private set; }
    public string BuyerCpf { get; private set; }
    public string PaymentCode { get; private set; }
    public decimal Price { get; private set; }
    public DateTime SaleDate { get; private set; }
    public DateTime? PaymentProcessedAt { get; private set; }
    public SaleStatus Status { get; private set; }

    private Sale()
    {
        BuyerId = string.Empty;
        BuyerCpf = string.Empty;
        PaymentCode = string.Empty;
    }

    public Sale(Guid vehicleId, string buyerId, string buyerCpf, decimal price, string paymentCode)
    {
        if (vehicleId == Guid.Empty)
            throw new DomainException("O veículo da venda é obrigatório.");

        if (string.IsNullOrWhiteSpace(buyerId))
            throw new DomainException("O comprador da venda é obrigatório.");

        var normalizedCpf = NormalizeCpf(buyerCpf);
        if (!IsValidCpf(normalizedCpf))
            throw new DomainException("O CPF do comprador é inválido.");

        if (string.IsNullOrWhiteSpace(paymentCode))
            throw new DomainException("O código do pagamento é obrigatório.");

        if (price <= 0)
            throw new DomainException("O preço da venda deve ser maior que zero.");

        VehicleId = vehicleId;
        BuyerId = buyerId.Trim();
        BuyerCpf = normalizedCpf;
        PaymentCode = paymentCode.Trim();
        Price = price;
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Pending;
    }

    public bool CompletePayment()
    {
        if (Status == SaleStatus.Completed)
            return false;

        if (Status == SaleStatus.Canceled)
            throw new DomainException("Uma venda cancelada não pode ser concluída.");

        Status = SaleStatus.Completed;
        PaymentProcessedAt = DateTime.UtcNow;
        return true;
    }

    public bool CancelPayment()
    {
        if (Status == SaleStatus.Canceled)
            return false;

        if (Status == SaleStatus.Completed)
            throw new DomainException("Uma venda concluída não pode ser cancelada.");

        Status = SaleStatus.Canceled;
        PaymentProcessedAt = DateTime.UtcNow;
        return true;
    }

    private static string NormalizeCpf(string cpf)
    {
        return new string((cpf ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    private static bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;

        for (var digit = 9; digit < 11; digit++)
        {
            var sum = 0;
            for (var index = 0; index < digit; index++)
                sum += (cpf[index] - '0') * (digit + 1 - index);

            var expected = sum * 10 % 11;
            if (expected == 10)
                expected = 0;

            if (expected != cpf[digit] - '0')
                return false;
        }

        return true;
    }
}
