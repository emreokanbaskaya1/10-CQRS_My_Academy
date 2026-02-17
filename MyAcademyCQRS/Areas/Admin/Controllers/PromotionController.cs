using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.DesignPatterns.Observer;
using MyAcademyCQRS.Entities;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class PromotionController : Controller
{
    private readonly AppDbContext _context;
    private readonly Services.ICloudStorageService _blobService;
    private readonly PromotionLogObserver _promotionObserver = new();

    public PromotionController(AppDbContext context, Services.ICloudStorageService blobService) 
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Kampanya Yönetimi";
        return View(await _context.Promotions.OrderByDescending(p => p.StartDate).ToListAsync());
    }

    [HttpGet] public IActionResult Create() { ViewData["Title"] = "Yeni Kampanya"; return View(); }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Create(Promotion promo, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            promo.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
        }
        await _context.Promotions.AddAsync(promo);
        await _context.SaveChangesAsync();
        _promotionObserver.OnPromotionCreated(promo);
        TempData["Success"] = "Kampanya oluşturuldu.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        ViewData["Title"] = "Kampanya Düzenle";
        var p = await _context.Promotions.FindAsync(id);
        return p == null ? NotFound() : View(p);
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Update(Promotion promo, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            promo.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
        }
        _context.Promotions.Update(promo);
        await _context.SaveChangesAsync();
        _promotionObserver.OnPromotionUpdated(promo);
        TempData["Success"] = "Kampanya güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _context.Promotions.FindAsync(id);
        if (p != null) { _context.Promotions.Remove(p); await _context.SaveChangesAsync(); TempData["Success"] = "Kampanya silindi."; }
        return RedirectToAction("Index");
    }
}
