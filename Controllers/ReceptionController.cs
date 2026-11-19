using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.ViewModels;
using Nawarut_8250_L005.Services;

namespace Nawarut_8250_L005.Controllers;

public class ReceptionController : Controller
{
    private readonly Csi402dbContext _db;

    public ReceptionController(Csi402dbContext db)
    {
        _db = db;
    }

    private IActionResult? ValidateReceptionAccess()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrWhiteSpace(role))
            return RedirectToAction("Login", "Account", new { returnUrl = "/Reception" });

        if (role.Equals("Staff", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Reception", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            return null;

        TempData["ErrorMessage"] = "คุณไม่มีสิทธิ์ดำเนินการในหน้าพนักงานต้อนรับ";
        return RedirectToAction("Index", "Home");
    }

    // ==========================================
    // GET: /Reception/Index
    // แดชบอร์ดและแผงปฏิบัติงานพนักงานต้อนรับ / ดูแลห้องพัก
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Index(string? search = null, string? roomFilter = null)
    {
        var accessCheck = ValidateReceptionAccess();
        if (accessCheck != null) return accessCheck;

        ViewBag.DepartmentTickets = await DepartmentTicketService.GetOpenTicketsAsync(_db, "Reception");

        var today = DateOnly.FromDateTime(DateTime.Today);

        // 1. ดึงข้อมูลห้องพักทั้งหมด
        var allRooms = await _db.Rooms.Include(r => r.RoomType).OrderBy(r => r.RoomNumber).ToListAsync();

        int totalRooms = allRooms.Count;
        int availableRooms = allRooms.Count(r => r.Status.ToLower() is "available" or "ว่าง");
        int occupiedRooms = allRooms.Count(r => r.Status.ToLower() is "occupied" or "เข้าพักอยู่" or "reserved" or "booked");
        int cleaningRooms = allRooms.Count(r => r.Status.ToLower() is "cleaning" or "กำลังทำความสะอาด");
        int maintenanceRooms = allRooms.Count(r => r.Status.ToLower() is "maintenance" or "ซ่อมบำรุง");

        // 2. ดึงข้อมูลรายการจอง
        var bookingsQuery = _db.Bookings
            .Include(b => b.User)
            .Include(b => b.Payments)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
                    .ThenInclude(r => r.RoomType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            bookingsQuery = bookingsQuery.Where(b => b.BookingCode.ToLower().Contains(s) ||
                                                    (b.User != null && (b.User.FirstName.ToLower().Contains(s) ||
                                                                        b.User.LastName.ToLower().Contains(s) ||
                                                                        (b.User.Phone != null && b.User.Phone.ToLower().Contains(s)))));
        }

        var allBookings = await bookingsQuery.OrderByDescending(b => b.CreatedAt).ToListAsync();

        int arrivalsToday = allBookings.Count(b => b.CheckInDate == today && (b.Status.ToLower() is "confirmed" or "pending"));
        int departuresToday = allBookings.Count(b => b.CheckOutDate == today && (b.Status.ToLower() is "checked_in" or "checkedin"));

        ViewBag.TotalRooms = totalRooms;
        ViewBag.AvailableRooms = availableRooms;
        ViewBag.OccupiedRooms = occupiedRooms;
        ViewBag.CleaningRooms = cleaningRooms;
        ViewBag.MaintenanceRooms = maintenanceRooms;
        ViewBag.ArrivalsToday = arrivalsToday;
        ViewBag.DeparturesToday = departuresToday;
        ViewBag.Search = search;
        ViewBag.RoomFilter = roomFilter;
        ViewBag.Rooms = allRooms;

        return View(allBookings);
    }

    // ==========================================
    // POST: /Reception/ConfirmBooking
    // ยืนยันการจองของลูกค้า
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmBooking(uint bookingId)
    {
        var accessCheck = ValidateReceptionAccess();
        if (accessCheck != null) return accessCheck;

        var booking = await _db.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            booking.Status = "confirmed";
            booking.ConfirmedAt = DateTime.Now;
            booking.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"ยืนยันการจอง #{booking.BookingCode} เรียบร้อยแล้ว";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CollectPayment(uint bookingId)
    {
        var accessCheck = ValidateReceptionAccess();
        if (accessCheck != null) return accessCheck;

        var booking = await _db.Bookings.Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        if (booking == null || booking.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "ไม่พบรายการจองหรือรายการจองถูกยกเลิกแล้ว";
            return RedirectToAction(nameof(Index));
        }

        var depositPaid = booking.Payments
            .Where(p => p.PaymentType == "deposit" && p.Status == "success").Sum(p => p.Amount);
        var balancePaid = booking.Payments
            .Where(p => p.PaymentType == "balance" && p.Status == "success").Sum(p => p.Amount);
        var depositDue = Math.Max(0m, booking.DepositAmount - depositPaid);
        var amount = depositDue > 0 ? depositDue : Math.Max(0m, booking.NetAmount - booking.DepositAmount - balancePaid);
        if (amount <= 0)
        {
            TempData["InfoMessage"] = "รายการจองนี้ชำระครบแล้ว";
            return RedirectToAction(nameof(Index));
        }

        var payment = new Payment
        {
            BookingId = booking.BookingId,
            PaymentType = depositDue > 0 ? "deposit" : "balance",
            Amount = amount,
            Status = "success",
            Method = "cash",
            TransactionRef = $"CASH-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..30],
            PaidAt = DateTime.Now
        };
        _db.Payments.Add(payment);
        if (depositDue > 0)
        {
            booking.Status = "confirmed";
            booking.ConfirmedAt ??= DateTime.Now;
        }
        booking.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        NotificationHelper.Add(_db, booking.UserId,
            payment.PaymentType == "deposit" ? "รับชำระค่ามัดจำที่ Reception แล้ว" : "รับชำระยอดค่าห้องที่ Reception แล้ว",
            $"การจอง {booking.BookingCode} · ฿{amount:N2} · เลขที่รายการ {payment.TransactionRef}",
            "payment", payment.PaymentId);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"บันทึกรับชำระ{(payment.PaymentType == "deposit" ? "ค่ามัดจำ" : "ยอดคงเหลือ")} ฿{amount:N2} แล้ว";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // POST: /Reception/CheckIn
    // บันทึกการเข้าพัก (Check-in 09:00 น.)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(uint bookingId)
    {
        var accessCheck = ValidateReceptionAccess();
        if (accessCheck != null) return accessCheck;

        var booking = await _db.Bookings
            .Include(b => b.BookingRooms)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        if (booking != null)
        {
            var depositPaid = booking.Payments.Where(p => p.PaymentType == "deposit" && p.Status == "success").Sum(p => p.Amount);
            var balancePaid = booking.Payments.Where(p => p.PaymentType == "balance" && p.Status == "success").Sum(p => p.Amount);
            if (depositPaid < booking.DepositAmount || depositPaid + balancePaid < booking.NetAmount)
            {
                TempData["ErrorMessage"] = "กรุณารับชำระเงินมัดจำและยอดคงเหลือให้ครบก่อนเช็กอิน";
                return RedirectToAction(nameof(Index));
            }

            booking.Status = "checked_in";
            booking.CheckedInAt = DateTime.Now;
            booking.UpdatedAt = DateTime.Now;

            // ปรับสถานะห้องที่เกี่ยวข้องเป็น occupied (มีผู้เข้าพัก)
            foreach (var br in booking.BookingRooms)
            {
                var room = await _db.Rooms.FindAsync(br.RoomId);
                if (room != null)
                {
                    room.Status = "occupied";
                    room.UpdatedAt = DateTime.Now;
                }
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"เช็กอิน #{booking.BookingCode} เรียบร้อยแล้ว (ชำระครบแล้ว)";
        }
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // POST: /Reception/CheckOut
    // บันทึกการคืนห้อง (Check-out 12:00 น.)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(uint bookingId)
    {
        var accessCheck = ValidateReceptionAccess();
        if (accessCheck != null) return accessCheck;

        var booking = await _db.Bookings
            .Include(b => b.BookingRooms)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        if (booking != null)
        {
            booking.Status = "checked_out";
            booking.CheckedOutAt = DateTime.Now;
            booking.UpdatedAt = DateTime.Now;

            // ปรับสถานะห้องเป็น cleaning (กำลังทำความสะอาด) เพื่อให้แม่บ้านดูแลต่อ
            foreach (var br in booking.BookingRooms)
            {
                var room = await _db.Rooms.FindAsync(br.RoomId);
                if (room != null)
                {
                    room.Status = "cleaning";
                    room.UpdatedAt = DateTime.Now;
                }
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"บันทึกเช็คเอาท์ (Check-out) สำหรับการจอง #{booking.BookingCode} เรียบร้อยแล้ว (สถานะห้องปรับเป็น: กำลังทำความสะอาด)";
        }
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // POST: /Reception/UpdateRoomStatus
    // อัปเดตสถานะห้องพักหน้างาน (ว่าง, มีผู้พัก, กำลังทำความสะอาด, ซ่อมบำรุง)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoomStatus(uint roomId, string status)
    {
        var accessCheck = ValidateReceptionAccess();
        if (accessCheck != null) return accessCheck;

        var room = await _db.Rooms.FindAsync(roomId);
        if (room != null)
        {
            string dbStatus = (status ?? "").Trim().ToLowerInvariant() switch
            {
                "available" or "ว่าง" or "ว่างพร้อมให้บริการ" => "available",
                "occupied" or "เข้าพักอยู่" or "มีผู้เข้าพัก" => "occupied",
                "cleaning" or "กำลังทำความสะอาด" => "cleaning",
                "maintenance" or "ซ่อมบำรุง" => "maintenance",
                "reserved" or "booked" or "จองแล้ว" => "reserved",
                _ => "available"
            };

            room.Status = dbStatus;
            room.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            string statusLabel = dbStatus switch
            {
                "available" => "ว่างพร้อมให้บริการ",
                "occupied"  => "มีผู้เข้าพัก",
                "cleaning"  => "กำลังทำความสะอาด",
                "maintenance" => "ซ่อมบำรุง",
                _ => dbStatus
            };

            TempData["SuccessMessage"] = $"อัปเดตห้องพัก #{room.RoomNumber} เป็น \"{statusLabel}\" เรียบร้อยแล้ว";
        }
        return RedirectToAction(nameof(Index));
    }
}

