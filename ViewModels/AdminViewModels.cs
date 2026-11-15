namespace Nawarut_8250_L005.ViewModels;

// ==========================================
// 1. Dashboard ViewModel
// ==========================================
public class AdminDashboardViewModel
{
    // ห้องพัก (Rooms)
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int MaintenanceRooms { get; set; }
    public int OccupancyRate => TotalRooms > 0 ? (int)Math.Round((double)OccupiedRooms / TotalRooms * 100) : 0;

    // ผู้ใช้ (Users)
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int AdminCount { get; set; }
    public int StaffUsers { get; set; }
    public int RenterUsers { get; set; }

    // การจอง (Bookings)
    public int TotalBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int PendingBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal TotalDepositAmount { get; set; }

    // รายการจองล่าสุด
    public List<AdminBookingItemViewModel> RecentBookings { get; set; } = new();
}

// ==========================================
// 2. Room Management ViewModels
// ==========================================
public class AdminRoomItemViewModel
{
    public uint RoomId { get; set; }
    public string RoomNumber { get; set; } = "";
    public byte? FloorNo { get; set; }
    public string Status { get; set; } = "Available"; // Available, Booked, Maintenance
    public decimal PricePerNight { get; set; }
    public ushort RoomTypeId { get; set; }
    public string TypeName { get; set; } = "";
    public string BedType { get; set; } = "";
    public decimal? SizeSqm { get; set; }
    public byte MaxGuests { get; set; }
    public bool IsActive { get; set; } = true;

    public string StatusBadgeColor => (Status ?? "").ToLowerInvariant() switch
    {
        "available" or "ว่าง" => "var(--color-cyan-signal)",
        "booked" or "reserved" or "จองแล้ว" => "var(--color-orchid-bloom)",
        "occupied" or "เข้าพักอยู่" => "var(--color-iris-gleam)",
        "cleaning" or "กำลังทำความสะอาด" => "#fcd34d",
        "maintenance" or "ซ่อมบำรุง" => "#f59e0b",
        _ => "var(--color-ash)"
    };

    public string StatusLabel => (Status ?? "").ToLowerInvariant() switch
    {
        "available" or "ว่าง" => "ว่างพร้อมให้บริการ",
        "booked" or "reserved" or "จองแล้ว" => "ถูกจองแล้ว",
        "occupied" or "เข้าพักอยู่" => "มีผู้เข้าพัก",
        "cleaning" or "กำลังทำความสะอาด" => "กำลังทำความสะอาด",
        "maintenance" or "ซ่อมบำรุง" => "ซ่อมบำรุง",
        _ => Status
    };
}

public class AdminRoomListViewModel
{
    public List<AdminRoomItemViewModel> Rooms { get; set; } = new();
    public int TotalRooms => Rooms.Count;
    public int AvailableCount => Rooms.Count(r => (r.Status ?? "").ToLower() is "available" or "ว่าง");
    public int BookedCount => Rooms.Count(r => (r.Status ?? "").ToLower() is "booked" or "reserved" or "จองแล้ว" or "occupied" or "เข้าพักอยู่");
    public int MaintenanceCount => Rooms.Count(r => (r.Status ?? "").ToLower() is "maintenance" or "ซ่อมบำรุง");

    public string? Search { get; set; }
    public string? StatusFilter { get; set; }
}

// ==========================================
// 3. Promotion Management ViewModels
// ==========================================
public class AdminPromotionItemViewModel
{
    public uint PromotionId { get; set; }
    public string PromoCode { get; set; } = "";
    public string PromoName { get; set; } = "";
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "percent"; // percent, fixed
    public decimal DiscountValue { get; set; }
    public string ConditionType { get; set; } = "";
    public decimal ConditionValue { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageCount { get; set; }

    public string DiscountDisplay => DiscountType == "fixed"
        ? $"ลด ฿{DiscountValue:N0}"
        : $"ลด {DiscountValue:N0}%";

    private static bool ContainsMojibake(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        if (text.Any(c => c >= '\u0E01' && c <= '\u0E5B')) return false;
        return text.Any(c => (c >= 0x00A0 && c <= 0x00FF) || c == '╣' || c == '┤' || c == '¿' || c == 'ó' || c == 'Ó' || c == '©');
    }

    public string CleanPromoName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(PromoName) && !ContainsMojibake(PromoName) && !PromoName.Contains('Ó') && !PromoName.Contains('ó') && !PromoName.Contains('©'))
                return PromoName;

