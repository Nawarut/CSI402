using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.Services;

namespace Nawarut_8250_L005.Controllers;

public class DepartmentTicketsController : Controller
{
    private readonly Csi402dbContext _db;
    public DepartmentTicketsController(Csi402dbContext db) => _db = db;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Take(uint ticketId)
    {
        var ticket = await _db.Tickets.Include(t => t.Department)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        var denied = ValidateDepartmentAccess(ticket);
        if (denied != null) return denied;
        if (ticket == null) return NotFound();
        if (ticket.Status != "escalated") return RedirectToDepartmentQueue(ticket.Department!.DepartmentName);

        var userId = (uint)HttpContext.Session.GetInt32("UserId")!.Value;
        ticket.Status = "in_progress";
        ticket.AssignedTo = userId;
        ticket.UpdatedAt = DateTime.Now;
        _db.TicketMessages.Add(new TicketMessage
        {
            TicketId = ticket.TicketId,
            SenderId = userId,
            Message = "ฝ่ายที่เกี่ยวข้องรับเรื่องแล้ว และกำลังดำเนินการ",
            IsInternal = true,
            CreatedAt = DateTime.Now
        });
        NotificationHelper.Add(_db, ticket.CustomerId, "ฝ่ายที่เกี่ยวข้องรับเรื่องแล้ว",
            $"Ticket {ticket.TicketCode} กำลังอยู่ระหว่างดำเนินการ", "ticket", ticket.TicketId);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"รับ Ticket {ticket.TicketCode} เข้าดำเนินการแล้ว";
        return RedirectToDepartmentQueue(ticket.Department!.DepartmentName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(uint ticketId, string response)
    {
        var ticket = await _db.Tickets.Include(t => t.Department)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        var denied = ValidateDepartmentAccess(ticket);
        if (denied != null) return denied;
        if (ticket == null) return NotFound();

        response = (response ?? string.Empty).Trim();
        if (response.Length is < 5 or > 2000)
        {
            TempData["ErrorMessage"] = "กรุณาระบุผลการดำเนินการ 5–2,000 ตัวอักษร";
            return RedirectToDepartmentQueue(ticket.Department!.DepartmentName);
        }
        if (ticket.Status == "resolved") return RedirectToDepartmentQueue(ticket.Department!.DepartmentName);

        var userId = (uint)HttpContext.Session.GetInt32("UserId")!.Value;
        ticket.Status = "resolved";
        ticket.ResolvedAt = DateTime.Now;
        ticket.UpdatedAt = DateTime.Now;
        _db.TicketMessages.Add(new TicketMessage
        {
            TicketId = ticket.TicketId,
            SenderId = userId,
            Message = response,
            IsInternal = false,
            CreatedAt = DateTime.Now
        });
        NotificationHelper.Add(_db, ticket.CustomerId, "ฝ่ายที่เกี่ยวข้องดำเนินการเสร็จแล้ว",
            $"Ticket {ticket.TicketCode}: {response[..Math.Min(350, response.Length)]}", "ticket", ticket.TicketId);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"ปิด Ticket {ticket.TicketCode} และแจ้งผลให้ลูกค้าแล้ว";
        return RedirectToDepartmentQueue(ticket.Department!.DepartmentName);
    }

    private IActionResult? ValidateDepartmentAccess(Ticket? ticket)
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrWhiteSpace(role))
            return RedirectToAction("Login", "Account");
        if (ticket == null) return null;

        var departmentName = ticket.Department?.DepartmentName;
        bool allowed = role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            (DepartmentTicketService.IsAccounting(departmentName) &&
                (role.Equals("Accountant", StringComparison.OrdinalIgnoreCase) || role.Equals("Accounting", StringComparison.OrdinalIgnoreCase))) ||
            (DepartmentTicketService.IsReception(departmentName) &&
                (role.Equals("Staff", StringComparison.OrdinalIgnoreCase) || role.Equals("Reception", StringComparison.OrdinalIgnoreCase)));
        if (allowed) return null;

        TempData["ErrorMessage"] = "คุณไม่มีสิทธิ์จัดการ Ticket ที่ส่งไปยังแผนกนี้";
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToDepartmentQueue(string departmentName) =>
        DepartmentTicketService.IsAccounting(departmentName)
            ? RedirectToAction("Index", "Accounting")
            : RedirectToAction("Index", "Reception");
}
