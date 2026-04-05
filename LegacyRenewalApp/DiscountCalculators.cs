namespace LegacyRenewalApp;

public record DiscountContext(
    string Segment,
    int YearsWithCompany,
    int SeatCount,
    int LoyaltyPoints,
    bool UseLoyaltyPoints,
    bool IsEducationEligible
);
// Każdy typ rabatu to osobna klasa
public class SilverSegmentDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.Segment == "Silver";

    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context) 
        => (baseAmount * 0.05m, "silver discount; ");
}

public class GoldSegmentDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.Segment == "Gold";

    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context) 
        => (baseAmount * 0.10m, "gold discount; ");
}

public class PlatinumSegmentDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.Segment == "Platinum";

    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context) 
        => (baseAmount * 0.15m, "platinum discount; ");
}

public class EducationSegmentDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.Segment == "Education" && context.IsEducationEligible;

    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context) 
        => (baseAmount * 0.20m, "education discount; ");
}
public class FiveAndMoreYearsWithCompanyDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.YearsWithCompany >= 5;
    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context)
    => (baseAmount * 0.07m, "long-term loyalty discount; ");
}
public class FromTwoToFiveYearsWithCompanyDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.YearsWithCompany >= 2 && context.YearsWithCompany < 5;
    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context)
        => (baseAmount * 0.03m, "basic loyalty discount; ");
}
public class FiftyAndMoreSeatCountDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.SeatCount >= 50;
    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context)
        => (baseAmount * 0.12m, "large team discount; ");
}
public class FromTwentyToFiftySeatCountDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.SeatCount >= 20 && context.SeatCount < 50;
    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context)
        => (baseAmount * 0.08m, "medium team discount; ");
}
public class FromTenToTwentySeatCountDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.SeatCount >= 10 && context.SeatCount < 20;
    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context)
        => (baseAmount * 0.04m, "small team discount; ");
}
public class LoyaltyDiscountRule
{
    public bool IsMatch(DiscountContext context) => context.UseLoyaltyPoints && context.LoyaltyPoints>0;

    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context)
    {
        int pointsToUse = context.LoyaltyPoints > 200 ? 200 : context.LoyaltyPoints;
        decimal discountAmount = pointsToUse;
        string notes = $"loyalty points used: {pointsToUse}; ";
        return (discountAmount, notes);
    }
}