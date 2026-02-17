using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Commands.CategoryCommands;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers
{
    public class UpdateCategoryCommandHandler
    {
        private readonly AppDbContext _context;

        public UpdateCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateCategoryCommand command)
        {
            var category = await _context.Categories.FindAsync(command.Id);
            if (category != null)
            {
                category.Name = command.Name;
                if (!string.IsNullOrEmpty(command.ImageUrl))
                {
                    category.ImageUrl = command.ImageUrl;
                }
                await _context.SaveChangesAsync();
            }
        }

    }
}
