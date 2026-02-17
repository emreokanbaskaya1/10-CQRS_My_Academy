using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.DesignPatterns.Mediator.Commands;

using Microsoft.AspNetCore.Authorization;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class TestimonialController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public TestimonialController(AppDbContext context, IMediator mediator) { _context = context; _mediator = mediator; }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Yorum Yönetimi";
        return View(await _context.Testimonials.OrderByDescending(t => t.CreatedDate).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        await _mediator.Send(new ApproveTestimonialCommand { TestimonialId = id, IsApproved = true });
        TempData["Success"] = "Yorum onaylandı.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        await _mediator.Send(new ApproveTestimonialCommand { TestimonialId = id, IsApproved = false });
        TempData["Success"] = "Yorum reddedildi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _context.Testimonials.FindAsync(id);
        if (t != null) { _context.Testimonials.Remove(t); await _context.SaveChangesAsync(); TempData["Success"] = "Yorum silindi."; }
        return RedirectToAction("Index");
    }
}
