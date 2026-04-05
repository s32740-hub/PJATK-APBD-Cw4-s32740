namespace LegacyRenewalApp;

public interface IInvoiceFactory 
{
    RenewalInvoice Create(InvoiceRequest request);
}

public interface IBillingRepository 
{
    void Save(RenewalInvoice invoice);
}

public interface INotificationService 
{
    void Notify(RenewalInvoice invoice, string email);
}

public interface IRenewalProcessService
{
    public RenewalInvoice Execute(InvoiceRequest request, string email);
}