using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Commands.CategoryCommands;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers
{
    public class CreateCategoryCommandHandler
    {
        private readonly AppDbContext _context;

        public CreateCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateCategoryCommand command)
        {
            var category = new Category { Name= command.Name, ImageUrl = command.ImageUrl };
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();

        }
    }
}
