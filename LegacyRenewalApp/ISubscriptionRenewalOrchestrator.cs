namespace LegacyRenewalApp;

public interface ISubscriptionRenewalOrchestrator
{
    public RenewalInvoice CreateRenewalInvoice(
        RenewalRequest request);
}