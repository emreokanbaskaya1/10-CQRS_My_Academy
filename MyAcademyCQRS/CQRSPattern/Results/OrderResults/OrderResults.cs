using MyAcademyCQRS.Entities.Enums;

namespace MyAcademyCQRS.CQRSPattern.Results.OrderResults;

public record GetOrdersQueryResult
{
    public int Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public decimal TotalPrice { get; init; }
    public string? PromoCode { get; init; }
    public decimal? DiscountAmount { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime CreatedDate { get; init; }
    public int ItemCount { get; init; }
}

public record GetOrderByIdQueryResult
{
    public int Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public decimal TotalPrice { get; init; }
    public string? PromoCode { get; init; }
    public decimal? DiscountAmount { get; init; }
    public OrderStatus Status { get; init; }
    public string? Note { get; init; }
    public DateTime CreatedDate { get; init; }
    public List<OrderItemResult> Items { get; init; } = new();
}

public record OrderItemResult
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
