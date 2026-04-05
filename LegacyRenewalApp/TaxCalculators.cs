namespace LegacyRenewalApp;

public class TaxCalculatorPoland : ITaxCalculator
{
    public bool IsMatch(string country) => country == "Poland";
    public decimal Calculate()
    {
        return 0.23m;
    }
}

public class TaxCalculatorGermany : ITaxCalculator
{
    public bool IsMatch(string country) => country == "Germany";
    public decimal Calculate()
    {
        return 0.19m;
    }
}

public class TaxCalculatorCzech : ITaxCalculator
{
    public bool IsMatch(string country) => country == "Czech Republic";
    public decimal Calculate()
    {
        return 0.21m;
    }
}

public class TaxCalculatorNorway : ITaxCalculator
{
    public bool IsMatch(string country) => country == "Norway";
    public decimal Calculate()
    {
        return 0.25m;
    }
}