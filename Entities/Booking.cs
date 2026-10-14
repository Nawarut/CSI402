using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç
/// </summary>
public partial class Booking
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç
    /// </summary>
    public uint BookingId { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ÑÓ©éÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©çÓ©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç Ó╣ÇÓ©èÓ╣êÓ©Ö BK20261115001
    /// </summary>
    public string BookingCode { get; set; } = null!;

    /// <summary>
    /// Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓Ó╣ÇÓ©êÓ╣ëÓ©▓Ó©éÓ©¡Ó©çÓ©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç
    /// </summary>
    public uint UserId { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ©¬Ó©úÓ╣ëÓ©▓Ó©çÓ©úÓ©▓Ó©óÓ©üÓ©▓Ó©ú (Ó©üÓ©úÓ©ôÓ©ÁÓ©×Ó©ÖÓ©▒Ó©üÓ©çÓ©▓Ó©ÖÓ©êÓ©¡Ó©çÓ╣üÓ©ùÓ©Ö)
    /// </summary>
    public uint? CreatedBy { get; set; }

    /// <summary>
    /// Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©ÖÓ©ùÓ©ÁÓ╣êÓ╣âÓ©èÓ╣ë (1 Ó╣éÓ©øÓ©úÓ©òÓ╣êÓ©¡ 1 Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç)
    /// </summary>
    public uint? PromotionId { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©ü
    /// </summary>
    public DateOnly CheckInDate { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©¡Ó©¡Ó©ü
    /// </summary>
    public DateOnly CheckOutDate { get; set; }

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©äÓ©ÀÓ©Ö
    /// </summary>
    public ushort Nights { get; set; }

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©£Ó©╣Ó╣ëÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©üÓ©úÓ©ºÓ©í
    /// </summary>
    public ushort TotalGuests { get; set; }

    /// <summary>
    /// Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ╣êÓ©▓Ó©½Ó╣ëÓ©¡Ó©çÓ©úÓ©ºÓ©íÓ©üÓ╣êÓ©¡Ó©ÖÓ©ÑÓ©ö
    /// </summary>
    public decimal SubtotalAmount { get; set; }

    /// <summary>
    /// Ó©äÓ╣êÓ©▓Ó╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©íÓ©úÓ©ºÓ©í
    /// </summary>
    public decimal ExtraBedAmount { get; set; }

    /// <summary>
    /// Ó©¬Ó╣êÓ©ºÓ©ÖÓ©ÑÓ©öÓ©úÓ©ºÓ©í
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Ó©óÓ©¡Ó©öÓ©¬Ó©©Ó©ùÓ©ÿÓ©┤Ó©½Ó©ÑÓ©▒Ó©çÓ©ÑÓ©ö
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©øÓ©¡Ó©úÓ╣îÓ╣ÇÓ©ïÓ╣çÓ©ÖÓ©òÓ╣îÓ©íÓ©▒Ó©öÓ©êÓ©│ Ó©ô Ó©ºÓ©▒Ó©ÖÓ©êÓ©¡Ó©ç
    /// </summary>
    public decimal DepositPercent { get; set; }

    /// <summary>
    /// Ó©óÓ©¡Ó©öÓ©íÓ©▒Ó©öÓ©êÓ©│
    /// </summary>
    public decimal DepositAmount { get; set; }

    /// <summary>
    /// Ó©üÓ©│Ó©½Ó©ÖÓ©öÓ©èÓ©│Ó©úÓ©░Ó©íÓ©▒Ó©öÓ©êÓ©│ (Ó╣ÇÓ©üÓ©┤Ó©ÖÓ╣üÓ©ÑÓ╣ëÓ©ºÓ©½Ó©íÓ©öÓ©¡Ó©▓Ó©óÓ©©)
    /// </summary>
    public DateTime? DepositDueAt { get; set; }

    /// <summary>
    /// Ó©×Ó©ÖÓ©▒Ó©üÓ©çÓ©▓Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©Ö
    /// </summary>
    public uint? ConfirmedBy { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©Ö
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó╣ÇÓ©èÓ╣çÓ©äÓ©¡Ó©┤Ó©ÖÓ©êÓ©úÓ©┤Ó©ç
    /// </summary>
    public DateTime? CheckedInAt { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó╣ÇÓ©èÓ╣çÓ©äÓ╣ÇÓ©¡Ó©▓Ó©ùÓ╣îÓ©êÓ©úÓ©┤Ó©ç
    /// </summary>
    public DateTime? CheckedOutAt { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©½Ó©òÓ©©Ó©£Ó©ÑÓ©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü
    /// </summary>
    public string? CancelReason { get; set; }

    /// <summary>
    /// Ó©óÓ©¡Ó©öÓ©íÓ©▒Ó©öÓ©êÓ©│Ó©ùÓ©ÁÓ╣êÓ©äÓ©ÀÓ©ÖÓ©êÓ©úÓ©┤Ó©ç
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// Ó©äÓ╣êÓ©▓Ó╣ÇÓ©¬Ó©ÁÓ©óÓ©½Ó©▓Ó©ó/Ó©äÓ╣êÓ©▓Ó╣âÓ©èÓ╣ëÓ©êÓ╣êÓ©▓Ó©óÓ©½Ó©▒Ó©üÓ©êÓ©▓Ó©üÓ©íÓ©▒Ó©öÓ©êÓ©│
    /// </summary>
    public decimal DamageCharge { get; set; }

    /// <summary>
    /// Ó©½Ó©íÓ©▓Ó©óÓ╣ÇÓ©½Ó©òÓ©©
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©êÓ©¡Ó©ç
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    public virtual User? ConfirmedByNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Promotion? Promotion { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
