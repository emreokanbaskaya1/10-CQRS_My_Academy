using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.Entities;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ServiceController : Controller
{
    private readonly AppDbContext _context;
    private readonly Services.ICloudStorageService _blobService;

    public ServiceController(AppDbContext context, Services.ICloudStorageService blobService) 
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Hizmet Yönetimi";
        return View(await _context.Services.OrderBy(s => s.DisplayOrder).ToListAsync());
    }

    [HttpGet] public IActionResult Create() { ViewData["Title"] = "Yeni Hizmet"; return View(); }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Create(Service service, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            service.IconUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
        }
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Hizmet eklendi.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        ViewData["Title"] = "Hizmet Düzenle";
        var s = await _context.Services.FindAsync(id);
        return s == null ? NotFound() : View(s);
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Update(Service service, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            service.IconUrl = await _blobService.UploadAsync(stream, imageFile.FileName, imageFile.ContentType);
        }
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Hizmet güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _context.Services.FindAsync(id);
        if (s != null) { _context.Services.Remove(s); await _context.SaveChangesAsync(); TempData["Success"] = "Hizmet silindi."; }
        return RedirectToAction("Index");
    }
}
