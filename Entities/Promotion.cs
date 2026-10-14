using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©Ö
/// </summary>
public partial class Promotion
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©Ö
    /// </summary>
    public uint PromotionId { get; set; }

    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó╣éÓ©øÓ©ú Ó╣ÇÓ©èÓ╣êÓ©Ö NEWMEMBER10
    /// </summary>
    public string PromoCode { get; set; } = null!;

    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©Ö
    /// </summary>
    public string PromoName { get; set; } = null!;

    /// <summary>
    /// Ó©úÓ©▓Ó©óÓ©ÑÓ©░Ó╣ÇÓ©¡Ó©ÁÓ©óÓ©ö
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©¬Ó╣êÓ©ºÓ©ÖÓ©ÑÓ©ö: Ó╣ÇÓ©øÓ©¡Ó©úÓ╣îÓ╣ÇÓ©ïÓ╣çÓ©ÖÓ©òÓ╣î/Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö
    /// </summary>
    public string DiscountType { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ╣êÓ©▓Ó©¬Ó╣êÓ©ºÓ©ÖÓ©ÑÓ©ö (% Ó©½Ó©úÓ©ÀÓ©¡ Ó©ÜÓ©▓Ó©ù)
    /// </summary>
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ╣ÇÓ©çÓ©ÀÓ╣êÓ©¡Ó©ÖÓ╣äÓ©é
    /// </summary>
    public string ConditionType { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ╣êÓ©▓Ó╣ÇÓ©çÓ©ÀÓ╣êÓ©¡Ó©ÖÓ╣äÓ©é (Ó©äÓ©ÀÓ©Ö/Ó©ÜÓ©▓Ó©ù/Ó©äÓ©Ö/Ó©ºÓ©▒Ó©Ö)
    /// </summary>
    public decimal ConditionValue { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ╣ÇÓ©úÓ©┤Ó╣êÓ©íÓ╣éÓ©øÓ©ú
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©¬Ó©┤Ó╣ëÓ©ÖÓ©¬Ó©©Ó©öÓ╣éÓ©øÓ©ú
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©øÓ©┤Ó©öÓ╣âÓ©èÓ╣ëÓ©çÓ©▓Ó©Ö (0/1)
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç (Ó╣üÓ©¡Ó©öÓ©íÓ©┤Ó©Ö)
    /// </summary>
    public uint? CreatedBy { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User? CreatedByNavigation { get; set; }
}
