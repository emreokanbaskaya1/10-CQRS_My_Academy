using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Entities.Enums;
using Serilog;

namespace MyAcademyCQRS.DesignPatterns.Observer;

public class OrderNotificationObserver : IOrderObserver
{
    public void OnOrderStatusChanged(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        Log.ForContext("Area", "Notification")
           .Information("BİLDİRİM: Sipariş #{OrderId} ({Customer}) yeni durumu: {NewStatus}",
               order.Id, order.CustomerName, newStatus);
    }
}
