using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ëÓ©úÓ©░Ó©ÜÓ©Ü
/// </summary>
public partial class User
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ë
    /// </summary>
    public uint UserId { get; set; }

    /// <summary>
    /// Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù (FK roles)
    /// </summary>
    public byte RoleId { get; set; }

    /// <summary>
    /// Ó╣üÓ©£Ó©ÖÓ©ü (Ó╣ÇÓ©ëÓ©×Ó©▓Ó©░Ó©×Ó©ÖÓ©▒Ó©üÓ©çÓ©▓Ó©Ö)
    /// </summary>
    public ushort? DepartmentId { get; set; }

    /// <summary>
    /// Ó©¡Ó©ÁÓ╣ÇÓ©íÓ©Ñ Ó╣âÓ©èÓ╣ëÓ©ÑÓ╣çÓ©¡Ó©üÓ©¡Ó©┤Ó©Ö
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©£Ó╣êÓ©▓Ó©ÖÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©úÓ©½Ó©▒Ó©¬Ó╣üÓ©ÑÓ╣ëÓ©º (bcrypt/argon2)
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Ó©ÖÓ©▓Ó©íÓ©¬Ó©üÓ©©Ó©Ñ
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Ó╣ÇÓ©ÜÓ©¡Ó©úÓ╣îÓ╣éÓ©ùÓ©ú
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░Ó©ÜÓ©▒Ó©ìÓ©èÓ©Á
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Ó©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©ÖÓ©¡Ó©ÁÓ╣ÇÓ©íÓ©ÑÓ╣üÓ©ÑÓ╣ëÓ©º (0/1)
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©äÓ©úÓ©▒Ó╣ëÓ©çÓ©ÑÓ╣çÓ©¡Ó©üÓ©¡Ó©┤Ó©ÖÓ©£Ó©┤Ó©öÓ©òÓ©┤Ó©öÓ©òÓ╣êÓ©¡Ó©üÓ©▒Ó©Ö
    /// </summary>
    public byte FailedLogins { get; set; }

    /// <summary>
    /// Ó╣ÇÓ©éÓ╣ëÓ©▓Ó©¬Ó©╣Ó╣êÓ©úÓ©░Ó©ÜÓ©ÜÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©¬Ó©íÓ©▒Ó©äÓ©ú
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Booking> BookingConfirmedByNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingCreatedByNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingUsers { get; set; } = new List<Booking>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SystemSetting> SystemSettings { get; set; } = new List<SystemSetting>();

    public virtual ICollection<Ticket> TicketAssignedToNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketCustomers { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketMessage> TicketMessages { get; set; } = new List<TicketMessage>();
}
