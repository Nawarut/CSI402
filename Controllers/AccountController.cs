using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Models;
using Nawarut_8250_L005.Services;
using Csi402dbContext = Nawarut_8250_L005.Entities.Csi402dbContext;
using Payment = Nawarut_8250_L005.Entities.Payment;
using BookingHistoryViewModel = Nawarut_8250_L005.ViewModels.BookingHistoryViewModel;
using BookingHistoryItemViewModel = Nawarut_8250_L005.ViewModels.BookingHistoryItemViewModel;
using BookingRoomSummary = Nawarut_8250_L005.ViewModels.BookingRoomSummary;

namespace Nawarut_8250_L005.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly Csi402dbContext _db;

    public AccountController(IUserService userService, Csi402dbContext db)
    {
        _userService = userService;
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _userService.Authenticate(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือรหัสผ่านไม่ถูกต้อง โปรดลองอีกครั้ง");
            return View(model);
        }

        if (user.Status == "Inactive")
        {
            ModelState.AddModelError(string.Empty, "บัญชีผู้ใช้นี้ถูกระงับการใช้งาน กรุณาติดต่อผู้ดูแลระบบ");
            return View(model);
        }

        // Store user info in session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetString("UserRole", user.Role);

        TempData["SuccessMessage"] = $"ยินดีต้อนรับคุณ {user.FullName} เข้าสู่ระบบ";

        // Role-based redirection to dedicated portals
        if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Admin");
        }
        if (user.Role.Equals("Staff", StringComparison.OrdinalIgnoreCase) || user.Role.Equals("Reception", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Reception");
        }
        if (user.Role.Equals("Accountant", StringComparison.OrdinalIgnoreCase) || user.Role.Equals("Accounting", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Accounting");
        }
        if (user.Role.Equals("CustomerService", StringComparison.OrdinalIgnoreCase) || user.Role.Equals("Support", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "CustomerService");
        }

        // Regular User redirection
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if email already exists
        if (_userService.GetByEmail(model.Email) != null)
        {
            ModelState.AddModelError("Email", "อีเมลนี้มีผู้ใช้งานแล้วในระบบ");
            return View(model);
        }

        var newUser = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            Role = "User",
            Status = "Active",
            CreatedAt = DateTime.Now
        };

        bool created = _userService.Add(newUser);
        if (!created)
        {
            ModelState.AddModelError(string.Empty, "ไม่สามารถลงทะเบียนได้ กรุณาลองใหม่อีกครั้ง");
            return View(model);
        }

        TempData["SuccessMessage"] = "สมัครสมาชิกสำเร็จเรียบร้อยแล้ว! กรุณาเข้าสู่ระบบด้วยบัญชีของคุณ";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["InfoMessage"] = "ออกจากระบบเรียบร้อยแล้ว";
        return RedirectToAction("Login");
    }

    // ==========================================
    // GET: /Account/BookingHistory
    // ประวัติการจองของผู้ใช้ที่ล็อกอิน
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> BookingHistory()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", new { returnUrl = "/Account/BookingHistory" });
        }

        var uId = (uint)userId.Value;

        var bookings = await _db.Bookings
            .Where(b => b.UserId == uId)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
                    .ThenInclude(r => r.RoomType)
            .Include(b => b.Payments)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var items = bookings.Select(b =>
        {
            // หา refund payment ถ้ามี
            var refundPayment = b.Payments.FirstOrDefault(p => p.PaymentType == "refund");
            string? refundStatus = null;
            if (b.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
            {
                refundStatus = refundPayment?.Status switch
                {
                    "success"  => "คืนเงินแล้ว",
                    "pending"  => "อยู่ระหว่างดำเนินการ",
                    "refunded" => "คืนเงินแล้ว",
                    _          => "รอดำเนินการ"
                };
            }

            var rooms = b.BookingRooms.Select(br => new BookingRoomSummary
            {
                RoomNumber    = br.Room?.RoomNumber ?? "-",
                TypeName      = br.Room?.RoomType?.TypeName ?? "-",
                PricePerNight = br.PricePerNight,
                ExtraBeds     = br.ExtraBeds,
                ExtraBedPrice = br.ExtraBedPrice,
                LineTotal     = br.LineTotal,
                Nights        = b.Nights
            }).ToList();

            return new BookingHistoryItemViewModel
            {
                BookingId      = b.BookingId,
                BookingCode    = b.BookingCode,
                Status         = b.Status,
                CheckInDate    = b.CheckInDate,
                CheckOutDate   = b.CheckOutDate,
                Nights         = b.Nights,
                TotalGuests    = b.TotalGuests,
                SubtotalAmount = b.SubtotalAmount,
                ExtraBedAmount = b.ExtraBedAmount,
                DiscountAmount = b.DiscountAmount,
                NetAmount      = b.NetAmount,
                DepositPercent = b.DepositPercent,
                DepositAmount  = b.DepositAmount,
                DepositPaid    = b.Payments.Where(p => p.PaymentType == "deposit" && p.Status == "success").Sum(p => p.Amount),
                BalancePaid    = b.Payments.Where(p => p.PaymentType == "balance" && p.Status == "success").Sum(p => p.Amount),
                SuccessfulPayments = b.Payments.Where(p => p.Status == "success")
                    .OrderByDescending(p => p.PaidAt)
                    .Select(p => new Nawarut_8250_L005.ViewModels.PaymentHistoryItemViewModel
                    {
                        PaymentId = p.PaymentId,
                        PaymentType = p.PaymentType,
                        Amount = p.Amount,
                        Method = p.Method,
                        PaidAt = p.PaidAt
                    }).ToList(),
                RefundAmount   = b.RefundAmount,
                ConfirmedAt    = b.ConfirmedAt,
                CancelledAt    = b.CancelledAt,
                CreatedAt      = b.CreatedAt,
                CancelReason   = b.CancelReason,
                RefundStatus   = refundStatus,
                Rooms          = rooms
            };
        }).ToList();

        var vm = new BookingHistoryViewModel { Bookings = items };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Support()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", new { returnUrl = "/Account/Support" });

        var uid = (uint)userId.Value;
        ViewBag.Bookings = await _db.Bookings
            .Where(b => b.UserId == uid)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        var tickets = await _db.Tickets
            .Where(t => t.CustomerId == uid)
            .Include(t => t.Booking)
            .Include(t => t.TicketMessages)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        return View(tickets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSupportTicket(uint? bookingId, string subject, string message)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", new { returnUrl = "/Account/Support" });

        var uid = (uint)userId.Value;
        subject = (subject ?? string.Empty).Trim();
        message = (message ?? string.Empty).Trim();
        if (subject.Length is < 3 or > 150 || message.Length < 5 || message.Length > 4000)
        {
            TempData["ErrorMessage"] = "กรุณากรอกหัวข้อ 3–150 ตัวอักษร และรายละเอียด 5–4,000 ตัวอักษร";
            return RedirectToAction(nameof(Support));
        }

        if (bookingId.HasValue && !await _db.Bookings.AnyAsync(b => b.BookingId == bookingId && b.UserId == uid))
        {
            TempData["ErrorMessage"] = "ไม่พบรายการจองที่เลือก";
            return RedirectToAction(nameof(Support));
        }

        var ticket = new Nawarut_8250_L005.Entities.Ticket
        {
            TicketCode = $"TK{DateTime.Now:yyyyMMddHHmmssfff}",
            CustomerId = uid,
            BookingId = bookingId,
            Subject = subject,
            Priority = "normal",
            Status = "open",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();
        _db.TicketMessages.Add(new Nawarut_8250_L005.Entities.TicketMessage
        {
            TicketId = ticket.TicketId,
            SenderId = uid,
            Message = message,
            IsInternal = false,
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"ส่งเรื่องถึง Customer Service แล้ว หมายเลข {ticket.TicketCode}";
        return RedirectToAction(nameof(Support));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayBalance(uint bookingId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login");

        return RedirectToAction("Checkout", "Payments", new { bookingId, type = "balance" });
    }

    // ==========================================
    // POST: /Account/CancelBooking
    // ยกเลิกการจอง + บันทึก Payment Refund
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(uint bookingId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        var uId = (uint)userId.Value;
        var booking = await _db.Bookings
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == uId);

        if (booking == null)
        {
            TempData["ErrorMessage"] = "ไม่พบรายการจองดังกล่าว";
            return RedirectToAction("BookingHistory");
        }

        if (booking.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
        {
            TempData["WarningMessage"] = "รายการจองนี้ถูกยกเลิกไปแล้ว";
            return RedirectToAction("BookingHistory");
        }

        if (booking.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
        {
            TempData["ErrorMessage"] = "ไม่สามารถยกเลิกการจองย้อนหลังหลังจากวันเช็คอินได้";
            return RedirectToAction("BookingHistory");
        }

        // อัปเดตสถานะการจองเป็น cancelled
        booking.Status      = "cancelled";
        booking.CancelledAt = DateTime.Now;
        booking.RefundAmount = booking.DepositAmount;

        // ปลดสถานะห้องพักกลับเป็น available
        var bookingRooms = await _db.BookingRooms.Where(br => br.BookingId == bookingId).ToListAsync();
        foreach (var br in bookingRooms)
        {
            var room = await _db.Rooms.FindAsync(br.RoomId);
            if (room != null && (room.Status == "reserved" || room.Status == "occupied"))
            {
                room.Status = "available";
                room.UpdatedAt = DateTime.Now;
            }
        }

        // บันทึก Payment Refund
        var refundPayment = new Payment
        {
            BookingId   = booking.BookingId,
            PaymentType = "refund",
            Amount      = booking.DepositAmount,
            Status      = "pending",
            Method      = "bank_transfer",
            PaidAt      = null
        };
        _db.Payments.Add(refundPayment);

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"ยกเลิกการจอง #{booking.BookingCode} เรียบร้อยแล้ว อยู่ระหว่างดำเนินการคืนเงินค่ามัดจำ 5% (฿{booking.DepositAmount:N2}) ภายใน 3-5 วันทำการ";
        return RedirectToAction("BookingHistory");
    }
}
