using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.CQRSPattern.Commands.OrderCommands;
using MyAcademyCQRS.CQRSPattern.Handlers.OrderHandlers;
using MyAcademyCQRS.DesignPatterns.Mediator.Commands;
using MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.ContactValidation;
using Newtonsoft.Json;

namespace MyAcademyCQRS.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;
    private readonly CreateOrderCommandHandler _createOrderHandler;

    public HomeController(AppDbContext context, IMediator mediator, CreateOrderCommandHandler createOrderHandler)
    {
        _context = context;
        _mediator = mediator;
        _createOrderHandler = createOrderHandler;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Ana Sayfa";
        ViewBag.Sliders = await _context.Sliders.Where(s => s.IsActive).OrderBy(s => s.DisplayOrder).ToListAsync();
        ViewBag.ServiceSteps = await _context.ServiceSteps.OrderBy(s => s.StepNumber).ToListAsync();
        ViewBag.OurHistory = await _context.OurHistories.FirstOrDefaultAsync();
        ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).Include(c => c.Products.Where(p => p.IsActive)).ToListAsync();
        ViewBag.Products = await _context.Products.Where(p => p.IsActive).Take(8).ToListAsync();
        ViewBag.Services = await _context.Services.OrderBy(s => s.DisplayOrder).ToListAsync();
        ViewBag.Gallery = await _context.PhotoGalleries.OrderByDescending(g => g.UploadDate).Take(8).ToListAsync();
        ViewBag.Promotions = await _context.Promotions.Where(p => p.IsActive && p.EndDate >= DateTime.UtcNow).ToListAsync();
        ViewBag.Testimonials = await _context.Testimonials.Where(t => t.IsApproved).OrderByDescending(t => t.CreatedDate).Take(6).ToListAsync();
        return View();
    }

    public async Task<IActionResult> Shop()
    {
        ViewData["Title"] = "Ürünler";
        ViewBag.PageHeader = await _context.PageHeaders.FirstOrDefaultAsync(h => h.PageName == "Shop" && h.IsActive);
        ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
        var products = await _context.Products.Where(p => p.IsActive).Include(p => p.Category).ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Gallery()
    {
        ViewData["Title"] = "Galeri";
        ViewBag.PageHeader = await _context.PageHeaders.FirstOrDefaultAsync(h => h.PageName == "Gallery" && h.IsActive);
        var photos = await _context.PhotoGalleries.OrderByDescending(g => g.UploadDate).ToListAsync();
        return View(photos);
    }

    [HttpGet]
    public async Task<IActionResult> Contact()
    {
        ViewData["Title"] = "İletişim";
        ViewBag.PageHeader = await _context.PageHeaders.FirstOrDefaultAsync(h => h.PageName == "Contact" && h.IsActive);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Contact(SendContactMessageCommand command)
    {
        // Chain of Responsibility — Contact Validation
        var spamHandler = new SpamCheckHandler();
        var emailHandler = new EmailValidationHandler();
        spamHandler.SetNext(emailHandler);

        var context = new ContactValidationContext
        {
            FullName = command.FullName,
            Email = command.Email,
            Subject = command.Subject,
            Message = command.Message
        };

        var result = spamHandler.Handle(context).Result;
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return View();
        }

        // MediatR — Mediator Pattern
        await _mediator.Send(command);
        TempData["Success"] = "Mesajınız başarıyla gönderildi!";
        return RedirectToAction("Contact");
    }

    [HttpGet]
    public async Task<IActionResult> CreateTestimonial()
    {
        ViewData["Title"] = "Yorum Bırak";
        ViewBag.PageHeader = await _context.PageHeaders.FirstOrDefaultAsync(h => h.PageName == "Testimonial" && h.IsActive);
        return View("Testimonial");
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestimonial(CreateTestimonialCommand command)
    {
        var result = await _mediator.Send(command);
        if (result)
            TempData["Success"] = "Yorumunuz başarıyla gönderildi! Admin onayından sonra yayınlanacaktır.";
        else
            TempData["Error"] = "Yorum gönderilirken bir hata oluştu.";
        return RedirectToAction("CreateTestimonial");
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity = 1)
    {
        var cart = GetCart();
        var existingIndex = cart.FindIndex(c => c.ProductId == productId);
        if (existingIndex >= 0)
            cart[existingIndex] = new OrderItemDto { ProductId = productId, Quantity = cart[existingIndex].Quantity + quantity };
        else
            cart.Add(new OrderItemDto { ProductId = productId, Quantity = quantity });

        SaveCart(cart);

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { success = true, message = "Ürün sepete eklendi!", cartCount = cart.Sum(c => c.Quantity) });

        TempData["CartSuccess"] = "Ürün sepete eklendi!";
        return Redirect(Request.Headers.Referer.ToString() ?? "/");
    }

    public async Task<IActionResult> Cart()
    {
        ViewData["Title"] = "Sepet";
        ViewBag.PageHeader = await _context.PageHeaders.FirstOrDefaultAsync(h => h.PageName == "Cart" && h.IsActive);
        var cart = GetCart();
        var productIds = cart.Select(c => c.ProductId).ToList();
        ViewBag.Products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(string customerName, string email, string? phone, string? note, string? promoCode)
    {
        var cart = GetCart();
        if (!cart.Any())
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return Json(new { success = false, message = "Sepetiniz boş!" });
            
            TempData["Error"] = "Sepetiniz boş!";
            return RedirectToAction("Cart");
        }

        var command = new CreateOrderCommand
        {
            CustomerName = customerName,
            Email = email,
            Phone = phone,
            Note = note,
            PromoCode = promoCode,
            Items = cart
        };

        var result = await _createOrderHandler.Handle(command);
        
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            if (result.Success)
            {
                HttpContext.Session.Remove("Cart");
                return Json(new { success = true, message = $"Siparişiniz #{result.OrderId} numarası ile oluşturuldu!", orderId = result.OrderId });
            }
            return Json(new { success = false, message = result.Message });
        }

        if (result.Success)
        {
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = $"Siparişiniz #{result.OrderId} numarası ile oluşturuldu!";
            return RedirectToAction("Index");
        }

        TempData["Error"] = result.Message;
        return RedirectToAction("Cart");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(c => c.ProductId == productId);
        SaveCart(cart);
        return RedirectToAction("Cart");
    }

    private List<OrderItemDto> GetCart()
    {
        var json = HttpContext.Session.GetString("Cart");
        return string.IsNullOrEmpty(json)
            ? new List<OrderItemDto>()
            : JsonConvert.DeserializeObject<List<OrderItemDto>>(json) ?? new();
    }

    private void SaveCart(List<OrderItemDto> cart)
    {
        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }
}
