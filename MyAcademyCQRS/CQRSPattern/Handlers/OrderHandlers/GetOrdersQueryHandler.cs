using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Results.OrderResults;

namespace MyAcademyCQRS.CQRSPattern.Handlers.OrderHandlers;

public class GetOrdersQueryHandler
{
    private readonly AppDbContext _context;

    public GetOrdersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetOrdersQueryResult>> Handle()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedDate)
            .Select(o => new GetOrdersQueryResult
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                Email = o.Email,
                TotalPrice = o.TotalPrice,
                PromoCode = o.PromoCode,
                DiscountAmount = o.DiscountAmount,
                Status = o.Status,
                CreatedDate = o.CreatedDate,
                ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
            })
            .ToListAsync();
    }
}
