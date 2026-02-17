using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Services;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class PhotoGalleryController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICloudStorageService _blobService;
    public PhotoGalleryController(AppDbContext context, ICloudStorageService blobService) { _context = context; _blobService = blobService; }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Galeri Yönetimi";
        return View(await _context.PhotoGalleries.OrderByDescending(p => p.UploadDate).ToListAsync());
    }

    [HttpGet] public IActionResult Create() { ViewData["Title"] = "Yeni Fotoğraf"; return View(); }

    [HttpPost]
    public async Task<IActionResult> Create(PhotoGallery photo, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            photo.ImageUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
        }
        photo.UploadDate = DateTime.UtcNow;
        await _context.PhotoGalleries.AddAsync(photo);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Fotoğraf eklendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _context.PhotoGalleries.FindAsync(id);
        if (p != null) { if (!string.IsNullOrEmpty(p.ImageUrl)) await _blobService.DeleteAsync(p.ImageUrl); _context.PhotoGalleries.Remove(p); await _context.SaveChangesAsync(); TempData["Success"] = "Fotoğraf silindi."; }
        return RedirectToAction("Index");
    }
}
