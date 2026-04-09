using System;

namespace LegacyRenewalApp;

public record InvoiceRequest(
    decimal BaseAmount,
    decimal TaxAmount,
    int CustomerId,
    string NormalizedPlanCode,
    string NormalizedPaymentMethod,
    int SeatCount,
    decimal DiscountAmount,
    decimal PaymentFee,
    decimal FinalAmount,
    string Notes,
    decimal SupportFee,
    string FullName
    );
public class RenewalInvoiceFactory : IInvoiceFactory
{
    public RenewalInvoice Create(InvoiceRequest request)
    {
        var invoice = new RenewalInvoice
        {
            BaseAmount = Round(request.BaseAmount),
            TaxAmount = Round(request.TaxAmount),
            InvoiceNumber = GenerateInvoiceNumber(request.CustomerId, request.NormalizedPlanCode),
            CustomerName = request.FullName,
            PlanCode = request.NormalizedPlanCode,
            PaymentMethod = request.NormalizedPaymentMethod,
            SeatCount = request.SeatCount,
            DiscountAmount = Round(request.DiscountAmount),
            SupportFee = Round(request.SupportFee),
            PaymentFee = Round(request.PaymentFee),
            FinalAmount = Round(request.FinalAmount),
            Notes = request.Notes?.Trim() ?? string.Empty,
            GeneratedAt = DateTime.UtcNow
        };
        return invoice;
    }

    private decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    
    private string GenerateInvoiceNumber(int CustomerId, string PlanCode) 
        => $"INV-{DateTime.UtcNow:yyyyMMdd}-{CustomerId}-{PlanCode}";
}
public class SaveInvoiceService: IBillingRepository{
    public void Save(RenewalInvoice invoice) 
        => LegacyBillingGateway.SaveInvoice(invoice);
}
public class LegacyEmailAdapter : INotificationService
{
    public void Notify(RenewalInvoice invoice, string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        string subject = "Subscription renewal invoice";
        string body = $"Hello {invoice.CustomerName}, your renewal is ready. Amount: {invoice.FinalAmount:F2}.";
        
        LegacyBillingGateway.SendEmail(email, subject, body);
    }
}