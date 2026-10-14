using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©üÓ©▓Ó©úÓ╣üÓ©êÓ╣ëÓ©çÓ╣ÇÓ©òÓ©ÀÓ©¡Ó©Ö
/// </summary>
public partial class Notification
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©üÓ©▓Ó©úÓ╣üÓ©êÓ╣ëÓ©çÓ╣ÇÓ©òÓ©ÀÓ©¡Ó©Ö
    /// </summary>
    public uint NotificationId { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ©úÓ©▒Ó©Ü
    /// </summary>
    public uint UserId { get; set; }

    /// <summary>
    /// Ó©½Ó©▒Ó©ºÓ©éÓ╣ëÓ©¡
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Ó╣ÇÓ©ÖÓ©ÀÓ╣ëÓ©¡Ó©½Ó©▓
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Ó©èÓ©ÖÓ©┤Ó©öÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©ç Ó╣ÇÓ©èÓ╣êÓ©Ö booking, ticket
    /// </summary>
    public string? LinkType { get; set; }

    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©úÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©ç
    /// </summary>
    public uint? LinkId { get; set; }

    /// <summary>
    /// Ó©¡Ó╣êÓ©▓Ó©ÖÓ╣üÓ©ÑÓ╣ëÓ©º (0/1)
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
