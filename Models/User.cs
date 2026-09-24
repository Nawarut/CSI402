using System;

namespace Nawarut_8250_L005.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // "Admin", "Staff", "User"
    public string Status { get; set; } = "Active"; // "Active", "Inactive"
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
