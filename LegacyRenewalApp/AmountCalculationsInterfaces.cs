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
}