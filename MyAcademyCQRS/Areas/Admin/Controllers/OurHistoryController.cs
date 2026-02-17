using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Services;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OurHistoryController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICloudStorageService _blobService;
    public OurHistoryController(AppDbContext context, ICloudStorageService blobService) { _context = context; _blobService = blobService; }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Tarihçemiz";
        return View(await _context.OurHistories.ToListAsync());
    }

    [HttpGet] public IActionResult Create() { ViewData["Title"] = "Yeni Tarihçe"; return View(); }

    [HttpPost]
    public async Task<IActionResult> Create(OurHistory history, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            history.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
        }
        await _context.OurHistories.AddAsync(history);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tarihçe eklendi.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        ViewData["Title"] = "Tarihçe Düzenle";
        var h = await _context.OurHistories.FindAsync(id);
        return h == null ? NotFound() : View(h);
    }

    [HttpPost]
    public async Task<IActionResult> Update(OurHistory history, IFormFile? imageFile)
    {
        var existing = await _context.OurHistories.FindAsync(history.Id);
        if (existing == null) return NotFound();
        existing.Title = history.Title;
        existing.Content = history.Content;
        existing.SinceYear = history.SinceYear;
        if (imageFile != null && imageFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.ImageUrl)) await _blobService.DeleteAsync(existing.ImageUrl);
            using var stream = imageFile.OpenReadStream();
            existing.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
        }
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tarihçe güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var h = await _context.OurHistories.FindAsync(id);
        if (h != null) { if (!string.IsNullOrEmpty(h.ImageUrl)) await _blobService.DeleteAsync(h.ImageUrl); _context.OurHistories.Remove(h); await _context.SaveChangesAsync(); TempData["Success"] = "Tarihçe silindi."; }
        return RedirectToAction("Index");
    }
}
