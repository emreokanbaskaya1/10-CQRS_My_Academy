using MediatR;
using MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers;
using MyAcademyCQRS.CQRSPattern.Handlers.OrderHandlers;
using MyAcademyCQRS.CQRSPattern.Handlers.ProductHandlers;
using MyAcademyCQRS.DesignPatterns.Mediator.Behaviors;
using MyAcademyCQRS.DesignPatterns.Observer;
using MyAcademyCQRS.DesignPatterns.UnitOfWork;
using MyAcademyCQRS.Services;
using System.Reflection;

namespace MyAcademyCQRS.Extensions;

public static class ServiceRegistrations
{
    public static void AddCQRSHandlers(this IServiceCollection services)
    {
        #region categoryHandlers
        services.AddScoped<GetCategoriesQueryHandler>();
        services.AddScoped<GetCategoryByIdQueryHandler>();
        services.AddScoped<UpdateCategoryCommandHandler>();
        services.AddScoped<CreateCategoryCommandHandler>();
        services.AddScoped<RemoveCategoryCommandHandler>();
        #endregion

        #region productHandlers
        services.AddScoped<GetProductsQueryHandler>();
        services.AddScoped<CreateProductCommandHandler>();
        services.AddScoped<GetProductByIdQueryHandler>();
        services.AddScoped<UpdateProductCommandHandler>();
        services.AddScoped<RemoveProductCommandHandler>();
        #endregion

        #region orderHandlers
        services.AddScoped<GetOrdersQueryHandler>();
        services.AddScoped<GetOrderByIdQueryHandler>();
        services.AddScoped<CreateOrderCommandHandler>();
        services.AddScoped<UpdateOrderStatusCommandHandler>();
        #endregion
    }

    public static void AddPackageExtensions(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Observer Pattern
        services.AddSingleton<OrderSubject>(sp =>
        {
            var subject = new OrderSubject();
            subject.Attach(new OrderLogObserver());
            subject.Attach(new OrderNotificationObserver());
            return subject;
        });

        // Google Cloud Storage
        services.AddSingleton<ICloudStorageService, GoogleCloudStorageService>();

        // MediatR Pipeline Behavior (LoggingBehavior)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }
}
