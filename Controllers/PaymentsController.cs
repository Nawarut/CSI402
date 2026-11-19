using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.Services;
using Nawarut_8250_L005.ViewModels;

namespace Nawarut_8250_L005.Controllers;

public class PaymentsController : Controller
{
    private readonly Csi402dbContext _db;
    public PaymentsController(Csi402dbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Checkout(uint bookingId, string type = "deposit")
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account", new { returnUrl = $"/Payments/Checkout?bookingId={bookingId}&type={type}" });
        if (type is not ("deposit" or "balance")) return BadRequest();

        var booking = await _db.Bookings.Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == (uint)userId.Value);
        if (booking == null || booking.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase)) return NotFound();
        if (type == "balance" && booking.Payments.Where(p => p.PaymentType == "deposit" && p.Status == "success").Sum(p => p.Amount) < booking.DepositAmount)
            return RedirectToAction(nameof(Checkout), new { bookingId, type = "deposit" });

        var amount = AmountDue(booking, type);
        if (amount <= 0)
        {
            var lastPayment = booking.Payments.Where(p => p.PaymentType == type && p.Status == "success")
                .OrderByDescending(p => p.PaidAt).FirstOrDefault();
            return lastPayment != null ? RedirectToAction(nameof(PaymentSlip), new { paymentId = lastPayment.PaymentId }) : RedirectToAction("BookingHistory", "Account");
        }

        return View(new PaymentCheckoutViewModel { Booking = booking, PaymentType = type, Amount = amount });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessMockPayment(uint bookingId, string type, string method,
        string? cardholder, string? cardNumber, string? expiry, string? cvv)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");
        if (type is not ("deposit" or "balance")) return BadRequest();

        var booking = await _db.Bookings.Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == (uint)userId.Value);
        if (booking == null || booking.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase)) return NotFound();
        if (type == "balance" && booking.Payments.Where(p => p.PaymentType == "deposit" && p.Status == "success").Sum(p => p.Amount) < booking.DepositAmount)
            return RedirectToAction(nameof(Checkout), new { bookingId, type = "deposit" });

        if (method is not ("promptpay" or "credit_card"))
        {
            TempData["ErrorMessage"] = "กรุณาเลือกวิธีชำระเงิน";
            return RedirectToAction(nameof(Checkout), new { bookingId, type });
        }
        if (method == "credit_card")
        {
            var digits = Regex.Replace(cardNumber ?? string.Empty, "\\D", string.Empty);
            if (string.IsNullOrWhiteSpace(cardholder) || digits.Length is < 12 or > 19 ||
                !Regex.IsMatch(expiry ?? string.Empty, "^(0[1-9]|1[0-2])/[0-9]{2}$") ||
                !Regex.IsMatch(cvv ?? string.Empty, "^[0-9]{3,4}$"))
            {
                TempData["ErrorMessage"] = "กรุณากรอกข้อมูลบัตรจำลองให้ครบและถูกต้อง";
                return RedirectToAction(nameof(Checkout), new { bookingId, type });
            }
        }

        var amount = AmountDue(booking, type);
        if (amount <= 0)
        {
            var paid = booking.Payments.Where(p => p.PaymentType == type && p.Status == "success")
                .OrderByDescending(p => p.PaidAt).FirstOrDefault();
            return paid != null ? RedirectToAction(nameof(PaymentSlip), new { paymentId = paid.PaymentId }) : RedirectToAction("BookingHistory", "Account");
        }

        var payment = new Payment
        {
            BookingId = booking.BookingId,
            PaymentType = type,
            Amount = amount,
            Status = "success",
            Method = method,
            TransactionRef = $"MOCK-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..30],
            PaidAt = DateTime.Now
        };
        _db.Payments.Add(payment);
        if (type == "deposit")
        {
            booking.Status = "confirmed";
            booking.ConfirmedAt = DateTime.Now;
            booking.UpdatedAt = DateTime.Now;
        }
        await _db.SaveChangesAsync();
        NotificationHelper.Add(_db, booking.UserId,
            type == "deposit" ? "ชำระค่ามัดจำสำเร็จ" : "ชำระยอดค่าห้องสำเร็จ",
            $"การจอง {booking.BookingCode} · ฿{amount:N2} · เลขที่สลิป {payment.TransactionRef}",
            "payment", payment.PaymentId);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(PaymentSlip), new { paymentId = payment.PaymentId });
    }

    [HttpGet]
    public async Task<IActionResult> PaymentSlip(uint paymentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");
        var payment = await _db.Payments
            .Include(p => p.Booking).ThenInclude(b => b.User)
            .Include(p => p.Booking).ThenInclude(b => b.BookingRooms).ThenInclude(br => br.Room).ThenInclude(r => r.RoomType)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId && p.Booking.UserId == (uint)userId.Value);
        if (payment == null || payment.Status != "success") return NotFound();
        return View(payment);
    }

    private static decimal AmountDue(Booking booking, string type)
    {
        var paid = booking.Payments.Where(p => p.PaymentType == type && p.Status == "success").Sum(p => p.Amount);
        var due = type == "deposit"
            ? booking.DepositAmount
            : booking.NetAmount - booking.DepositAmount;
        return Math.Max(0m, due - paid);
    }
}
