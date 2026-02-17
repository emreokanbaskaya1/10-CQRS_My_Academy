using MyAcademyCQRS.Entities.Enums;

namespace MyAcademyCQRS.Entities;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public decimal TotalPrice { get; set; }
    public string? PromoCode { get; set; }
    public decimal? DiscountAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Note { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public List<OrderItem> OrderItems { get; set; } = new();
}
