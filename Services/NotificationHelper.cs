using Nawarut_8250_L005.Entities;

namespace Nawarut_8250_L005.Services;

public static class NotificationHelper
{
    public static void Add(Csi402dbContext db, uint userId, string title, string body, string linkType, uint linkId)
    {
        db.Notifications.Add(new Notification
        {
            UserId = userId,
            Title = title,
            Body = body,
            LinkType = linkType,
            LinkId = linkId,
            IsRead = false,
            CreatedAt = DateTime.Now
        });
    }
}
