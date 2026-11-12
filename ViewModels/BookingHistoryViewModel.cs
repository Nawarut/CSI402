namespace Nawarut_8250_L005.ViewModels;

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
        (Status == "Confirmed" || Status == "Pending") &&
        CheckInDate >= DateOnly.FromDateTime(DateTime.Today);

    public bool IsCancelled => Status == "Cancelled";

    public string StatusLabel => Status switch
    {
        "Confirmed" => "ยืนยันแล้ว",
        "Pending"   => "รอดำเนินการ",
        "Cancelled" => "ยกเลิกแล้ว",
        "CheckedIn" => "เช็คอินแล้ว",
        "CheckedOut"=> "เช็คเอาท์แล้ว",
        "Completed" => "เสร็จสิ้น",
        _ => Status
    };

    public string StatusColor => Status switch
    {
        "Confirmed"  => "var(--color-cyan-signal)",
        "Pending"    => "var(--color-iris-gleam)",
        "Cancelled"  => "#ef4444",
        "CheckedIn"  => "var(--color-orchid-bloom)",
        "CheckedOut" => "var(--color-ash)",
        "Completed"  => "var(--color-periwinkle)",
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
    public int ActiveBookings => Bookings.Count(b => b.Status is "Confirmed" or "Pending" or "CheckedIn");
    public int CancelledBookings => Bookings.Count(b => b.IsCancelled);
    public int CompletedBookings => Bookings.Count(b => b.Status is "Completed" or "CheckedOut");
}

