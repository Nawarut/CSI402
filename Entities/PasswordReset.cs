using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó╣éÓ©ùÓ╣ÇÓ©äÓ©ÖÓ©úÓ©ÁÓ╣ÇÓ©ïÓ╣çÓ©òÓ©úÓ©½Ó©▒Ó©¬Ó©£Ó╣êÓ©▓Ó©Ö/Ó©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©ÖÓ©¡Ó©ÁÓ╣ÇÓ©íÓ©Ñ
/// </summary>
public partial class PasswordReset
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©ú
    /// </summary>
    public uint ResetId { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ë
    /// </summary>
    public uint UserId { get; set; }

    /// <summary>
    /// Ó╣éÓ©ùÓ╣ÇÓ©äÓ©Ö (SHA-256 hash)
    /// </summary>
    public string TokenHash { get; set; } = null!;

    /// <summary>
    /// Ó©ºÓ©▒Ó©òÓ©ûÓ©©Ó©øÓ©úÓ©░Ó©¬Ó©çÓ©äÓ╣î
    /// </summary>
    public string Purpose { get; set; } = null!;

    /// <summary>
    /// Ó©½Ó©íÓ©öÓ©¡Ó©▓Ó©óÓ©©
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ╣âÓ©èÓ╣ëÓ╣üÓ©ÑÓ╣ëÓ©º
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
