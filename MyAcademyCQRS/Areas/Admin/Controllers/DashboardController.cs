using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities.Enums;
using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ProductCount = await _context.Products.CountAsync();
        ViewBag.OrderCount = await _context.Orders.CountAsync();
        ViewBag.ContactCount = await _context.Contacts.CountAsync(c => !c.IsRead);
        ViewBag.TestimonialCount = await _context.Testimonials.CountAsync(t => !t.IsApproved);
        
        // Sipariþ durumlarýna göre daðýlým
        ViewBag.PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        ViewBag.PreparingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Preparing);
        ViewBag.ReadyOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Ready);
        ViewBag.DeliveredOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
        ViewBag.CancelledOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);
        
        // Kategorilere göre ürün daðýlýmý
        var categoryData = await _context.Categories
            .Include(c => c.Products)
            .Select(c => new { c.Name, Count = c.Products.Count })
            .ToListAsync();
        ViewBag.CategoryNames = string.Join(",", categoryData.Select(c => $"'{c.Name}'"));
        ViewBag.CategoryCounts = string.Join(",", categoryData.Select(c => c.Count));
        
        // Son 7 günlük sipariþ trendi
        var last7Days = Enumerable.Range(0, 7).Select(i => DateTime.Now.Date.AddDays(-i)).Reverse().ToList();
        var ordersByDay = await _context.Orders
            .Where(o => o.CreatedDate >= DateTime.Now.Date.AddDays(-7))
            .GroupBy(o => o.CreatedDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();
        
        var dailyOrderCounts = last7Days.Select(d =>
            ordersByDay.FirstOrDefault(o => o.Date == d)?.Count ?? 0
        ).ToList();
        
        ViewBag.Last7DaysLabels = string.Join(",", last7Days.Select(d => $"'{d:dd MMM}'"));
        ViewBag.Last7DaysCounts = string.Join(",", dailyOrderCounts);
        
        // Toplam gelir (TotalPrice deðil TotalAmount olabilir, Order entity'ye göre kontrol edin)
        ViewBag.TotalRevenue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalPrice);
        
        // Aylýk gelir (son 6 ay)
        var last6Months = Enumerable.Range(0, 6).Select(i => DateTime.Now.AddMonths(-i)).Reverse().ToList();
        var revenueByMonth = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered && o.CreatedDate >= DateTime.Now.AddMonths(-6))
            .GroupBy(o => new { o.CreatedDate.Year, o.CreatedDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => x.TotalPrice) })
            .ToListAsync();
        
        var monthlyRevenue = last6Months.Select(m =>
            revenueByMonth.FirstOrDefault(r => r.Year == m.Year && r.Month == m.Month)?.Total ?? 0
        ).ToList();
        
        ViewBag.MonthLabels = string.Join(",", last6Months.Select(m => $"'{m:MMM}'"));
        ViewBag.MonthlyRevenue = string.Join(",", monthlyRevenue.Select(r => r.ToString("F2")));
        
        // En çok satan ürünler (top 5)
        var topProducts = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SelectMany(o => o.OrderItems)
            .GroupBy(oi => oi.Product.Name)
            .Select(g => new { ProductName = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToListAsync();
        
        ViewBag.TopProductNames = string.Join(",", topProducts.Select(p => $"'{p.ProductName}'"));
        ViewBag.TopProductQuantities = string.Join(",", topProducts.Select(p => p.Quantity));
        
        // Aktif/Pasif ürünler
        ViewBag.ActiveProducts = await _context.Products.CountAsync(p => p.IsActive);
        ViewBag.InactiveProducts = await _context.Products.CountAsync(p => !p.IsActive);
        
        // Onaylý/Onaysýz yorumlar
        ViewBag.ApprovedTestimonials = await _context.Testimonials.CountAsync(t => t.IsApproved);
        ViewBag.PendingTestimonials = await _context.Testimonials.CountAsync(t => !t.IsApproved);
        
        ViewData["Title"] = "Dashboard";
        return View();
    }
}
