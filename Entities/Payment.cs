using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©üÓ©▓Ó©úÓ©èÓ©│Ó©úÓ©░Ó╣ÇÓ©çÓ©┤Ó©Ö
/// </summary>
public partial class Payment
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©úÓ©èÓ©│Ó©úÓ©░
    /// </summary>
    public uint PaymentId { get; set; }

    /// <summary>
    /// Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç
    /// </summary>
    public uint BookingId { get; set; }

    /// <summary>
    /// Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ù: Ó©íÓ©▒Ó©öÓ©êÓ©│/Ó©¬Ó╣êÓ©ºÓ©ÖÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©½Ó©ÑÓ©ÀÓ©¡/Ó©äÓ©ÀÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö/Ó©äÓ╣êÓ©▓Ó╣ÇÓ©¬Ó©ÁÓ©óÓ©½Ó©▓Ó©ó
    /// </summary>
    public string PaymentType { get; set; } = null!;

    /// <summary>
    /// Ó©èÓ╣êÓ©¡Ó©çÓ©ùÓ©▓Ó©çÓ©èÓ©│Ó©úÓ©░
    /// </summary>
    public string Method { get; set; } = null!;

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Ó╣ÇÓ©ÑÓ©éÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©çÓ©êÓ©▓Ó©üÓ©£Ó©╣Ó╣ëÓ╣âÓ©½Ó╣ëÓ©ÜÓ©úÓ©┤Ó©üÓ©▓Ó©úÓ©èÓ©│Ó©úÓ©░Ó╣ÇÓ©çÓ©┤Ó©Ö
    /// </summary>
    public string? TransactionRef { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ©öÓ©│Ó╣ÇÓ©ÖÓ©┤Ó©ÖÓ©üÓ©▓Ó©ú (Ó╣ÇÓ©èÓ╣êÓ©Ö Ó©ÜÓ©▒Ó©ìÓ©èÓ©ÁÓ©ùÓ©ÁÓ╣êÓ©äÓ©ÀÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö)
    /// </summary>
    public uint? ProcessedBy { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©èÓ©│Ó©úÓ©░Ó©¬Ó©│Ó╣ÇÓ©úÓ╣çÓ©ê
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©çÓ©úÓ©▓Ó©óÓ©üÓ©▓Ó©ú
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User? ProcessedByNavigation { get; set; }
}
