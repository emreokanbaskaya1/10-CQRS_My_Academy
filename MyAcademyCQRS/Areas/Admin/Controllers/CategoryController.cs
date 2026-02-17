using Microsoft.AspNetCore.Mvc;
using MyAcademyCQRS.CQRSPattern.Commands.CategoryCommands;
using MyAcademyCQRS.CQRSPattern.Handlers.CategoryHandlers;
using MyAcademyCQRS.CQRSPattern.Queries.CategoryQueries;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly GetCategoriesQueryHandler _getCategoriesQueryHandler;
        private readonly GetCategoryByIdQueryHandler _getCategoryByIdQueryHandler;
        private readonly UpdateCategoryCommandHandler _updateCategoryCommandHandler;
        private readonly CreateCategoryCommandHandler _createCategoryCommandHandler;
        private readonly RemoveCategoryCommandHandler _removeCategoryCommandHandler;
        private readonly Services.ICloudStorageService _blobService;

        public CategoryController(GetCategoriesQueryHandler getCategoriesQueryHandler, GetCategoryByIdQueryHandler getCategoryByIdQueryHandler, UpdateCategoryCommandHandler updateCategoryCommandHandler, CreateCategoryCommandHandler createCategoryCommandHandler, RemoveCategoryCommandHandler removeCategoryCommandHandler, Services.ICloudStorageService blobService)
        {
            _getCategoriesQueryHandler = getCategoriesQueryHandler;
            _getCategoryByIdQueryHandler = getCategoryByIdQueryHandler;
            _updateCategoryCommandHandler = updateCategoryCommandHandler;
            _createCategoryCommandHandler = createCategoryCommandHandler;
            _removeCategoryCommandHandler = removeCategoryCommandHandler;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _getCategoriesQueryHandler.Handle();
            return View(categories);
        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _getCategoryByIdQueryHandler.Handle(new GetCategoryByIdQuery(id));
            return View(category);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryCommand command, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Check if there is an existing image to delete? 
                // Handler takes care of updating, but here we don't have the old URL easily accessible without querying.
                // For simplicity and to stick to CQRS pattern where Controller puts data into Command, we just upload.
                // Deletion of old image is an optimization we can skip for now or would need a Query first.
                // Given the user wants the "upload logic", we focus on that.
                
                // Better approach: Query existing to delete old image? 
                // Controller shouldn't do too much logic. But cleaning up old images is good practice.
                // Let's implement simple upload for now.
                
                using var stream = imageFile.OpenReadStream();
                command.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            }
            await _updateCategoryCommandHandler.Handle(command);
            return RedirectToAction("Index");
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                command.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            }
            await _createCategoryCommandHandler.Handle(command);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _removeCategoryCommandHandler.Handle(new RemoveCategoryCommand(id));
            return RedirectToAction("Index");
        }

    }
}
