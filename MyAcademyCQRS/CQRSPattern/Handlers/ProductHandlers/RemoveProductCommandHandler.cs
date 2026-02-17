using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Commands.ProductCommands;

namespace MyAcademyCQRS.CQRSPattern.Handlers.ProductHandlers
{
    public class RemoveProductCommandHandler(AppDbContext context)
    {
        public async Task Handle(RemoveProductCommand command)
        {
            var product = await context.Products.FindAsync(command.Id);
            context.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
