using Microsoft.AspNetCore.Mvc;
using MyAcademyCQRS.CQRSPattern.Commands.OrderCommands;
using MyAcademyCQRS.CQRSPattern.Handlers.OrderHandlers;
using MyAcademyCQRS.Entities.Enums;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly GetOrdersQueryHandler _getOrdersHandler;
    private readonly GetOrderByIdQueryHandler _getOrderByIdHandler;
    private readonly UpdateOrderStatusCommandHandler _updateStatusHandler;

    public OrderController(GetOrdersQueryHandler getOrdersHandler, GetOrderByIdQueryHandler getOrderByIdHandler, UpdateOrderStatusCommandHandler updateStatusHandler)
    {
        _getOrdersHandler = getOrdersHandler;
        _getOrderByIdHandler = getOrderByIdHandler;
        _updateStatusHandler = updateStatusHandler;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        ViewData["Title"] = "Sipariş Yönetimi";
        
        const int pageSize = 1000;
        var allOrders = await _getOrdersHandler.Handle();
        
        var totalCount = allOrders.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var orders = allOrders
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalCount = totalCount;
        ViewBag.PageSize = pageSize;
        
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Sipariş Detayı";
        var order = await _getOrderByIdHandler.Handle(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        await _updateStatusHandler.Handle(new UpdateOrderStatusCommand(id, status));
        TempData["Success"] = $"Sipariş durumu '{status}' olarak güncellendi.";
        return RedirectToAction("Index");
    }
}
