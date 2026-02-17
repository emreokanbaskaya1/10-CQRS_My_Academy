using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Commands.OrderCommands;
using MyAcademyCQRS.DesignPatterns.Observer;
using MyAcademyCQRS.Entities.Enums;
using Serilog;

namespace MyAcademyCQRS.CQRSPattern.Handlers.OrderHandlers;

public class UpdateOrderStatusCommandHandler
{
    private readonly AppDbContext _context;
    private readonly OrderSubject _orderSubject;

    public UpdateOrderStatusCommandHandler(AppDbContext context, OrderSubject orderSubject)
    {
        _context = context;
        _orderSubject = orderSubject;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand command)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == command.OrderId);
        if (order == null) return false;

        var oldStatus = order.Status;
        order.Status = command.NewStatus;
        await _context.SaveChangesAsync();

        // Observer — Durum değişikliği bildirimi
        _orderSubject.NotifyStatusChanged(order, oldStatus, command.NewStatus);

        Log.ForContext("Area", "Order")
           .Information("Sipariş #{OrderId} durumu güncellendi: {OldStatus} → {NewStatus}",
               order.Id, oldStatus, command.NewStatus);

        return true;
    }
}
