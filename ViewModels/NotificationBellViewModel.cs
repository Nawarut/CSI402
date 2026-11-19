using Nawarut_8250_L005.Entities;

namespace Nawarut_8250_L005.ViewModels;

public class NotificationBellViewModel
{
    public List<Notification> Notifications { get; set; } = new();
    public List<string> FlashMessages { get; set; } = new();
    public int UnreadCount { get; set; }
}
