using MyAcademyCQRS.DesignPatterns.UnitOfWork;

namespace MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.OrderValidation;

public class ProductAvailabilityHandler : OrderValidationHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductAvailabilityHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<(bool Success, string Message)> Handle(OrderValidationContext context)
    {
        foreach (var item in context.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product == null || !product.IsActive)
                return (false, $"Ürün #{item.ProductId} bulunamadı veya aktif değil.");
        }

        return await base.Handle(context);
    }
}
