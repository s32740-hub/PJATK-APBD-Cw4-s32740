using System.Collections.Generic;

namespace LegacyRenewalApp;

public interface ISubscriptionPlanRepository
{
    public static readonly Dictionary<string, SubscriptionPlan> Database;
    public SubscriptionPlan GetByCode(string code);

}