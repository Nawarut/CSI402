using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©äÓ╣êÓ©▓Ó©òÓ©▒Ó╣ëÓ©çÓ©úÓ©░Ó©ÜÓ©Ü
/// </summary>
public partial class SystemSetting
{
    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡Ó©äÓ╣êÓ©▓Ó©òÓ©▒Ó╣ëÓ©ç
    /// </summary>
    public string SettingKey { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ╣êÓ©▓
    /// </summary>
    public string SettingValue { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©ó
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public uint? UpdatedBy { get; set; }

    /// <summary>
    /// Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
