using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Audit Log
/// </summary>
public partial class AuditLog
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬ log
    /// </summary>
    public ulong LogId { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ©üÓ©úÓ©░Ó©ùÓ©│
    /// </summary>
    public uint? UserId { get; set; }

    /// <summary>
    /// Ó©üÓ©▓Ó©úÓ©üÓ©úÓ©░Ó©ùÓ©│ Ó╣ÇÓ©èÓ╣êÓ©Ö LOGIN, CREATE_BOOKING, REFUND
    /// </summary>
    public string Action { get; set; } = null!;

    /// <summary>
    /// Ó©òÓ©▓Ó©úÓ©▓Ó©çÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©üÓ©ÁÓ╣êÓ©óÓ©ºÓ©éÓ╣ëÓ©¡Ó©ç
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó╣üÓ©ûÓ©ºÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©üÓ©ÁÓ╣êÓ©óÓ©ºÓ©éÓ╣ëÓ©¡Ó©ç
    /// </summary>
    public uint? EntityId { get; set; }

    /// <summary>
    /// Ó©úÓ©▓Ó©óÓ©ÑÓ©░Ó╣ÇÓ©¡Ó©ÁÓ©óÓ©öÓ©üÓ╣êÓ©¡Ó©Ö/Ó©½Ó©ÑÓ©▒Ó©çÓ╣üÓ©üÓ╣ëÓ╣äÓ©é
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// IP Ó©£Ó©╣Ó╣ëÓ©üÓ©úÓ©░Ó©ùÓ©│
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
