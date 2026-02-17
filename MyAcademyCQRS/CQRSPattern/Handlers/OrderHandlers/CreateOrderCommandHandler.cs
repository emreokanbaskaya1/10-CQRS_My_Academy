using MyAcademyCQRS.CQRSPattern.Commands.OrderCommands;
using MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.OrderValidation;
using MyAcademyCQRS.DesignPatterns.Observer;
using MyAcademyCQRS.DesignPatterns.UnitOfWork;
using MyAcademyCQRS.Entities;
using Serilog;

namespace MyAcademyCQRS.CQRSPattern.Handlers.OrderHandlers;

public class CreateOrderCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OrderSubject _orderSubject;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, OrderSubject orderSubject)
    {
        _unitOfWork = unitOfWork;
        _orderSubject = orderSubject;
    }

    public async Task<(bool Success, string Message, int? OrderId)> Handle(CreateOrderCommand command)
    {
        // 1. Chain of Responsibility — Sipariş validasyonu
        var availabilityHandler = new ProductAvailabilityHandler(_unitOfWork);
        var priceHandler = new PriceCalculationHandler(_unitOfWork);
        var promoHandler = new PromoCodeValidationHandler(_unitOfWork);

        availabilityHandler.SetNext(priceHandler).SetNext(promoHandler);

        var context = new OrderValidationContext
        {
            Items = command.Items.Select(i => (i.ProductId, i.Quantity)).ToList(),
            PromoCode = command.PromoCode
        };

        var validationResult = await availabilityHandler.Handle(context);
        if (!validationResult.Success)
        {
            Log.ForContext("Area", "Order")
               .Warning("Sipariş validasyonu başarısız: {Message}", validationResult.Message);
            return (false, validationResult.Message, null);
        }

        // 2. Unit of Work — Transaction ile sipariş oluşturma
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var order = new Order
            {
                CustomerName = command.CustomerName,
                Email = command.Email,
                Phone = command.Phone,
                Note = command.Note,
                PromoCode = command.PromoCode,
                DiscountAmount = context.DiscountAmount,
                TotalPrice = context.CalculatedTotal,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            foreach (var item in command.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product!.Price
                };
                await _unitOfWork.OrderItems.AddAsync(orderItem);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            // 3. Observer — Sipariş oluşturma bildirimi
            _orderSubject.NotifyStatusChanged(order, Entities.Enums.OrderStatus.Pending, Entities.Enums.OrderStatus.Pending);

            return (true, "Sipariş başarıyla oluşturuldu.", order.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            Log.ForContext("Area", "Order")
               .Error(ex, "Sipariş oluşturulurken hata oluştu.");
            return (false, "Sipariş oluşturulurken bir hata oluştu.", null);
        }
    }
}
