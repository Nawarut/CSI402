using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©ÖÓ╣éÓ©óÓ©ÜÓ©▓Ó©óÓ©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü
/// </summary>
public partial class CancellationPolicy
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©ÖÓ╣éÓ©óÓ©ÜÓ©▓Ó©ó
    /// </summary>
    public byte PolicyId { get; set; }

    /// <summary>
    /// Ó©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©üÓ©üÓ╣êÓ©¡Ó©ÖÓ╣ÇÓ©èÓ╣çÓ©äÓ©¡Ó©┤Ó©ÖÓ©¡Ó©óÓ╣êÓ©▓Ó©çÓ©ÖÓ╣ëÓ©¡Ó©óÓ©üÓ©ÁÓ╣êÓ©ºÓ©▒Ó©Ö
    /// </summary>
    public ushort MinDaysBefore { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©øÓ©¡Ó©úÓ╣îÓ╣ÇÓ©ïÓ╣çÓ©ÖÓ©òÓ╣îÓ©íÓ©▒Ó©öÓ©êÓ©│Ó©ùÓ©ÁÓ╣êÓ©äÓ©ÀÓ©Ö
    /// </summary>
    public decimal RefundPercent { get; set; }

    /// <summary>
    /// Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©ó
    /// </summary>
    public string? Description { get; set; }
}
