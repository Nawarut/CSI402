using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©¬Ó©┤Ó╣êÓ©çÓ©¡Ó©│Ó©ÖÓ©ºÓ©óÓ©äÓ©ºÓ©▓Ó©íÓ©¬Ó©░Ó©öÓ©ºÓ©ü
/// </summary>
public partial class Amenity
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©¬Ó©┤Ó╣êÓ©çÓ©¡Ó©│Ó©ÖÓ©ºÓ©óÓ©äÓ©ºÓ©▓Ó©íÓ©¬Ó©░Ó©öÓ©ºÓ©ü
    /// </summary>
    public ushort AmenityId { get; set; }

    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡ Ó╣ÇÓ©èÓ╣êÓ©Ö Wi-Fi, TV, Ó©òÓ©╣Ó╣ëÓ╣ÇÓ©óÓ╣çÓ©Ö
    /// </summary>
    public string AmenityName { get; set; } = null!;

    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡Ó╣äÓ©¡Ó©äÓ©¡Ó©Ö
    /// </summary>
    public string? Icon { get; set; }

    public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
}
