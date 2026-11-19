namespace Nawarut_8250_L005.ViewModels;

public class PaymentHistoryItemViewModel
{
    public uint PaymentId { get; set; }
    public string PaymentType { get; set; } = "";
    public decimal Amount { get; set; }
    public string Method { get; set; } = "";
    public DateTime? PaidAt { get; set; }
}

/// <summary>
/// สรุปข้อมูลห้องพักในการจอง
/// </summary>
public class BookingRoomSummary
{
    public string RoomNumber { get; set; } = "";
    public string TypeName { get; set; } = "";
    public decimal PricePerNight { get; set; }
    public int ExtraBeds { get; set; }
    public decimal ExtraBedPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int Nights { get; set; }
}

/// <summary>
/// ข้อมูลรายการจองแต่ละรายการ
/// </summary>
public class BookingHistoryItemViewModel
{
    public uint BookingId { get; set; }
    public string BookingCode { get; set; } = "";
    public string Status { get; set; } = "";
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Nights { get; set; }
    public int TotalGuests { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal ExtraBedAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal DepositPercent { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal DepositPaid { get; set; }
    public decimal DepositRemaining => Math.Max(0m, DepositAmount - DepositPaid);
    public decimal BalancePaid { get; set; }
    public decimal BalanceRemaining => Math.Max(0m, NetAmount - DepositAmount - BalancePaid);
    public List<PaymentHistoryItemViewModel> SuccessfulPayments { get; set; } = new();
    public decimal RefundAmount { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CancelReason { get; set; }

    public List<BookingRoomSummary> Rooms { get; set; } = new();

    // Refund payment status (จาก Payments table)
    public string? RefundStatus { get; set; }

    // Computed properties
    public bool IsCancellable =>
        (Status.ToLower() is "confirmed" or "pending") &&
        CheckInDate >= DateOnly.FromDateTime(DateTime.Today);

    public bool IsCancelled => Status.ToLower() == "cancelled";

    public string StatusLabel => Status.ToLower() switch
    {
        "confirmed" => "ยืนยันแล้ว",
        "pending"   => "รอดำเนินการ",
        "cancelled" => "ยกเลิกแล้ว",
        "checked_in" or "checkedin" => "เช็คอินแล้ว",
        "checked_out" or "checkedout" => "เช็คเอาท์แล้ว",
        "completed" => "เสร็จสิ้น",
        _ => Status
    };

    public string StatusColor => Status.ToLower() switch
    {
        "confirmed"  => "var(--color-cyan-signal)",
        "pending"    => "var(--color-iris-gleam)",
        "cancelled"  => "#ef4444",
        "checked_in" or "checkedin"  => "var(--color-orchid-bloom)",
        "checked_out" or "checkedout" => "var(--color-ash)",
        "completed"  => "var(--color-periwinkle)",
        _ => "var(--color-ash)"
    };
}

/// <summary>
/// ViewModel หน้าประวัติการจองทั้งหมด
/// </summary>
public class BookingHistoryViewModel
{
    public List<BookingHistoryItemViewModel> Bookings { get; set; } = new();
    public int TotalBookings => Bookings.Count;
    public int ActiveBookings => Bookings.Count(b => b.Status.ToLower() is "confirmed" or "pending" or "checked_in" or "checkedin");
    public int CancelledBookings => Bookings.Count(b => b.IsCancelled);
    public int CompletedBookings => Bookings.Count(b => b.Status.ToLower() is "completed" or "checked_out" or "checkedout");
}

