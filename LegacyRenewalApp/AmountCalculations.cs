using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacyRenewalApp;

public class BaseCalculation : IBaseCalculation
{
    public decimal Calculate(int seatCount, decimal monthlyPricePerSeat, decimal setupFee)
    {
        return (monthlyPricePerSeat * seatCount * 12m) + setupFee;
    }
}
public class DiscountProvider : IDiscountProvider
{
    private readonly IEnumerable<IDiscountStrategy> _strategy;
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

public class SupportFeeProvider:ISupportFeeProvider
{
    private readonly IEnumerable<ISupportFee> _supportFee;
    public SupportFeeProvider(IEnumerable<ISupportFee> supportFee)
    {
        _supportFee = supportFee;
    }
    public (decimal, string) CalculateFee(bool includePremiumSupport, string normalizedPlanCode)
    {
        if (includePremiumSupport)
        {
            var match = _supportFee.FirstOrDefault(c => c.IsMatch(normalizedPlanCode));
            return (match.Calculate(), "premium support included;");
        }

        return (0m, null);
    }
}


public class PaymentFeeProvider:IPaymentFeeProvider
{
    private readonly IEnumerable<IPaymentFee> _paymentFees;
    public PaymentFeeProvider(IEnumerable<IPaymentFee> paymentFees)
    {
        _paymentFees = paymentFees;
    }
    public (decimal, string?) CalculateFee(string normalizedPaymentMethod, decimal subtotalAfterDiscount, decimal supportFee)
    {
        var match = _paymentFees.FirstOrDefault(c => c.IsMatch(normalizedPaymentMethod));
        return match?.Calculate(subtotalAfterDiscount, supportFee) ?? throw new ArgumentException("Unsupported payment method");
    }
}
public class TaxProvider : ITaxProvider
{
    private readonly IEnumerable<ITaxCalculator> _calculators;

    public TaxProvider(IEnumerable<ITaxCalculator> calculators)
    {
        _calculators = calculators;
    }

    public decimal CalculateFee(string country)
    {
        var match = _calculators.FirstOrDefault(c => c.IsMatch(country));
        return match?.Calculate() ?? 0.20m;
    }
}

public class FinalSumCalculator:IFinalSumCalculator
{
    public (decimal, decimal, string?) Calculate(decimal subtotalAfterDiscount, decimal supportFee, decimal paymentFee, decimal taxRate)
    {
        decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
        decimal taxAmount = taxBase * taxRate;
        decimal finalAmount = taxBase + taxAmount;
        decimal totalTax = (subtotalAfterDiscount + supportFee + paymentFee) * taxRate;

        if (finalAmount < 500m)
        {
            finalAmount = 500m;
            string note = "minimum invoice amount applied; ";
            return (finalAmount, totalTax, note);
        }
        return (finalAmount, totalTax, null);
    }
}