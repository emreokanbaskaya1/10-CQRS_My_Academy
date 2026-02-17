using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Services;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SliderController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICloudStorageService _blobService;

    public SliderController(AppDbContext context, ICloudStorageService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Slider Yönetimi";
        var sliders = await _context.Sliders.OrderBy(s => s.DisplayOrder).ToListAsync();
        return View(sliders);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Yeni Slider";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Slider slider, IFormFile? imageFile)
    {
        try
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Lütfen bir görsel seçin.";
                return View(slider);
            }

            using var stream = imageFile.OpenReadStream();
            slider.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            
            // ButtonUrl boşsa ImageUrl'i kullan
            if (string.IsNullOrEmpty(slider.ButtonUrl))
                slider.ButtonUrl = slider.ImageUrl;

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Slider '{slider.Title}' başarıyla eklendi!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Hata oluştu: {ex.Message}";
            return View(slider);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        ViewData["Title"] = "Slider Düzenle";
        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();
        return View(slider);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Slider slider, IFormFile? imageFile)
    {
        var existing = await _context.Sliders.FindAsync(slider.Id);
        if (existing == null) return NotFound();

        existing.Title = slider.Title;
        existing.Description = slider.Description;
        existing.ButtonText = slider.ButtonText;
        existing.ButtonUrl = slider.ButtonUrl;
        existing.DisplayOrder = slider.DisplayOrder;
        existing.IsActive = slider.IsActive;

        if (imageFile != null && imageFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.ImageUrl))
                await _blobService.DeleteAsync(existing.ImageUrl);

            using var stream = imageFile.OpenReadStream();
            existing.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            existing.ButtonUrl = existing.ImageUrl; // Auto-populate ButtonUrl
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Slider başarıyla güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider != null)
        {
            if (!string.IsNullOrEmpty(slider.ImageUrl))
                await _blobService.DeleteAsync(slider.ImageUrl);
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Slider silindi.";
        }
        return RedirectToAction("Index");
    }
}
