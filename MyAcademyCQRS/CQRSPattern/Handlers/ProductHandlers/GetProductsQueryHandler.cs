using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Results.CategoryResults;
using MyAcademyCQRS.CQRSPattern.Results.ProductResults;

namespace MyAcademyCQRS.CQRSPattern.Handlers.ProductHandlers
{
    public class GetProductsQueryHandler(AppDbContext _context, IMapper _mapper)
    {

        public async Task<List<GetProductsQueryResult>> Handle()
        {
            var products = await _context.Products.Include(x => x.Category).ToListAsync();
            return _mapper.Map<List<GetProductsQueryResult>>(products);
        }




    }
}
