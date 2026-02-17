using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ContactController : Controller
{
    private readonly AppDbContext _context;
    public ContactController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Mesajlar";
        return View(await _context.Contacts.OrderByDescending(c => c.CreatedDate).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var c = await _context.Contacts.FindAsync(id);
        if (c != null) { c.IsRead = true; await _context.SaveChangesAsync(); }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _context.Contacts.FindAsync(id);
        if (c != null) { _context.Contacts.Remove(c); await _context.SaveChangesAsync(); TempData["Success"] = "Mesaj silindi."; }
        return RedirectToAction("Index");
    }
}
