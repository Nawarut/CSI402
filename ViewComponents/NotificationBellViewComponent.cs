using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.ViewModels;

namespace Nawarut_8250_L005.ViewComponents;

public class NotificationBellViewComponent : ViewComponent
{
    private readonly Csi402dbContext _db;

    public NotificationBellViewComponent(Csi402dbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new NotificationBellViewModel();
        var factory = HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
        var tempData = factory.GetTempData(HttpContext);
        foreach (var key in new[] { "SuccessMessage", "ErrorMessage", "WarningMessage", "InfoMessage", "Error" })
        {
            if (tempData.Peek(key) is string message && !string.IsNullOrWhiteSpace(message))
            {
                model.FlashMessages.Add(message);
                tempData.Remove(key);
            }
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId.HasValue)
        {
            model.UnreadCount = await _db.Notifications.CountAsync(n => n.UserId == (uint)userId.Value && !n.IsRead);
            model.Notifications = await _db.Notifications
                .Where(n => n.UserId == (uint)userId.Value)
                .OrderByDescending(n => n.CreatedAt)
                .Take(8)
                .ToListAsync();
        }

        return View(model);
    }
}
