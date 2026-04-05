using System;

namespace LegacyRenewalApp;

public record InvoiceRequest(
    decimal BaseAmount,
    decimal TaxAmount,
    int customerId,
    string normalizedPlanCode,
    string normalizedPaymentMethod,
    int seatCount,
    decimal discountAmount,
    decimal paymentFee,
    decimal finalAmount,
    string notes,
    decimal supportFee,
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
            InvoiceNumber = GenerateInvoiceNumber(request.customerId, request.normalizedPlanCode),
            CustomerName = request.FullName,
            PlanCode = request.normalizedPlanCode,
            PaymentMethod = request.normalizedPaymentMethod,
            SeatCount = request.seatCount,
            DiscountAmount = Round(request.discountAmount),
            SupportFee = Round(request.supportFee),
            PaymentFee = Round(request.paymentFee),
            FinalAmount = Round(request.finalAmount),
            Notes = request.notes?.Trim() ?? string.Empty,
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