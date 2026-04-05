namespace LegacyRenewalApp;

public interface ICustomerPlanRepository
{
    public Customer GetById(int customerId);
}
public interface ISubscriptionPlanRepository
{
    public SubscriptionPlan GetByCode(string code);

}