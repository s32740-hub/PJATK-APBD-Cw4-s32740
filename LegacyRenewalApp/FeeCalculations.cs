namespace LegacyRenewalApp;

// Każdy typ fee to osobna klasa
public class SupportFeeStartPlan:ISupportFee
{
    public bool IsMatch(string normalizedPlanCode) => normalizedPlanCode == "START";
    public decimal Calculate() => 250m;
}

public class SupportFeeProPlan:ISupportFee
{
    public bool IsMatch(string normalizedPlanCode) => normalizedPlanCode == "PRO";
    public decimal Calculate() => 400m;
}
public class SupportFeeEnterprisePlan:ISupportFee
{
    public bool IsMatch(string normalizedPlanCode) => normalizedPlanCode == "ENTERPRISE";
    public decimal Calculate() => 700m;
}
public class DefaultSupportPlan:ISupportFee
{
    public bool IsMatch(string normalizedPlanCode) => true;
    public decimal Calculate() => 0m;
}

public class PaymentFeeCard : IPaymentFee
{
    public bool IsMatch(string normalizedPaymentMethod) => normalizedPaymentMethod == "CARD";
    public (decimal fee, string note) Calculate(decimal subtotalAfterDiscount, decimal supportFee) => 
        ((subtotalAfterDiscount + supportFee) * 0.02m, "card payment fee; ");
}

public class PaymentFeeBankTransfer : IPaymentFee
{
    public bool IsMatch(string normalizedPaymentMethod) => normalizedPaymentMethod == "BANK_TRANSFER";
    public (decimal fee, string note) Calculate(decimal subtotalAfterDiscount, decimal supportFee) => 
        ((subtotalAfterDiscount + supportFee) * 0.01m, "bank transfer fee; ");
}

public class PaymentFeePayPal : IPaymentFee
{
    public bool IsMatch(string normalizedPaymentMethod) => normalizedPaymentMethod == "PAYPAL";
    public (decimal fee, string note) Calculate(decimal subtotalAfterDiscount, decimal supportFee) => 
        ((subtotalAfterDiscount + supportFee) * 0.035m, "paypal fee; ");
}

public class PaymentFeeInvoice : IPaymentFee
{
    public bool IsMatch(string normalizedPaymentMethod) => normalizedPaymentMethod == "INVOICE";
    public (decimal fee, string note) Calculate(decimal subtotalAfterDiscount, decimal supportFee) => 
        ((subtotalAfterDiscount + supportFee) * 0.0m, "invoice payment; ");
}