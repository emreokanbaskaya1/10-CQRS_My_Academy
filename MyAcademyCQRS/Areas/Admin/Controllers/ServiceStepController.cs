using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Services;
using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ServiceStepController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICloudStorageService _blobService;
    
    public ServiceStepController(AppDbContext context, ICloudStorageService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Feature Steps";
        return View(await _context.ServiceSteps.OrderBy(s => s.StepNumber).Take(8).ToListAsync());
    }

    [HttpGet] 
    public IActionResult Create() 
    { 
        ViewData["Title"] = "Yeni Step"; 
        return View(); 
    }

    [HttpPost]
    public async Task<IActionResult> Create(ServiceStep step, IFormFile? imageFile)
    {
        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                step.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            }

            await _context.ServiceSteps.AddAsync(step);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Step başarıyla eklendi.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Hata oluştu: {ex.Message}";
            return View(step);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        ViewData["Title"] = "Step Düzenle";
        var s = await _context.ServiceSteps.FindAsync(id);
        return s == null ? NotFound() : View(s);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ServiceStep step, IFormFile? imageFile)
    {
        try
        {
            var existing = await _context.ServiceSteps.FindAsync(step.Id);
            if (existing == null) return NotFound();

            existing.StepNumber = step.StepNumber;
            existing.Title = step.Title;
            existing.Description = step.Description;
            existing.IconUrl = step.IconUrl;

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existing.ImageUrl))
                    await _blobService.DeleteAsync(existing.ImageUrl);

                using var stream = imageFile.OpenReadStream();
                existing.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Step başarıyla güncellendi.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Hata oluştu: {ex.Message}";
            return View(step);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _context.ServiceSteps.FindAsync(id);
        if (s != null) 
        { 
            if (!string.IsNullOrEmpty(s.ImageUrl))
                await _blobService.DeleteAsync(s.ImageUrl);

            _context.ServiceSteps.Remove(s); 
            await _context.SaveChangesAsync(); 
            TempData["Success"] = "Step silindi."; 
        }
        return RedirectToAction("Index");
    }
}
