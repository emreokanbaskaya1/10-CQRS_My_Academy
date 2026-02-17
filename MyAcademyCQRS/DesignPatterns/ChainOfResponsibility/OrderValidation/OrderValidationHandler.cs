namespace MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.OrderValidation;

public abstract class OrderValidationHandler
{
    private OrderValidationHandler? _next;

    public OrderValidationHandler SetNext(OrderValidationHandler next)
    {
        _next = next;
        return next;
    }

    public virtual async Task<(bool Success, string Message)> Handle(OrderValidationContext context)
    {
        if (_next != null)
            return await _next.Handle(context);

        return (true, "Tüm validasyonlar başarılı.");
    }
}

public class OrderValidationContext
{
    public List<(int ProductId, int Quantity)> Items { get; set; } = new();
    public string? PromoCode { get; set; }
    public decimal CalculatedTotal { get; set; }
    public decimal? DiscountAmount { get; set; }
}
