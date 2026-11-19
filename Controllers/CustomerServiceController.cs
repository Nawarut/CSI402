using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.Services;

namespace Nawarut_8250_L005.Controllers;

public class CustomerServiceController : Controller
{
    private readonly Csi402dbContext _db;

    public CustomerServiceController(Csi402dbContext db)
    {
        _db = db;
        EnsureDefaultTickets();
    }

    private IActionResult? ValidateCustomerServiceAccess()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrWhiteSpace(role))
            return RedirectToAction("Login", "Account", new { returnUrl = "/CustomerService" });

        if (role.Equals("CustomerService", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Support", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            return null;

        TempData["ErrorMessage"] = "คุณไม่มีสิทธิ์ดำเนินการในหน้าฝ่ายบริการลูกค้า";
        return RedirectToAction("Index", "Home");
    }

    // ==========================================
    // GET: /CustomerService/Index
    // แดชบอร์ดฝ่ายบริการลูกค้าและรับแจ้งปัญหา/ส่งต่อเรื่อง
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Index(string? search = null, string? statusFilter = null)
    {
        var accessCheck = ValidateCustomerServiceAccess();
        if (accessCheck != null) return accessCheck;

        var ticketsQuery = _db.Tickets
            .Include(t => t.Customer)
            .Include(t => t.Booking)
            .Include(t => t.Department)
            .Include(t => t.TicketMessages)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            ticketsQuery = ticketsQuery.Where(t => t.TicketCode.ToLower().Contains(s) ||
                                                  t.Subject.ToLower().Contains(s) ||
                                                  (t.Customer != null && (t.Customer.FirstName.ToLower().Contains(s) ||
                                                                          t.Customer.LastName.ToLower().Contains(s) ||
                                                                          t.Customer.Email.ToLower().Contains(s))));
        }

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            ticketsQuery = ticketsQuery.Where(t => t.Status == statusFilter);
        }

        var tickets = await ticketsQuery.OrderByDescending(t => t.CreatedAt).ToListAsync();

        int totalTickets = await _db.Tickets.CountAsync();
        int openTickets = await _db.Tickets.CountAsync(t => t.Status == "open" || t.Status == "Open" || t.Status == "เปิดเรื่อง");
        int escalatedTickets = await _db.Tickets.CountAsync(t => t.Status == "escalated" || t.Status.StartsWith("Escalated") || t.Status == "ส่งต่อแล้ว");
        int resolvedTickets = await _db.Tickets.CountAsync(t => t.Status == "resolved" || t.Status == "Resolved" || t.Status == "แก้ไขแล้ว");

        ViewBag.TotalTickets = totalTickets;
        ViewBag.OpenTickets = openTickets;
        ViewBag.EscalatedTickets = escalatedTickets;
        ViewBag.ResolvedTickets = resolvedTickets;
        ViewBag.Search = search;
        ViewBag.StatusFilter = statusFilter;

        // ดึงรายการลูกค้าและการจองเผื่อเปิด Ticket ใหม่
        ViewBag.Customers = await _db.Users.Where(u => u.Status == "active").Take(20).ToListAsync();
        ViewBag.Bookings = await _db.Bookings.OrderByDescending(b => b.CreatedAt).Take(20).ToListAsync();

        return View(tickets);
    }

    // ==========================================
    // POST: /CustomerService/CreateTicket
    // บันทึกรับแจ้งปัญหาการใช้งานใหม่
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTicket(uint customerId, uint? bookingId, string subject, string priority, string message)
    {
        var accessCheck = ValidateCustomerServiceAccess();
        if (accessCheck != null) return accessCheck;

        string ticketCode = $"TK{DateTime.Now:yyyyMMdd}{DateTime.Now.Ticks % 1000:D3}";

        string cleanPriority = (priority ?? "normal").Trim().ToLowerInvariant() switch
        {
            "urgent" or "ด่วนมาก" => "urgent",
            "low" or "ต่ำ" => "low",
            _ => "normal"
        };

        var ticket = new Ticket
        {
            TicketCode = ticketCode,
            CustomerId = customerId,
            BookingId = bookingId,
            Subject = subject,
            Priority = cleanPriority,
            Status = "open",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(message))
        {
            _db.TicketMessages.Add(new TicketMessage
            {
                TicketId = ticket.TicketId,
                SenderId = customerId,
                Message = message,
                IsInternal = false,
                CreatedAt = DateTime.Now
            });
            await _db.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = $"บันทึกเรื่องแจ้งปัญหา #{ticketCode} เรียบร้อยแล้ว";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // POST: /CustomerService/EscalateTicket
    // คัดกรองและส่งต่อเรื่อง (Escalation) ไปยังฝ่ายที่รับผิดชอบ
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EscalateTicket(uint ticketId, string targetDepartment, string? note)
    {
        var accessCheck = ValidateCustomerServiceAccess();
        if (accessCheck != null) return accessCheck;

        if (targetDepartment is not ("Accounting" or "Reception"))
            return BadRequest();

        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket != null)
        {
            var department = await DepartmentTicketService.GetOrCreateAsync(_db, targetDepartment);
            ticket.DepartmentId = department.DepartmentId;
            ticket.AssignedTo = null;
            ticket.Status = "escalated";
            ticket.UpdatedAt = DateTime.Now;

            string targetName = targetDepartment switch
            {
                "Accounting" => "ฝ่ายบัญชีและการเงิน (ตรวจสอบยอดโอน/คืนเงินมัดจำ)",
                "Reception"  => "ฝ่ายต้อนรับและห้องพัก (เปลี่ยนวันเข้าพัก/จัดเตรียมห้อง)",
                _ => targetDepartment
            };

            // บันทึก Internal Message บันทึกการส่งต่อเรื่อง
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            _db.TicketMessages.Add(new TicketMessage
            {
                TicketId = ticket.TicketId,
                SenderId = currentUserId.HasValue ? (uint)currentUserId.Value : ticket.CustomerId,
                Message = $"[ระบบส่งต่อเรื่อง - {targetName}] บันทึกประสานงาน: {(string.IsNullOrEmpty(note) ? "ส่งเรื่องต่อเพื่อดำเนินการ" : note)}",
                IsInternal = true,
                CreatedAt = DateTime.Now
            });

            var activeUsers = await _db.Users.Include(u => u.Role)
                .Where(u => u.Status.ToLower() == "active")
                .ToListAsync();
            var recipients = activeUsers.Where(u => u.DepartmentId == department.DepartmentId ||
                (targetDepartment == "Accounting" &&
                    (u.Role.RoleCode.Equals("ACCOUNTANT", StringComparison.OrdinalIgnoreCase) ||
                     u.Role.RoleName.Contains("บัญชี", StringComparison.OrdinalIgnoreCase) ||
                     u.Role.RoleName.Contains("account", StringComparison.OrdinalIgnoreCase))) ||
                (targetDepartment == "Reception" &&
                    (u.Role.RoleCode.Equals("RECEPTION", StringComparison.OrdinalIgnoreCase) ||
                     u.Role.RoleCode.Equals("STAFF", StringComparison.OrdinalIgnoreCase) ||
                     u.Role.RoleName.Contains("ต้อนรับ", StringComparison.OrdinalIgnoreCase) ||
                     u.Role.RoleName.Contains("reception", StringComparison.OrdinalIgnoreCase) ||
                     u.Role.RoleName.Contains("staff", StringComparison.OrdinalIgnoreCase))));

            foreach (var recipient in recipients)
            {
                NotificationHelper.Add(_db, recipient.UserId,
                    $"มี Ticket ใหม่ส่งถึง{(targetDepartment == "Accounting" ? "ฝ่ายบัญชี" : "ฝ่ายต้อนรับ")}",
                    $"{ticket.TicketCode} · {ticket.Subject}", "department_ticket", ticket.TicketId);
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"คัดกรองและส่งต่อเรื่อง #{ticket.TicketCode} ไปยัง {targetName} เรียบร้อยแล้ว";
        }
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // POST: /CustomerService/ResolveTicket
    // บันทึกการแก้ไขเสร็จสิ้นและปิดเรื่อง
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResolveTicket(uint ticketId, string? resolution)
    {
        var accessCheck = ValidateCustomerServiceAccess();
        if (accessCheck != null) return accessCheck;

        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket != null)
        {
            ticket.Status = "resolved";
            ticket.ResolvedAt = DateTime.Now;
            ticket.UpdatedAt = DateTime.Now;

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!string.IsNullOrWhiteSpace(resolution))
            {
                _db.TicketMessages.Add(new TicketMessage
                {
                    TicketId = ticket.TicketId,
                    SenderId = currentUserId.HasValue ? (uint)currentUserId.Value : ticket.CustomerId,
                    Message = $"[คำตอบ / ข้อสรุปการบริการ] {resolution}",
                    IsInternal = false,
                    CreatedAt = DateTime.Now
                });
            }

            NotificationHelper.Add(_db, ticket.CustomerId, "มีคำตอบจาก Customer Service",
                $"Ticket {ticket.TicketCode} ได้รับการอัปเดตแล้ว", "ticket", ticket.TicketId);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"บันทึกคำตอบและปิดเรื่อง #{ticket.TicketCode} เรียบร้อยแล้ว";
        }
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // Helper: ตรวจสอบและเตรียมข้อมูล Tickets ตัวอย่างเบื้องต้น
    // ==========================================
    private void EnsureDefaultTickets()
    {
        try
        {
            if (!_db.Tickets.Any())
            {
                var user = _db.Users.FirstOrDefault(u => u.Email == "somchai@gmail.com") ?? _db.Users.FirstOrDefault();
                var booking = _db.Bookings.FirstOrDefault();

                if (user != null)
                {
                    _db.Tickets.AddRange(
                        new Ticket
                        {
                            TicketCode = "TK20261101",
                            CustomerId = user.UserId,
                            BookingId = booking?.BookingId,
                            Subject = "สอบถามหลักฐานการโอนเงินค่ามัดจำ 5% และขอใบเสร็จรับเงิน",
                            Priority = "urgent",
                            Status = "open",
                            CreatedAt = DateTime.Now.AddHours(-6),
                            UpdatedAt = DateTime.Now.AddHours(-6)
                        },
                        new Ticket
                        {
                            TicketCode = "TK20261102",
                            CustomerId = user.UserId,
                            BookingId = booking?.BookingId,
                            Subject = "ขอประสานงานแจ้งเวลาเข้าพักล่วงหน้า (09:00 น.) และขอเตียงเสริมเพิ่ม",
                            Priority = "normal",
                            Status = "open",
                            CreatedAt = DateTime.Now.AddHours(-3),
                            UpdatedAt = DateTime.Now.AddHours(-3)
                        },
                        new Ticket
                        {
                            TicketCode = "TK20261103",
                            CustomerId = user.UserId,
                            Subject = "สอบถามเงื่อนไขโปรโมชั่นจองล่วงหน้า 30 วัน (ลด 25%)",
                            Priority = "low",
                            Status = "resolved",
                            CreatedAt = DateTime.Now.AddDays(-1),
                            ResolvedAt = DateTime.Now.AddHours(-12),
                            UpdatedAt = DateTime.Now.AddHours(-12)
                        }
                    );
                    _db.SaveChanges();
                }
            }
        }
        catch
        {
            // ป้องกันข้อผิดพลาดกรณีติด constraint
        }
    }
}

