using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ISubscriptionRenewalOrchestrator _factory;
        public SubscriptionRenewalService()
        {
            _factory = new SubscriptionRenewalServiceFactory(
                customerPlanRepository: new CustomerPlanRepository(),
                subscriptionPlanRepository: new SubscriptionPlanRepository(),
                renewalValidator: new RenewalValidator(),
                baseCalculation: new BaseCalculation(),
                discountProvider: new DiscountProvider(new List<IDiscountStrategy>
                {
                    new SilverSegmentDiscount(),
                    new GoldSegmentDiscount(),
                    new PlatinumSegmentDiscount(),
                    new EducationSegmentDiscount(),
                    new FiveAndMoreYearsWithCompanyDiscount(),
                    new FromTwoToFiveYearsWithCompanyDiscount(),
                    new FiftyAndMoreSeatCountDiscount(),
                    new FromTwentyToFiftySeatCountDiscount(),
                    new FromTenToTwentySeatCountDiscount(),
                    new LoyaltyDiscountRule()
                }),
                discountValidator: new DiscountValidator(),
                supportFeeProvider: new SupportFeeProvider(new List<ISupportFee>
                {
                    new SupportFeeStartPlan(),
                    new SupportFeeProPlan(),
                    new SupportFeeEnterprisePlan(),
                    new DefaultSupportPlan()
                }),
                paymentFeeProvider: new PaymentFeeProvider(new List<IPaymentFee>
                {
                    new PaymentFeeCard(),
                    new PaymentFeeBankTransfer(),
                    new PaymentFeePayPal(),
                    new PaymentFeeInvoice()
                }),
                taxProvider: new TaxProvider(new List<ITaxCalculator>
                {
                    new TaxCalculatorPoland(),
                    new TaxCalculatorGermany(),
                    new TaxCalculatorCzech(),
                    new TaxCalculatorNorway(),
                    new TaxCalculatorDefault()
                }),
                finalSumCalculator: new FinalSumCalculator(),
                renewalProcessService: new RenewalProcessService(
                    factory: new RenewalInvoiceFactory(),
                    repository: new SaveInvoiceService(),
                    notification: new LegacyEmailAdapter()
                ));
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            RenewalRequest request = new RenewalRequest(
                customerId, planCode, seatCount,
                paymentMethod, includePremiumSupport, useLoyaltyPoints);
            return _factory.CreateRenewalInvoice(request);
        }
    }
}