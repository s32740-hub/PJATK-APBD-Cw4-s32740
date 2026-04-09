using System;

namespace LegacyRenewalApp;

public class SubscriptionRenewalServiceOrchestrator:ISubscriptionRenewalOrchestrator
{
    private readonly ICustomerPlanRepository _customerPlanRepository;
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
    private readonly IRenewalValidator _renewalValidator;
    private readonly IBaseCalculation _baseCalculation;
    private readonly IDiscountProvider _discountProvider;
    private readonly IDiscountValidator _discountValidator;
    private readonly ISupportFeeProvider _supportFeeProvider;
    private readonly IPaymentFeeProvider _paymentFeeProvider;
    private readonly ITaxProvider _taxProvider;
    private readonly IFinalSumCalculator _finalSumCalculator;
    private readonly IRenewalProcessService _renewalProcessService;

    public SubscriptionRenewalServiceOrchestrator(
        ICustomerPlanRepository customerPlanRepository,
        ISubscriptionPlanRepository subscriptionPlanRepository,
        IRenewalValidator renewalValidator,
        IBaseCalculation baseCalculation,
        IDiscountProvider discountProvider,
        IDiscountValidator discountValidator,
        ISupportFeeProvider supportFeeProvider,
        IPaymentFeeProvider paymentFeeProvider,
        ITaxProvider taxProvider,
        IFinalSumCalculator finalSumCalculator,
        IRenewalProcessService renewalProcessService)
    {
        _customerPlanRepository = customerPlanRepository;
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _renewalValidator = renewalValidator;
        _baseCalculation = baseCalculation;
        _discountProvider = discountProvider;
        _discountValidator = discountValidator;
        _supportFeeProvider = supportFeeProvider;
        _paymentFeeProvider = paymentFeeProvider;
        _taxProvider = taxProvider;
        _finalSumCalculator = finalSumCalculator;
        _renewalProcessService = renewalProcessService;
    }

    public RenewalInvoice CreateRenewalInvoice(
        RenewalRequest request)
    {
        _renewalValidator.Validate(request);

        var customer = _customerPlanRepository.GetById(request.CustomerId);
        var plan = _subscriptionPlanRepository.GetByCode(request.PlanCode);

        if (!customer.IsActive)
            throw new InvalidOperationException("Inactive customers cannot renew subscriptions");

        decimal baseAmount = _baseCalculation.Calculate(
            request.SeatCount, plan.MonthlyPricePerSeat, plan.SetupFee);

        var discountContext = new DiscountContext(
            customer.Segment,
            customer.YearsWithCompany,
            request.SeatCount,
            customer.LoyaltyPoints,
            request.UseLoyaltyPoints,
            plan.IsEducationEligible);

        var (discountAmount, notes) = _discountProvider.CalculateAllDiscounts(baseAmount, discountContext);

        var (subtotalAfterDiscount, discountNote) = _discountValidator.Validate(baseAmount, discountAmount);
        notes += discountNote ?? string.Empty;

        var (supportFee, supportNote) = _supportFeeProvider.CalculateFee(
            request.IncludePremiumSupport, request.PlanCode);
        notes += supportNote ?? string.Empty;

        var (paymentFee, paymentNote) = _paymentFeeProvider.CalculateFee(
            request.PaymentMethod, subtotalAfterDiscount, supportFee);
        notes += paymentNote ?? string.Empty;

        decimal taxRate = _taxProvider.CalculateFee(customer.Country);

        var (finalAmount, taxAmount, finalNote) = _finalSumCalculator.Calculate(
            subtotalAfterDiscount, supportFee, paymentFee, taxRate);
        notes += finalNote ?? string.Empty;

        var invoiceRequest = new InvoiceRequest(
            BaseAmount: baseAmount,
            TaxAmount: taxAmount,
            CustomerId: customer.Id,
            NormalizedPlanCode: request.PlanCode,
            NormalizedPaymentMethod: request.PaymentMethod,
            SeatCount: request.SeatCount,
            DiscountAmount: discountAmount,
            PaymentFee: paymentFee,
            FinalAmount: finalAmount,
            Notes: notes,
            SupportFee: supportFee,
            FullName: customer.FullName);

        return _renewalProcessService.Execute(invoiceRequest, customer.Email);
    }
}