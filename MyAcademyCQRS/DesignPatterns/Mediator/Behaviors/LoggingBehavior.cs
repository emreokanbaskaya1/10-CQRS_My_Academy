using System.Diagnostics;
using MediatR;
using Serilog;

namespace MyAcademyCQRS.DesignPatterns.Mediator.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        Log.ForContext("Area", "Mediator")
           .Information("MediatR İstek Başladı: {RequestName} | {@Request}", requestName, request);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        Log.ForContext("Area", "Mediator")
           .Information("MediatR İstek Tamamlandı: {RequestName} | Süre: {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
