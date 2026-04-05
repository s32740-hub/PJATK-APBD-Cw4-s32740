namespace LegacyRenewalApp;

public interface IBaseCalculation
{
    public decimal Calculate(int seatCount, decimal monthlyPricePerSeat, decimal setupFee);
}

public interface IDiscountStrategy
{
    bool IsMatch(DiscountContext context);
    (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context);
}
public interface IDiscountValidator
{
    public (decimal subtotalAfterDiscount, string? note) Validate(decimal baseAmount, decimal discountAmount);
}

public interface ISupportFee
{
    public bool IsMatch(string normalizedPlanCode);
    public decimal Calculate();
}

public interface IPaymentFee
{
    public bool IsMatch(string normalizedPaymentMethod);
    public (decimal fee, string note) Calculate(decimal subtotalAfterDiscount, decimal supportFee);
}

public interface ISupportFeeProvider
{ 
    public (decimal, string?) CalculateFee(bool includePremiumSupport, string normalizedPlanCode);
}
public interface IPaymentFeeProvider
{ 
    public (decimal, string) CalculateFee(string normalizedPaymentMethod, decimal subtotalAfterDiscount, decimal supportFee);
}

public interface ITaxCalculator
{
    public bool IsMatch(string country);
    public decimal Calculate();
}

public interface ITaxProvider
{
    public decimal CalculateFee(string country);
}
public interface IFinalSumCalculator
{
    public (decimal, decimal, string?) Calculate(decimal subtotalAfterDiscount, decimal supportFee, decimal paymentFee,
        decimal taxRate);
}

public interface IDiscountProvider
{
    (decimal total, string notes) CalculateAllDiscounts(decimal baseAmount, DiscountContext context);
}