using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Queries.CategoryQueries;
using MyAcademyCQRS.CQRSPattern.Results.CategoryResults;

namespace MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers
{
    public class GetCategoryByIdQueryHandler
    {
        private readonly AppDbContext _context;

        public GetCategoryByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetCategoryByIdQueryResult> Handle(GetCategoryByIdQuery query)
        {
            var category = await _context.Categories.FindAsync(query.Id);

            return new GetCategoryByIdQueryResult
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            };
        }



    }
}
