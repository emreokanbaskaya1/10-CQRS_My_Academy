using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Entities.Enums;

namespace MyAcademyCQRS.DesignPatterns.Observer;

public class OrderSubject
{
    private readonly List<IOrderObserver> _observers = new();

    public void Attach(IOrderObserver observer) => _observers.Add(observer);
    public void Detach(IOrderObserver observer) => _observers.Remove(observer);

    public void NotifyStatusChanged(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        foreach (var observer in _observers)
        {
            observer.OnOrderStatusChanged(order, oldStatus, newStatus);
        }
    }
}
