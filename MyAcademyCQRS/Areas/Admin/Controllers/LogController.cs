using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class LogController : Controller
{
    private readonly AppDbContext _context;
    public LogController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? area, string? level)
    {
        ViewData["Title"] = "Log Görüntüleyici";
        var query = _context.AppLogs.AsQueryable();

        if (!string.IsNullOrEmpty(area))
            query = query.Where(l => l.Area == area);
        if (!string.IsNullOrEmpty(level))
            query = query.Where(l => l.Level == level);

        ViewBag.SelectedArea = area;
        ViewBag.SelectedLevel = level;

        var logs = await query.OrderByDescending(l => l.TimeStamp).Take(100).ToListAsync();
        return View(logs);
    }
}