            string upper = (PromoCode ?? "").ToUpperInvariant().Trim();
            if (upper.Contains("NEW") || upper.Contains("MEMBER") || upper.Contains("10"))
                return "สมาชิกใหม่จองครั้งแรก";
            if (upper.Contains("LONG") || upper.Contains("STAY") || upper.Contains("15") || upper.Contains("NIGHT"))
                return "เข้าพัก 2 คืนขึ้นไป";
            if (upper.Contains("500") || upper.Contains("BIG") || upper.Contains("OVER"))
                return "ยอดจองเกิน 5,000 บาท";
            if (upper.Contains("GROUP") || upper.Contains("20") || upper.Contains("4"))
                return "เข้าพักเป็นกลุ่ม 4 ท่านขึ้นไป";
            if (upper.Contains("EARLY") || upper.Contains("ADVANCE") || upper.Contains("25") || upper.Contains("30"))
                return "จองล่วงหน้า 30 วัน";

            return PromoName ?? PromoCode;
        }
    }

    public string CleanDescription
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Description) && !ContainsMojibake(Description) && !Description.Contains('Ó') && !Description.Contains('ó') && !Description.Contains('©'))
                return Description;

            string upper = (PromoCode ?? "").ToUpperInvariant().Trim();
            if (upper.Contains("NEW") || upper.Contains("MEMBER") || upper.Contains("10"))
                return "ลด 10% สำหรับสมาชิกใหม่ที่ทำการจองห้องพักครั้งแรก";
            if (upper.Contains("LONG") || upper.Contains("STAY") || upper.Contains("15") || upper.Contains("NIGHT"))
                return "ลด 15% เมื่อจองและเข้าพักตั้งแต่ 2 คืนขึ้นไป";
            if (upper.Contains("500") || upper.Contains("BIG") || upper.Contains("OVER"))
                return "ลดทันที 500 บาท เมื่อยอดค่าห้องพักรวมเกิน 5,000 บาท";
            if (upper.Contains("GROUP") || upper.Contains("20") || upper.Contains("4"))
                return "ลด 20% เมื่อเข้าพักเป็นกลุ่มตั้งแต่ 4 ท่านขึ้นไป";
            if (upper.Contains("EARLY") || upper.Contains("ADVANCE") || upper.Contains("25") || upper.Contains("30"))
                return "ลด 25% เมื่อจองล่วงหน้าอย่างน้อย 30 วันก่อนวันเช็คอิน";

            return Description ?? "";
        }
    }
}

public class AdminPromotionListViewModel
{
    public List<AdminPromotionItemViewModel> Promotions { get; set; } = new();
    public int TotalPromotions => Promotions.Count;
    public int ActiveCount => Promotions.Count(p => p.IsActive);
    public int InactiveCount => Promotions.Count(p => !p.IsActive);
    public int TotalUsage => Promotions.Sum(p => p.UsageCount);

    public string? Search { get; set; }
    public string? StatusFilter { get; set; }
}

// ==========================================
// 4. Booking Management ViewModels
// ==========================================
public class AdminBookingItemViewModel
{
    public uint BookingId { get; set; }
    public string BookingCode { get; set; } = "";
    public string GuestName { get; set; } = "";
    public string GuestEmail { get; set; } = "";
    public string GuestPhone { get; set; } = "";
    public string RoomSummary { get; set; } = "";
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Nights { get; set; }
    public int TotalGuests { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal DepositPercent { get; set; }
    public string Status { get; set; } = "Confirmed";
    public DateTime CreatedAt { get; set; }
    public string? PaymentStatus { get; set; }

    public string StatusLabel => (Status ?? "").ToLowerInvariant() switch
    {
        "confirmed"  => "ยืนยันแล้ว",
        "pending"    => "รอดำเนินการ",
        "checkedin" or "checked_in"  => "เช็คอินแล้ว",
        "checkedout" or "checked_out" => "เช็คเอาท์แล้ว",
        "completed"  => "เสร็จสิ้น",
        "cancelled"  => "ยกเลิกแล้ว",
        _ => Status
    };

    public string StatusColor => (Status ?? "").ToLowerInvariant() switch
    {
        "confirmed"  => "var(--color-cyan-signal)",
        "pending"    => "var(--color-iris-gleam)",
        "checkedin" or "checked_in"  => "var(--color-orchid-bloom)",
        "checkedout" or "checked_out" => "var(--color-ash)",
        "completed"  => "var(--color-periwinkle)",
        "cancelled"  => "#ef4444",
        _ => "var(--color-ash)"
    };
}

public class AdminBookingListViewModel
{
    public List<AdminBookingItemViewModel> Bookings { get; set; } = new();
    public int TotalBookings => Bookings.Count;
    public int ConfirmedCount => Bookings.Count(b => (b.Status ?? "").ToLower() is "confirmed" or "checkedin" or "checked_in");
    public int PendingCount => Bookings.Count(b => (b.Status ?? "").ToLower() == "pending");
    public int CancelledCount => Bookings.Count(b => (b.Status ?? "").ToLower() == "cancelled");
    public decimal TotalDepositCollected => Bookings.Where(b => (b.Status ?? "").ToLower() != "cancelled").Sum(b => b.DepositAmount);

    public string? Search { get; set; }
    public string? StatusFilter { get; set; }
}

