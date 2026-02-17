using AutoMapper;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Queries.ProductQueries;
using MyAcademyCQRS.CQRSPattern.Results.ProductResults;

namespace MyAcademyCQRS.CQRSPattern.Handlers.ProductHandlers
{
    public class GetProductByIdQueryHandler(AppDbContext context, IMapper mapper)
    {

        public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery query)
        {
            var product = await context.Products.FindAsync(query.Id);
            return mapper.Map<GetProductByIdQueryResult>(product);
        }

    }
}
