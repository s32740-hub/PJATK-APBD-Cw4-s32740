namespace LegacyRenewalApp;

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
