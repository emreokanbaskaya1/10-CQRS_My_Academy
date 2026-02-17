using MyAcademyCQRS.CQRSPattern.Results.CategoryResults;

namespace MyAcademyCQRS.CQRSPattern.Results.ProductResults;

    public record GetProductsQueryResult(int Id, 
                                         string Name, 
                                         string Description, 
                                         decimal Price, 
                                         string ImageUrl, 
                                         int CategoryId, 
                                         GetCategoriesQueryResult Category);
   
