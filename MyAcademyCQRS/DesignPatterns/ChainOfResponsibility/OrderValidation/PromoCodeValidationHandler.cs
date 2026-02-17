using MyAcademyCQRS.DesignPatterns.UnitOfWork;

namespace MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.OrderValidation;

public class PromoCodeValidationHandler : OrderValidationHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public PromoCodeValidationHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<(bool Success, string Message)> Handle(OrderValidationContext context)
    {
        if (!string.IsNullOrEmpty(context.PromoCode))
        {
            var promotions = await _unitOfWork.Promotions
                .FindAsync(p => p.PromoCode == context.PromoCode
                    && p.IsActive
                    && p.StartDate <= DateTime.UtcNow
                    && p.EndDate >= DateTime.UtcNow);

            var promo = promotions.FirstOrDefault();
            if (promo == null)
                return (false, $"Promosyon kodu '{context.PromoCode}' geçersiz veya süresi dolmuş.");

            var discount = context.CalculatedTotal * (promo.DiscountPercentage / 100);
            context.DiscountAmount = discount;
            context.CalculatedTotal -= discount;
        }

        return await base.Handle(context);
    }
}
