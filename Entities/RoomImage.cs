using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©úÓ©╣Ó©øÓ©áÓ©▓Ó©×Ó©½Ó╣ëÓ©¡Ó©ç
/// </summary>
public partial class RoomImage
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©╣Ó©ø
    /// </summary>
    public uint ImageId { get; set; }

    /// <summary>
    /// Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç
    /// </summary>
    public ushort RoomTypeId { get; set; }

    /// <summary>
    /// Ó©ùÓ©ÁÓ╣êÓ©¡Ó©óÓ©╣Ó╣êÓ╣äÓ©ƒÓ©ÑÓ╣îÓ©úÓ©╣Ó©ø
    /// </summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©óÓ©úÓ©╣Ó©ø
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Ó©ÑÓ©│Ó©öÓ©▒Ó©ÜÓ╣üÓ©¬Ó©öÓ©ç
    /// </summary>
    public byte SortOrder { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©øÓ╣çÓ©ÖÓ©úÓ©╣Ó©øÓ©øÓ©ü (0/1)
    /// </summary>
    public bool IsCover { get; set; }

    public virtual RoomType RoomType { get; set; } = null!;
}
