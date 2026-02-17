using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.DesignPatterns.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    IRepository<Product> Products { get; }
    IRepository<Promotion> Promotions { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
