using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ùÓ©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ë
/// </summary>
public partial class Role
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù
    /// </summary>
    public byte RoleId { get; set; }

    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù Ó╣ÇÓ©èÓ╣êÓ©Ö GUEST, STAFF, ACCOUNTANT, SUPPORT, ADMIN
    /// </summary>
    public string RoleCode { get; set; } = null!;

    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù
    /// </summary>
    public string RoleName { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©óÓ©½Ó©ÖÓ╣ëÓ©▓Ó©ùÓ©ÁÓ╣ê
    /// </summary>
    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
