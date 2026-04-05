namespace LegacyRenewalApp;

public interface IDiscountStrategy
{
    bool IsMatch(DiscountContext context);
    (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context);
}
public interface IDiscountValidator
{
    public (decimal subtotalAfterDiscount, string? note) Validate(decimal baseAmount, decimal discountAmount);
}
public interface IDiscountProvider
{
    (decimal total, string notes) CalculateAllDiscounts(decimal baseAmount, DiscountContext context);
}