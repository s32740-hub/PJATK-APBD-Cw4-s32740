namespace LegacyRenewalApp;

public interface IBaseCalculation
{
    public decimal Calculate(int seatCount, decimal monthlyPricePerSeat, decimal setupFee);
}
public interface IFinalSumCalculator
{
    public (decimal, decimal, string?) Calculate(decimal subtotalAfterDiscount, decimal supportFee, decimal paymentFee,
        decimal taxRate);
}