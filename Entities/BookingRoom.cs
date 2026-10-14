using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©½Ó╣ëÓ©¡Ó©çÓ╣âÓ©ÖÓ©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç
/// </summary>
public partial class BookingRoom
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©úÓ©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public uint BookingRoomId { get; set; }

    /// <summary>
    /// Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç
    /// </summary>
    public uint BookingId { get; set; }

    /// <summary>
    /// Ó©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public uint RoomId { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©ü (Ó©¬Ó©│Ó╣ÇÓ©ÖÓ©▓Ó©êÓ©▓Ó©ü bookings)
    /// </summary>
    public DateOnly CheckInDate { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©¡Ó©¡Ó©ü (Ó©¬Ó©│Ó╣ÇÓ©ÖÓ©▓Ó©êÓ©▓Ó©ü bookings)
    /// </summary>
    public DateOnly CheckOutDate { get; set; }

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©£Ó©╣Ó╣ëÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©üÓ╣âÓ©ÖÓ©½Ó╣ëÓ©¡Ó©çÓ©ÖÓ©ÁÓ╣ë
    /// </summary>
    public byte Guests { get; set; }

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©í
    /// </summary>
    public byte ExtraBeds { get; set; }

    /// <summary>
    /// Ó©úÓ©▓Ó©äÓ©▓Ó©½Ó╣ëÓ©¡Ó©çÓ©òÓ╣êÓ©¡Ó©äÓ©ÀÓ©Ö Ó©ô Ó©ºÓ©▒Ó©ÖÓ©êÓ©¡Ó©ç (snapshot)
    /// </summary>
    public decimal PricePerNight { get; set; }

    /// <summary>
    /// Ó©úÓ©▓Ó©äÓ©▓Ó╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©íÓ©òÓ╣êÓ©¡Ó©äÓ©ÀÓ©Ö Ó©ô Ó©ºÓ©▒Ó©ÖÓ©êÓ©¡Ó©ç
    /// </summary>
    public decimal ExtraBedPrice { get; set; }

    /// <summary>
    /// Ó©úÓ©ºÓ©íÓ©½Ó╣ëÓ©¡Ó©çÓ©ÖÓ©ÁÓ╣ë (Ó©½Ó╣ëÓ©¡Ó©ç + Ó╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©í) Ó©ùÓ©©Ó©üÓ©äÓ©ÀÓ©Ö
    /// </summary>
    public decimal LineTotal { get; set; }

    /// <summary>
    /// Ó©óÓ©▒Ó©çÓ©íÓ©ÁÓ©£Ó©ÑÓ©üÓ©▒Ó©ÜÓ©òÓ©▓Ó©úÓ©▓Ó©çÓ©½Ó╣ëÓ©¡Ó©ç (0 = Ó©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü/Ó©êÓ©ÜÓ╣üÓ©ÑÓ╣ëÓ©º)
    /// </summary>
    public bool? IsActive { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
