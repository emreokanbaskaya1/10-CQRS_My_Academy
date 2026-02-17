using Microsoft.EntityFrameworkCore.Storage;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.DesignPatterns.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<Order>? _orders;
    private IRepository<OrderItem>? _orderItems;
    private IRepository<Product>? _products;
    private IRepository<Promotion>? _promotions;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<Order> Orders
        => _orders ??= new Repository<Order>(_context);

    public IRepository<OrderItem> OrderItems
        => _orderItems ??= new Repository<OrderItem>(_context);

    public IRepository<Product> Products
        => _products ??= new Repository<Product>(_context);

    public IRepository<Promotion> Promotions
        => _promotions ??= new Repository<Promotion>(_context);

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
