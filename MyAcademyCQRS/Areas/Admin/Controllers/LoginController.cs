using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAcademyCQRS.Entities;
using MyAcademyCQRS.Models;

namespace MyAcademyCQRS.Areas.Admin.Controllers;

[Area("Admin")]
public class LoginController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new AdminLoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(AdminLoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new AdminRegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(AdminRegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Login", new { area = "Admin" });
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
