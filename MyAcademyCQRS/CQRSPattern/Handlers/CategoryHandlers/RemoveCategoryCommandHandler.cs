using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Commands.CategoryCommands;

namespace MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers
{
    public class RemoveCategoryCommandHandler
    {
        private readonly AppDbContext _context;

        public RemoveCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveCategoryCommand command)
        {
            var category = await _context.Categories.FindAsync(command.Id);
            _context.Remove(category);
            await _context.SaveChangesAsync();

        }
    }
}
