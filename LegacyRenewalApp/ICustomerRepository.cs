using System.Collections.Generic;
namespace LegacyRenewalApp;

public interface ICustomerRepository
{
    public static readonly Dictionary<int, Customer> Database;
    public Customer GetById(int customerId);
}