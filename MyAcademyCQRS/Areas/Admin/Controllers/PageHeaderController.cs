using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Services;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class PageHeaderController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICloudStorageService _blobService;

    public PageHeaderController(AppDbContext context, ICloudStorageService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Sayfa Baþlýklarý";
        var headers = await _context.PageHeaders.OrderBy(h => h.PageName).ToListAsync();
        return View(headers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Yeni Sayfa Baþlýðý";
        ViewBag.PageNames = new[] { "Shop", "Gallery", "Contact", "Testimonial", "Cart" };
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PageHeader header, IFormFile? imageFile)
    {
        try
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Lütfen bir arka plan görseli seçin.";
                ViewBag.PageNames = new[] { "Shop", "Gallery", "Contact", "Testimonial", "Cart" };
                return View(header);
            }

            // Ayný sayfa için zaten bir header var mý kontrol et
            var existing = await _context.PageHeaders.FirstOrDefaultAsync(h => h.PageName == header.PageName);
            if (existing != null)
            {
                TempData["Error"] = $"{header.PageName} sayfasý için zaten bir baþlýk tanýmlý. Lütfen düzenleyin.";
                ViewBag.PageNames = new[] { "Shop", "Gallery", "Contact", "Testimonial", "Cart" };
                return View(header);
            }

            using var stream = imageFile.OpenReadStream();
            header.BackgroundImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);

            // Otomatik baþlýk oluþtur (Türkçe)
            header.Title = header.PageName switch
            {
                "Shop" => "Ürünlerimiz",
                "Gallery" => "Galeri",
                "Contact" => "Ýletiþim",
                "Testimonial" => "Yorum Býrakýn",
                "Cart" => "Sepet",
                _ => header.PageName
            };
            
            header.Subtitle = null; // Subtitle kullanmýyoruz

            await _context.PageHeaders.AddAsync(header);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{header.PageName} sayfasý için görsel baþarýyla eklendi!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Hata oluþtu: {ex.Message}";
            ViewBag.PageNames = new[] { "Shop", "Gallery", "Contact", "Testimonial", "Cart" };
            return View(header);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        ViewData["Title"] = "Sayfa Baþlýðý Düzenle";
        var header = await _context.PageHeaders.FindAsync(id);
        if (header == null) return NotFound();
        ViewBag.PageNames = new[] { "Shop", "Gallery", "Contact", "Testimonial", "Cart" };
        return View(header);
    }

    [HttpPost]
    public async Task<IActionResult> Update(PageHeader header, IFormFile? imageFile)
    {
        try
        {
            var existing = await _context.PageHeaders.FindAsync(header.Id);
            if (existing == null) return NotFound();

            existing.IsActive = header.IsActive;
            existing.PageName = header.PageName;

            // Otomatik baþlýk güncelle
            existing.Title = header.PageName switch
            {
                "Shop" => "Ürünlerimiz",
                "Gallery" => "Galeri",
                "Contact" => "Ýletiþim",
                "Testimonial" => "Yorum Býrakýn",
                "Cart" => "Sepet",
                _ => header.PageName
            };
            
            existing.Subtitle = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existing.BackgroundImageUrl))
                    await _blobService.DeleteAsync(existing.BackgroundImageUrl);

                using var stream = imageFile.OpenReadStream();
                existing.BackgroundImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"{existing.PageName} sayfasý görseli baþarýyla güncellendi!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Hata oluþtu: {ex.Message}";
            ViewBag.PageNames = new[] { "Shop", "Gallery", "Contact", "Testimonial", "Cart" };
            return View(header);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var header = await _context.PageHeaders.FindAsync(id);
        if (header != null)
        {
            if (!string.IsNullOrEmpty(header.BackgroundImageUrl))
                await _blobService.DeleteAsync(header.BackgroundImageUrl);

            _context.PageHeaders.Remove(header);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{header.PageName} sayfasý baþlýðý silindi.";
        }
        return RedirectToAction("Index");
    }
}
