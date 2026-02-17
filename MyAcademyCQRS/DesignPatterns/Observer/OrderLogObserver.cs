using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Entities.Enums;
using Serilog;

namespace MyAcademyCQRS.DesignPatterns.Observer;

public class OrderLogObserver : IOrderObserver
{
    public void OnOrderStatusChanged(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        Log.ForContext("Area", "Order")
           .Information("Sipariş #{OrderId} durumu değişti: {OldStatus} → {NewStatus} | Müşteri: {Customer}",
               order.Id, oldStatus, newStatus, order.CustomerName);
    }
}
