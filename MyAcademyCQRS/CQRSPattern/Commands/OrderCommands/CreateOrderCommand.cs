namespace MyAcademyCQRS.CQRSPattern.Commands.OrderCommands;

public record CreateOrderCommand
{
    public string CustomerName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Note { get; init; }
    public string? PromoCode { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
}

public record OrderItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}
