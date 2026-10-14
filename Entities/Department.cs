using System;
using System.Collections.Generic;

namespace Nawarut_8250_L005.Entities;

/// <summary>
/// Ó╣üÓ©£Ó©ÖÓ©ü
/// </summary>
public partial class Department
{
    /// <summary>
    /// Ó©úÓ©½Ó©▒Ó©¬Ó╣üÓ©£Ó©ÖÓ©ü
    /// </summary>
    public ushort DepartmentId { get; set; }

    /// <summary>
    /// Ó©èÓ©ÀÓ╣êÓ©¡Ó╣üÓ©£Ó©ÖÓ©ü
    /// </summary>
    public string DepartmentName { get; set; } = null!;

    /// <summary>
    /// Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©ó
    /// </summary>
    public string? Description { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
