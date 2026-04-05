using System;
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

public class SupportFeeProvider:ISupportFeeProvider
{
    public ISupportFee _supportFee;
    public SupportFeeProvider(ISupportFee supportFee)
    {
        _supportFee = supportFee;
    }
    public (decimal, string) CalculateFee(bool includePremiumSupport, string normalizedPlanCode)
    {
        if (includePremiumSupport && _supportFee.IsMatch(normalizedPlanCode))
        {
            return (_supportFee.Calculate(), "premium support included;");
        }

        return (0m, null);
    }
}


public class PaymentFeeProvider:IPaymentFeeProvider
{
    public IPaymentFee _paymentFee;
    public PaymentFeeProvider(IPaymentFee paymentFee)
    {
        _paymentFee = paymentFee;
    }
    public (decimal, string?) CalculateFee(string normalizedPaymentMethod, decimal subtotalAfterDiscount, decimal supportFee)
    {
        if (_paymentFee.IsMatch(normalizedPaymentMethod))
        {
            return _paymentFee.Calculate(subtotalAfterDiscount, supportFee);
        }
        throw new ArgumentException("Unsupported payment method");
    }
}

public class TaxProvider : ITaxProvider
{
    public ITaxCalculator _taxCalculator;
    public TaxProvider(ITaxCalculator taxCalculator)
    {
        _taxCalculator = taxCalculator;
    }
    public decimal CalculateFee(string country)
    {
        if (_taxCalculator.IsMatch(country))
        {
            return _taxCalculator.Calculate();
        }
        return 0.20m;
    }
}

public class FinalSumCalculator:IFinalSumCalculator
{
    public (decimal, string?) Calculate(decimal subtotalAfterDiscount, decimal supportFee, decimal paymentFee, decimal taxRate)
    {
        decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
        decimal taxAmount = taxBase * taxRate;
        decimal finalAmount = taxBase + taxAmount;

        if (finalAmount < 500m)
        {
            finalAmount = 500m;
            string note = "minimum invoice amount applied; ";
            return (finalAmount, note);
        }
        return (finalAmount, null);
    }
}