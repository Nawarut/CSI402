using Microsoft.AspNetCore.Mvc;
using Nawarut_8250_L005.Models;
using Nawarut_8250_L005.Services;

namespace Nawarut_8250_L005.Controllers;

public class AdminController : Controller
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    // Dashboard
    public IActionResult Index()
    {
        var users = _userService.GetAll();
        ViewBag.TotalUsers = users.Count;
        ViewBag.ActiveUsers = users.Count(u => u.Status == "Active");
        ViewBag.AdminCount = users.Count(u => u.Role == "Admin");
        return View();
    }

    // User List (UserList)
    public IActionResult Users(string? search = null, string? roleFilter = null)
    {
        var users = _userService.GetAll();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            users = users.Where(u =>
                u.FullName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                u.PhoneNumber.Contains(s, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(roleFilter) && roleFilter != "All")
        {
            users = users.Where(u => u.Role.Equals(roleFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        ViewBag.Search = search;
        ViewBag.RoleFilter = roleFilter;
        ViewBag.TotalUsers = _userService.GetAll().Count;

        return View(users);
    }

    // Alias for UserList as requested in assessment prompt
    public IActionResult UserList(string? search = null, string? roleFilter = null)
    {
        return RedirectToAction(nameof(Users), new { search, roleFilter });
    }

    // Add User (GET)
    [HttpGet]
    public IActionResult AddUser()
    {
        return View(new AddUserViewModel());
    }

    // Add User (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddUser(AddUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_userService.GetByEmail(model.Email) != null)
        {
            ModelState.AddModelError("Email", "อีเมลนี้มีผู้ใช้งานแล้วในระบบ กรุณาใช้อีเมลอื่น");
            return View(model);
        }

        var newUser = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Role = model.Role,
            Status = model.Status,
            Password = model.Password,
            CreatedAt = DateTime.Now
        };

        bool created = _userService.Add(newUser);
        if (!created)
        {
            ModelState.AddModelError(string.Empty, "เกิดข้อผิดพลาด ไม่สามารถเพิ่มผู้ใช้ได้");
            return View(model);
        }

        TempData["SuccessMessage"] = $"เพิ่มผู้ใช้งาน \"{newUser.FullName}\" สำเร็จเรียบร้อยแล้ว";
        return RedirectToAction(nameof(Users));
    }

    // Delete User
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteUser(int id)
    {
        var user = _userService.GetById(id);
        if (user != null)
        {
            _userService.Delete(id);
            TempData["SuccessMessage"] = $"ลบผู้ใช้งาน \"{user.FullName}\" เรียบร้อยแล้ว";
        }
        else
        {
            TempData["ErrorMessage"] = "ไม่พบข้อมูลผู้ใช้ที่ต้องการลบ";
        }

        return RedirectToAction(nameof(Users));
    }
}
