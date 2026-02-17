using AutoMapper;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Commands.ProductCommands;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.CQRSPattern.Handlers.ProductHandlers
{
    public class CreateProductCommandHandler(AppDbContext context, IMapper mapper)
    {
        public async Task Handle(CreateProductCommand command)
        {
            var product = mapper.Map<Product>(command);
            await context.AddAsync(product);
            await context.SaveChangesAsync();
        }
    }
}
