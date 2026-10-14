using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ticket Ó©øÓ©▒Ó©ìÓ©½Ó©▓Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬ Ticket
    /// </summary>
    public uint TicketId { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©ÑÓ©é Ticket Ó╣ÇÓ©èÓ╣êÓ©Ö TK-001
    /// </summary>
    public string TicketCode { get; set; } = null!;

    /// <summary>
    /// Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓Ó©£Ó©╣Ó╣ëÓ╣üÓ©êÓ╣ëÓ©ç
    /// </summary>
    public uint CustomerId { get; set; }

    /// <summary>
    /// Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©çÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©üÓ©ÁÓ╣êÓ©óÓ©ºÓ©éÓ╣ëÓ©¡Ó©ç
    /// </summary>
    public uint? BookingId { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©êÓ╣ëÓ©▓Ó©½Ó©ÖÓ╣ëÓ©▓Ó©ùÓ©ÁÓ╣ê CS Ó©£Ó©╣Ó╣ëÓ©úÓ©▒Ó©ÜÓ╣ÇÓ©úÓ©ÀÓ╣êÓ©¡Ó©ç
    /// </summary>
    public uint? AssignedTo { get; set; }

    /// <summary>
    /// Ó╣üÓ©£Ó©ÖÓ©üÓ©øÓ©ÑÓ©▓Ó©óÓ©ùÓ©▓Ó©çÓ©ùÓ©ÁÓ╣êÓ©¬Ó╣êÓ©çÓ©òÓ╣êÓ©¡
    /// </summary>
    public ushort? DepartmentId { get; set; }

    /// <summary>
    /// Ó©½Ó©▒Ó©ºÓ©éÓ╣ëÓ©¡
    /// </summary>
    public string Subject { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ©ºÓ©▓Ó©íÓ╣ÇÓ©úÓ╣êÓ©çÓ©öÓ╣êÓ©ºÓ©Ö
    /// </summary>
    public string Priority { get; set; } = null!;

    /// <summary>
    /// Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ╣üÓ©êÓ╣ëÓ©ç
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ╣üÓ©üÓ╣ëÓ©øÓ©▒Ó©ìÓ©½Ó©▓Ó╣ÇÓ©¬Ó©úÓ╣çÓ©ê
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Department? Department { get; set; }

    public virtual ICollection<TicketMessage> TicketMessages { get; set; } = new List<TicketMessage>();
}
