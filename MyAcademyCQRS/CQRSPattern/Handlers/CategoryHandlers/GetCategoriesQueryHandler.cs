using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Results.CategoryResults;

namespace MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers
{
    public class GetCategoriesQueryHandler
    {
        private readonly AppDbContext _context;
        public GetCategoriesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetCategoriesQueryResult>> Handle()
        {
            var categories = await _context.Categories.ToListAsync();

            return categories.Select(c=> new GetCategoriesQueryResult
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl
            }).ToList();
        }

    }
}
