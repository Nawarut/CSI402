using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;

namespace Nawarut_8250_L005.Controllers;

public class NotificationsController : Controller
{
    private readonly Csi402dbContext _db;
    public NotificationsController(Csi402dbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Open(uint id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");
        var note = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == (uint)userId.Value);
        if (note == null) return RedirectToAction("Index", "Home");
        note.IsRead = true;
        await _db.SaveChangesAsync();

        return (note.LinkType, note.LinkId) switch
        {
            ("payment", uint paymentId) => RedirectToAction("PaymentSlip", "Payments", new { paymentId }),
            ("ticket", _) => RedirectToAction("Support", "Account"),
            ("department_ticket", _) => RedirectDepartmentTicket(),
            ("booking", _) => RedirectToAction("BookingHistory", "Account"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    private IActionResult RedirectDepartmentTicket()
    {
        var role = HttpContext.Session.GetString("UserRole");
        return role?.ToLowerInvariant() switch
        {
            "accountant" or "accounting" => RedirectToAction("Index", "Accounting"),
            "staff" or "reception" => RedirectToAction("Index", "Reception"),
            "admin" => RedirectToAction("Index", "CustomerService"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReadAll()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId != null)
        {
            var unread = await _db.Notifications.Where(n => n.UserId == (uint)userId.Value && !n.IsRead).ToListAsync();
            unread.ForEach(n => n.IsRead = true);
            await _db.SaveChangesAsync();
        }
        return Redirect(Request.Headers.Referer.FirstOrDefault() ?? Url.Action("Index", "Home")!);
    }
}
