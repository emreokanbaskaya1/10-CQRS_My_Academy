using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Results.OrderResults;

namespace MyAcademyCQRS.CQRSPattern.Handlers.OrderHandlers;

public class GetOrderByIdQueryHandler
{
    private readonly AppDbContext _context;

    public GetOrderByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrderByIdQueryResult?> Handle(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Id == orderId)
            .Select(o => new GetOrderByIdQueryResult
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                Email = o.Email,
                Phone = o.Phone,
                TotalPrice = o.TotalPrice,
                PromoCode = o.PromoCode,
                DiscountAmount = o.DiscountAmount,
                Status = o.Status,
                Note = o.Note,
                CreatedDate = o.CreatedDate,
                Items = o.OrderItems.Select(oi => new OrderItemResult
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}
