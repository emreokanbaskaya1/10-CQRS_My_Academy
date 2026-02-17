using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAcademyCQRS.CQRSPattern.Commands.ProductCommands;
using MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers;
using MyAcademyCQRS.CQRSPattern.Handlers.ProductHandlers;
using MyAcademyCQRS.CQRSPattern.Queries.ProductQueries;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    public class ProductController(GetProductsQueryHandler getProductsQueryHandler,
                                   GetCategoriesQueryHandler getCategoriesQueryHandler,
                                   CreateProductCommandHandler createProductCommandHandler,
                                   UpdateProductCommandHandler updateProductCommandHandler,
                                   RemoveProductCommandHandler removeProductCommandHandler,
                                   GetProductByIdQueryHandler getProductByIdQueryHandler,
                                   Services.ICloudStorageService blobService) : Controller
    {

        public async Task GetCategoriesAsync()
        {
            var categories = await getCategoriesQueryHandler.Handle();
            ViewBag.Categories = (from x in categories
                                  select new SelectListItem
                                  {
                                      Text= x.Name,
                                      Value = x.Id.ToString(),
                                  }).ToList();
        }


        public async Task<IActionResult> Index()
        {
            var products = await getProductsQueryHandler.Handle();
            return View(products);
        }

        public async Task<IActionResult> CreateProduct()
        {
            await GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imageUrl = await blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
                command = command with { ImageUrl = imageUrl };
            }
            await createProductCommandHandler.Handle(command);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UpdateProduct(int id)
        {
            await GetCategoriesAsync();
            var product = await getProductByIdQueryHandler.Handle(new GetProductByIdQuery(id));
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductCommand command, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imageUrl = await blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
                command = command with { ImageUrl = imageUrl };
            }
            await updateProductCommandHandler.Handle(command);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            await removeProductCommandHandler.Handle(new RemoveProductCommand(id));
            return RedirectToAction("Index");
        }

    }
}
