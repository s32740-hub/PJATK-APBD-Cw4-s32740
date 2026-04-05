using System.Collections.Generic;

namespace LegacyRenewalApp;

public class BaseCalculation : IBaseCalculation
{
    public decimal Calculate(int seatCount, decimal monthlyPricePerSeat, decimal setupFee)
    {
        return (monthlyPricePerSeat * seatCount * 12m) + setupFee;
    }
}
public class DiscountProvider
{
    public IEnumerable<IDiscountStrategy> _strategy;
    public DiscountProvider(IEnumerable<IDiscountStrategy> strategies)
    {
        _strategy = strategies;
    }

    public (decimal total, string notes) CalculateAllDiscounts(decimal baseAmount, DiscountContext context)
    {
        // Klasa nie wie, JAKI konkretnie rabat oblicza
        // Wie tylko, że _strategy umie to zrobić
        decimal discountAmount = 0m;
        string notes = string.Empty;
        foreach (var strategy in _strategy)
        {
            if (strategy.IsMatch(context))
            {
                (decimal discount, string note) = strategy.Calculate(baseAmount, context);
                discountAmount += discount;
                notes += note;
            }
        }
        return (discountAmount, notes);
    }
}

public class DiscountValidator : IDiscountValidator
{
    public (decimal subtotalAfterDiscount, string? note) Validate(decimal  baseAmount, decimal discountAmount)
    {
        decimal subtotalAfterDiscount = baseAmount - discountAmount;
        if (subtotalAfterDiscount < 300m)
        {
            subtotalAfterDiscount = 300m;
            string note = "minimum discounted subtotal applied; ";
            return (subtotalAfterDiscount, note);
        }
        return (subtotalAfterDiscount, null);
    }
}

public class FeeCalculator
{
    
}