namespace LegacyRenewalApp;

public interface ITaxCalculator
{
    public bool IsMatch(string country);
    public decimal Calculate();
}

public interface ITaxProvider
{
    public decimal CalculateFee(string country);
}