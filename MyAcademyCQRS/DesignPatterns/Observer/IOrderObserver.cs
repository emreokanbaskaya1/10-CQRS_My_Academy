using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Entities.Enums;

namespace MyAcademyCQRS.DesignPatterns.Observer;

public interface IOrderObserver
{
    void OnOrderStatusChanged(Order order, OrderStatus oldStatus, OrderStatus newStatus);
}
