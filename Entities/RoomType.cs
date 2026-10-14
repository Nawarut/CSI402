using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©çÓ©×Ó©▒Ó©ü
/// </summary>
public partial class RoomType
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public ushort RoomTypeId { get; set; }

    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ù Ó╣ÇÓ©èÓ╣êÓ©Ö Deluxe King Suite
    /// </summary>
    public string TypeName { get; set; } = null!;

    /// <summary>
    /// Ó©úÓ©▓Ó©óÓ©ÑÓ©░Ó╣ÇÓ©¡Ó©ÁÓ©óÓ©öÓ©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ╣ÇÓ©òÓ©ÁÓ©óÓ©ç Ó╣ÇÓ©èÓ╣êÓ©Ö King, Twin
    /// </summary>
    public string BedType { get; set; } = null!;

    /// <summary>
    /// Ó©éÓ©ÖÓ©▓Ó©öÓ©½Ó╣ëÓ©¡Ó©ç (Ó©òÓ©ú.Ó©í.)
    /// </summary>
    public decimal? SizeSqm { get; set; }

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©£Ó©╣Ó╣ëÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©üÓ©¬Ó©╣Ó©çÓ©¬Ó©©Ó©öÓ©òÓ╣êÓ©¡Ó©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public byte MaxGuests { get; set; }

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©íÓ©¬Ó©╣Ó©çÓ©¬Ó©©Ó©ö
    /// </summary>
    public byte MaxExtraBeds { get; set; }

    /// <summary>
    /// Ó©úÓ©▓Ó©äÓ©▓Ó©×Ó©ÀÓ╣ëÓ©ÖÓ©ÉÓ©▓Ó©ÖÓ©òÓ╣êÓ©¡Ó©äÓ©ÀÓ©Ö (Ó©ÜÓ©▓Ó©ù)
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©øÓ©┤Ó©öÓ©éÓ©▓Ó©ó (0/1)
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

    public virtual ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
}
