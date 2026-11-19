using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;

namespace Nawarut_8250_L005.Services;

public static class DepartmentTicketService
{
    public static bool IsAccounting(string? name) =>
        !string.IsNullOrWhiteSpace(name) &&
        (name.Contains("account", StringComparison.OrdinalIgnoreCase) || name.Contains("บัญชี", StringComparison.OrdinalIgnoreCase));

    public static bool IsReception(string? name) =>
        !string.IsNullOrWhiteSpace(name) &&
        (name.Contains("reception", StringComparison.OrdinalIgnoreCase) ||
         name.Contains("front desk", StringComparison.OrdinalIgnoreCase) ||
         name.Contains("ต้อนรับ", StringComparison.OrdinalIgnoreCase));

    public static async Task<Department?> FindAsync(Csi402dbContext db, string target)
    {
        var departments = await db.Departments.ToListAsync();
        return departments.FirstOrDefault(d => target == "Accounting"
            ? IsAccounting(d.DepartmentName)
            : IsReception(d.DepartmentName));
    }

    public static async Task<Department> GetOrCreateAsync(Csi402dbContext db, string target)
    {
        var department = await FindAsync(db, target);
        if (department != null) return department;

        department = new Department
        {
            DepartmentName = target == "Accounting" ? "Accounting" : "Reception",
            Description = target == "Accounting" ? "ฝ่ายบัญชีและการเงิน" : "ฝ่ายต้อนรับและห้องพัก"
        };
        db.Departments.Add(department);
        await db.SaveChangesAsync();
        return department;
    }

    public static async Task<List<Ticket>> GetOpenTicketsAsync(Csi402dbContext db, string target)
    {
        var department = await FindAsync(db, target);
        if (department == null) return new List<Ticket>();

        // Backfill older escalations created before DepartmentId was assigned.
        var unassigned = await db.Tickets
            .Where(t => t.DepartmentId == null && t.Status == "escalated")
            .Include(t => t.TicketMessages)
            .ToListAsync();
        bool changed = false;
        foreach (var ticket in unassigned)
        {
            var routeNote = ticket.TicketMessages
                .Where(m => m.IsInternal)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => m.Message)
                .FirstOrDefault();
            bool belongsToTarget = target == "Accounting"
                ? routeNote?.Contains("บัญชี", StringComparison.OrdinalIgnoreCase) == true
                : routeNote?.Contains("ต้อนรับ", StringComparison.OrdinalIgnoreCase) == true;
            if (belongsToTarget)
            {
                ticket.DepartmentId = department.DepartmentId;
                changed = true;
            }
        }
        if (changed) await db.SaveChangesAsync();

        return await db.Tickets
            .Where(t => t.DepartmentId == department.DepartmentId && t.Status != "resolved")
            .Include(t => t.Customer)
            .Include(t => t.Booking)
            .Include(t => t.TicketMessages)
            .OrderByDescending(t => t.Priority == "urgent")
            .ThenByDescending(t => t.UpdatedAt)
            .ToListAsync();
    }
}
