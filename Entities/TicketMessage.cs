using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©éÓ╣ëÓ©¡Ó©äÓ©ºÓ©▓Ó©íÓ╣âÓ©Ö Ticket
/// </summary>
public partial class TicketMessage
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©éÓ╣ëÓ©¡Ó©äÓ©ºÓ©▓Ó©í
    /// </summary>
    public uint MessageId { get; set; }

    /// <summary>
    /// Ticket
    /// </summary>
    public uint TicketId { get; set; }

    /// <summary>
    /// Ó©£Ó©╣Ó╣ëÓ©¬Ó╣êÓ©ç
    /// </summary>
    public uint SenderId { get; set; }

    /// <summary>
    /// Ó©éÓ╣ëÓ©¡Ó©äÓ©ºÓ©▓Ó©í
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Ó©ÜÓ©▒Ó©ÖÓ©ùÓ©ÂÓ©üÓ©áÓ©▓Ó©óÓ╣âÓ©Ö Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓Ó╣äÓ©íÓ╣êÓ╣ÇÓ©½Ó╣çÓ©Ö (0/1)
    /// </summary>
    public bool IsInternal { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó╣êÓ©ç
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public virtual User Sender { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
