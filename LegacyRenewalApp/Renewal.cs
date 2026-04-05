using System;

namespace LegacyRenewalApp;
//Record, który odpowiada za atrybuty requestu (jeżeli chcemy zmienić np. liczbę atrybutów -
//nie musimy zmieniać konkretnej klasy)
public record RenewalRequest
{
    public int CustomerId;
    public string PlanCode;
    public int SeatCount;
    public string PaymentMethod;
    public bool IncludePremiumSupport;
    public bool UseLoyaltyPoints;

    public RenewalRequest(int customerId,
        string planCode,
        int seatCount,
        string paymentMethod,
        bool includePremiumSupport,
        bool useLoyaltyPoints)
    {
        CustomerId = customerId;
        SeatCount = seatCount;
        IncludePremiumSupport = includePremiumSupport;
        UseLoyaltyPoints = useLoyaltyPoints;
        PlanCode = planCode.Trim().ToUpperInvariant();
        PaymentMethod = paymentMethod.Trim().ToUpperInvariant(); 
    }
}
//klasa, odpowiadająca za walidację danych wejsciowych
public class RenewalValidator:IRenewalValidator
{
    public void Validate(RenewalRequest request)
    {
        if (request.CustomerId <= 0)
        {
            throw new ArgumentException("Customer id must be positive");
        }

        if (string.IsNullOrWhiteSpace(request.PlanCode))
        {
            throw new ArgumentException("Plan code is required");
        }

        if (request.SeatCount <= 0)
        {
            throw new ArgumentException("Seat count must be positive");
        }

        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            throw new ArgumentException("Payment method is required");
        }

    }
}