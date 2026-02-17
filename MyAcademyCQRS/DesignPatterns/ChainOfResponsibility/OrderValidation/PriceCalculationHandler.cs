using MyAcademyCQRS.DesignPatterns.UnitOfWork;

namespace MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.OrderValidation;

public class PriceCalculationHandler : OrderValidationHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public PriceCalculationHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<(bool Success, string Message)> Handle(OrderValidationContext context)
    {
        decimal total = 0;

        foreach (var item in context.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product != null)
                total += product.Price * item.Quantity;
        }

        if (total <= 0)
            return (false, "Sipariş tutarı geçersiz.");

        context.CalculatedTotal = total;
        return await base.Handle(context);
    }
}
