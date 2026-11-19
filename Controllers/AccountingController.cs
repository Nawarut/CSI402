using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.Services;

namespace Nawarut_8250_L005.Controllers;

public class AccountingController : Controller
{
    private readonly Csi402dbContext _db;

    public AccountingController(Csi402dbContext db)
    {
        _db = db;
    }

    // ==========================================
    // GET: /Accounting/Index
    // แดชบอร์ดตรวจสอบการเงิน ยอดโอน คืนเงินมัดจำ และใบเสร็จ
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role))
        {
            return RedirectToAction("Login", "Account", new { returnUrl = "/Accounting" });
        }
        if (!role.Equals("Accountant", StringComparison.OrdinalIgnoreCase) &&
            !role.Equals("Accounting", StringComparison.OrdinalIgnoreCase) &&
            !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "คุณไม่มีสิทธิ์เข้าถึงหน้าฝ่ายบัญชีและการเงิน (เฉพาะเจ้าหน้าที่เท่านั้น)";
            return RedirectToAction("Index", "Home");
        }

        ViewBag.DepartmentTickets = await DepartmentTicketService.GetOpenTicketsAsync(_db, "Accounting");

        // 1. ดึงข้อมูลการจองทั้งหมดสำหรับการคำนวณการเงิน
        var bookings = await _db.Bookings
            .Include(b => b.User)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
                    .ThenInclude(r => r.RoomType)
            .Include(b => b.Payments)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        // 2. ดึงข้อมูลการชำระเงินทั้งหมด (Payments)
        var payments = await _db.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // 3. คำนวณสรุปสถิติทางการเงินจริง
        decimal totalNetRevenue = bookings.Where(b => !string.Equals(b.Status, "cancelled", StringComparison.OrdinalIgnoreCase)).Sum(b => b.NetAmount);
        decimal totalDepositsCollected = payments.Where(p => p.PaymentType == "deposit" && p.Status == "success").Sum(p => p.Amount);
        decimal pendingRefundsAmount = payments.Where(p => p.PaymentType == "refund" && p.Status == "pending").Sum(p => p.Amount);
        decimal totalRefundedAmount = payments.Where(p => p.PaymentType == "refund" && (p.Status == "success" || p.Status == "refunded")).Sum(p => p.Amount);
        decimal pendingBalanceAmount = bookings
            .Where(b => (b.Status ?? "").ToLower() is "confirmed" or "checked_in" or "checkedin")
            .Sum(b => Math.Max(0m, b.NetAmount - b.DepositAmount - b.Payments
                .Where(p => p.PaymentType == "balance" && p.Status == "success").Sum(p => p.Amount)));

        ViewBag.TotalNetRevenue = totalNetRevenue;
        ViewBag.TotalDepositsCollected = totalDepositsCollected;
        ViewBag.PendingRefundsAmount = pendingRefundsAmount;
        ViewBag.TotalRefundedAmount = totalRefundedAmount;
        ViewBag.PendingBalanceAmount = pendingBalanceAmount;
        ViewBag.Payments = payments;
        ViewBag.Bookings = bookings;

        return View();
    }

    // ==========================================
    // POST: /Accounting/ConfirmPayment
    // ยืนยันยอดเงินโอน (Pending -> Success)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayment(uint paymentId)
    {
        var payment = await _db.Payments.FindAsync(paymentId);
        if (payment != null)
        {
            payment.Status = "success";
            payment.PaidAt = DateTime.Now;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"ยืนยันยอดเงินโอน ฿{payment.Amount:N2} เรียบร้อยแล้ว (สถานะ: ชำระสำเร็จ)";
        }
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // POST: /Accounting/ProcessRefund
    // อนุมัติการคืนเงินค่ามัดจำ 5% (Pending -> Refunded)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessRefund(uint paymentId)
    {
        var payment = await _db.Payments.FindAsync(paymentId);
        if (payment != null)
        {
            payment.Status = "refunded";
            payment.PaidAt = DateTime.Now;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"อนุมัติและทำรายการคืนเงินค่ามัดจำ 5% จำนวน ฿{payment.Amount:N2} เรียบร้อยแล้ว";
        }
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // GET: /Accounting/Receipt/{id}
    // ดูและพิมพ์ใบเสร็จรับเงิน (Official Receipt)
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Receipt(uint id)
    {
        var booking = await _db.Bookings
            .Include(b => b.User)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
                    .ThenInclude(r => r.RoomType)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == id);

        if (booking == null)
        {
            TempData["ErrorMessage"] = "ไม่พบรายการจองดังกล่าว";
            return RedirectToAction(nameof(Index));
        }

        return View(booking);
    }
}

