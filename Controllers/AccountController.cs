using Microsoft.AspNetCore.Mvc;
using Nawarut_8250_L005.Models;
using Nawarut_8250_L005.Services;

namespace Nawarut_8250_L005.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _userService.Authenticate(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือรหัสผ่านไม่ถูกต้อง โปรดลองอีกครั้ง");
            return View(model);
        }

        if (user.Status == "Inactive")
        {
            ModelState.AddModelError(string.Empty, "บัญชีผู้ใช้นี้ถูกระงับการใช้งาน กรุณาติดต่อผู้ดูแลระบบ");
            return View(model);
        }

        // Store user info in session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetString("UserRole", user.Role);

        TempData["SuccessMessage"] = $"ยินดีต้อนรับคุณ {user.FullName} เข้าสู่ระบบ";

        // Admin redirection
        if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Users", "Admin");
        }

        // Regular User redirection
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if email already exists
        if (_userService.GetByEmail(model.Email) != null)
        {
            ModelState.AddModelError("Email", "อีเมลนี้มีผู้ใช้งานแล้วในระบบ");
            return View(model);
        }

        var newUser = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            Role = "User",
            Status = "Active",
            CreatedAt = DateTime.Now
        };

        bool created = _userService.Add(newUser);
        if (!created)
        {
            ModelState.AddModelError(string.Empty, "ไม่สามารถลงทะเบียนได้ กรุณาลองใหม่อีกครั้ง");
            return View(model);
        }

        TempData["SuccessMessage"] = "สมัครสมาชิกสำเร็จเรียบร้อยแล้ว! กรุณาเข้าสู่ระบบด้วยบัญชีของคุณ";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["InfoMessage"] = "ออกจากระบบเรียบร้อยแล้ว";
        return RedirectToAction("Login");
    }
}
