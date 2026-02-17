using AutoMapper;
using MyAcademyCQRS.CQRSPattern.Results.CategoryResults;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.Mappings
{
    public class CategoryMappings: Profile
    {
        public CategoryMappings()
        {
            CreateMap<Category, GetCategoriesQueryResult>();
        }
    }
}
