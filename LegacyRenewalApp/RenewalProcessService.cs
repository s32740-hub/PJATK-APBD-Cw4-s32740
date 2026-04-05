namespace LegacyRenewalApp;

public class RenewalProcessService:IRenewalProcessService
{
    private readonly IInvoiceFactory _factory;
    private readonly IBillingRepository _repository;
    private readonly INotificationService _notification;

    public RenewalProcessService(
        IInvoiceFactory factory,
        IBillingRepository repository,
        INotificationService notification
    )
    {
        _factory = factory;
        _repository = repository;
        _notification = notification;
    }
    public RenewalInvoice Execute(InvoiceRequest request, string email)
    {
        var invoice = _factory.Create(request);
        _repository.Save(invoice);
        _notification.Notify(invoice, email);
        return invoice;
    }
}