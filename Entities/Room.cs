using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©½Ó╣ëÓ©¡Ó©çÓ©×Ó©▒Ó©ü
/// </summary>
public partial class Room
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public uint RoomId { get; set; }

    /// <summary>
    /// Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç (FK)
    /// </summary>
    public ushort RoomTypeId { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ÑÓ©éÓ©½Ó╣ëÓ©¡Ó©ç Ó╣ÇÓ©èÓ╣êÓ©Ö 1205
    /// </summary>
    public string RoomNumber { get; set; } = null!;

    /// <summary>
    /// Ó©èÓ©▒Ó╣ëÓ©Ö
    /// </summary>
    public byte? FloorNo { get; set; }

    /// <summary>
    /// Ó©úÓ©▓Ó©äÓ©▓Ó╣ÇÓ©ëÓ©×Ó©▓Ó©░Ó©½Ó╣ëÓ©¡Ó©ç (Ó©ûÓ╣ëÓ©▓ NULL Ó╣âÓ©èÓ╣ë base_price Ó©éÓ©¡Ó©çÓ©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ù)
    /// </summary>
    public decimal? PricePerNight { get; set; }

    /// <summary>
    /// Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░Ó©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Ó╣ÇÓ©øÓ©┤Ó©öÓ╣âÓ©èÓ╣ëÓ©çÓ©▓Ó©Ö (0/1)
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    public virtual RoomType RoomType { get; set; } = null!;
}
