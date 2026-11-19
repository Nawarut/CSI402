using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.Models;
using Nawarut_8250_L005.Services;
using Nawarut_8250_L005.ViewModels;
using DbUser = Nawarut_8250_L005.Entities.User;
using UserModel = Nawarut_8250_L005.Models.User;

namespace Nawarut_8250_L005.Controllers;

public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly Csi402dbContext _db;

    public AdminController(IUserService userService, Csi402dbContext db)
    {
        _userService = userService;
        _db = db;
        EnsureDefaultHotelData();
    }

    private IActionResult? ValidateAdminAccess()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role))
        {
            return RedirectToAction("Login", "Account", new { returnUrl = "/Admin" });
        }
        if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "คุณไม่มีสิทธิ์เข้าถึงหน้าผู้ดูแลระบบ (เฉพาะผู้ดูแลระบบเท่านั้น)";
            return RedirectToAction("Index", "Home");
        }
        return null;
    }

    // ==========================================
    // 1. Dashboard (Index) - แสดงข้อมูลสถิติจริง 100%
    // ==========================================
    public async Task<IActionResult> Index()
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        // 1. สถิติห้องพักจริง
        int totalRooms = await _db.Rooms.CountAsync();
        int availableRooms = await _db.Rooms.CountAsync(r => r.Status == "available" || r.Status == "Available" || r.Status == "ว่าง");
        int occupiedRooms = await _db.Rooms.CountAsync(r => r.Status == "occupied" || r.Status == "reserved" || r.Status == "Occupied" || r.Status == "Booked" || r.Status == "จองแล้ว" || r.Status == "เข้าพักอยู่");
        int maintenanceRooms = await _db.Rooms.CountAsync(r => r.Status == "maintenance" || r.Status == "Maintenance" || r.Status == "ซ่อมบำรุง");

        // 2. สถิติผู้ใช้งานจริง (คำนวณจาก RoleCode และ RoleName จริงในฐานข้อมูล)
        var allUsers = await _db.Users.Include(u => u.Role).ToListAsync();
        int totalUsers = allUsers.Count;
        int activeUsers = allUsers.Count(u => u.Status == "active");
        
        int adminCount = allUsers.Count(u => 
            u.Email.Equals("admin@jojo.com", StringComparison.OrdinalIgnoreCase) ||
            u.Email.Equals("nawaroodinmeesri@gmail.com", StringComparison.OrdinalIgnoreCase) ||
            (u.Role != null && (u.Role.RoleCode == "ADMIN" || 
                                (u.Role.RoleName != null && (u.Role.RoleName.ToUpper().Contains("ADMIN") || u.Role.RoleName.Contains("ผู้ดูแลระบบ"))))));

        int staffUsers = allUsers.Count(u =>
            u.Email.Equals("reception@jojo.com", StringComparison.OrdinalIgnoreCase) ||
            u.Email.Equals("accountant@jojo.com", StringComparison.OrdinalIgnoreCase) ||
            u.Email.Equals("support@jojo.com", StringComparison.OrdinalIgnoreCase) ||
            (u.Role != null && (u.Role.RoleCode is "STAFF" or "RECEPTION" or "ACCOUNTANT" or "SUPPORT" ||
                                (u.Role.RoleName != null && (u.Role.RoleName.Contains("พนักงาน") || 
                                                             u.Role.RoleName.Contains("ต้อนรับ") || 
                                                             u.Role.RoleName.Contains("บัญชี") || 
                                                             u.Role.RoleName.Contains("บริการ"))))));

        int renterUsers = totalUsers - adminCount - staffUsers;
        if (renterUsers < 0) renterUsers = 0;

        // 3. สถิติการจองจริง
        var allBookings = await _db.Bookings
            .Include(b => b.User)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
                    .ThenInclude(r => r.RoomType)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        int totalBookings = allBookings.Count;
        int confirmedBookings = allBookings.Count(b => (b.Status ?? "").ToLower() is "confirmed" or "checked_in" or "checkedin" or "completed");
        int pendingBookings = allBookings.Count(b => (b.Status ?? "").ToLower() == "pending");
        int cancelledBookings = allBookings.Count(b => (b.Status ?? "").ToLower() == "cancelled");
        decimal totalDeposit = allBookings.Where(b => (b.Status ?? "").ToLower() != "cancelled").Sum(b => b.DepositAmount);

        // 4. รายการจองล่าสุด (5 รายการจริง)
        var recentBookings = allBookings.Take(5).Select(b =>
        {
            var roomNames = b.BookingRooms
                .Select(br => $"{br.Room?.RoomType?.TypeName ?? "Suite"} (ห้อง {br.Room?.RoomNumber ?? "-"})")
                .ToList();

            string roomSummary = roomNames.Any() ? string.Join(", ", roomNames) : "ห้องพักมาตรฐาน";

            return new AdminBookingItemViewModel
            {
                BookingId = b.BookingId,
                BookingCode = b.BookingCode,
                GuestName = b.User != null ? $"{b.User.FirstName} {b.User.LastName}".Trim() : "ลูกค้า",
                GuestEmail = b.User?.Email ?? "-",
                GuestPhone = b.User?.Phone ?? "-",
                RoomSummary = roomSummary,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Nights = b.Nights,
                TotalGuests = b.TotalGuests,
                SubtotalAmount = b.SubtotalAmount,
                DiscountAmount = b.DiscountAmount,
                NetAmount = b.NetAmount,
                DepositAmount = b.DepositAmount,
                DepositPercent = b.DepositPercent,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            };
        }).ToList();

        var totalPromotions = await _db.Promotions.CountAsync();
        ViewBag.OpenCustomerServiceTickets = await _db.Tickets.CountAsync(t => t.Status != "resolved" && t.Status != "closed");
        ViewBag.EscalatedCustomerServiceTickets = await _db.Tickets.CountAsync(t => t.Status == "escalated");

        var model = new AdminDashboardViewModel
        {
            TotalRooms = totalRooms,
            AvailableRooms = availableRooms,
            OccupiedRooms = occupiedRooms,
            MaintenanceRooms = maintenanceRooms,
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            AdminCount = adminCount,
            StaffUsers = staffUsers,
            RenterUsers = renterUsers,
            TotalBookings = totalBookings,
            ConfirmedBookings = confirmedBookings,
            PendingBookings = pendingBookings,
            CancelledBookings = cancelledBookings,
            TotalDepositAmount = totalDeposit,
            TotalPromotions = totalPromotions,
            RecentBookings = recentBookings
        };

        // สำหรับ ViewBags รองรับโค้ดเก่า
        ViewBag.TotalRooms = totalRooms;
        ViewBag.AvailableRooms = availableRooms;
        ViewBag.OccupiedRooms = occupiedRooms;
        ViewBag.MaintenanceRooms = maintenanceRooms;
        ViewBag.OccupancyRate = model.OccupancyRate;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.ActiveUsers = activeUsers;
        ViewBag.AdminCount = adminCount;
        ViewBag.StaffUsers = staffUsers;
        ViewBag.RenterUsers = renterUsers;
        ViewBag.TotalBookings = totalBookings;
        ViewBag.TotalDeposit = totalDeposit;
        ViewBag.TotalPromotions = totalPromotions;

        return View(model);
    }

    // ==========================================
    // 2. จัดการห้องพัก (Rooms) - แสดงและปรับสถานะห้องพักจริง
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Rooms(string? search = null, string? statusFilter = null)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        var query = _db.Rooms
            .Include(r => r.RoomType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            query = query.Where(r => r.RoomNumber.ToLower().Contains(s) ||
                                     (r.RoomType != null && r.RoomType.TypeName.ToLower().Contains(s)));
        }

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            string sf = statusFilter.Trim().ToLower();
            query = query.Where(r => r.Status.ToLower() == sf || 
                                     (sf == "reserved" && r.Status.ToLower() == "booked") ||
                                     (sf == "booked" && r.Status.ToLower() == "reserved"));
        }

        var roomEntities = await query.OrderBy(r => r.RoomNumber).ToListAsync();

        var rooms = roomEntities.Select(r => new AdminRoomItemViewModel
        {
            RoomId = r.RoomId,
            RoomNumber = r.RoomNumber,
            FloorNo = r.FloorNo,
            Status = r.Status,
            PricePerNight = r.PricePerNight ?? r.RoomType?.BasePrice ?? 3500m,
            RoomTypeId = r.RoomTypeId,
            TypeName = r.RoomType?.TypeName ?? "ห้องพัก",
            BedType = r.RoomType?.BedType ?? "King Bed",
            SizeSqm = r.RoomType?.SizeSqm ?? 45m,
            MaxGuests = r.RoomType?.MaxGuests ?? 2,
            IsActive = r.IsActive ?? true
        }).ToList();

        var vm = new AdminRoomListViewModel
        {
            Rooms = rooms,
            Search = search,
            StatusFilter = statusFilter
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoomStatus(uint roomId, string status)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        var room = await _db.Rooms.FindAsync(roomId);
        if (room == null)
        {
            TempData["ErrorMessage"] = "ไม่พบห้องพักที่ระบุ";
            return RedirectToAction(nameof(Rooms));
        }

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

        TempData["SuccessMessage"] = $"อัปเดตสถานะห้องพัก #{room.RoomNumber} เป็น \"{status}\" เรียบร้อยแล้ว";
        return RedirectToAction(nameof(Rooms));
    }

    // ==========================================
    // 3. จัดการโปรโมชั่น (Promotions) - แสดงและเปิด/ปิดโปรโมชั่นจริง
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Promotions(string? search = null, string? statusFilter = null)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        var query = _db.Promotions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            query = query.Where(p => p.PromoCode.ToLower().Contains(s) || p.PromoName.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            bool isActive = statusFilter == "Active";
            query = query.Where(p => (p.IsActive ?? false) == isActive);
        }

        var promoEntities = await query.OrderBy(p => p.PromotionId).ToListAsync();

        // PromotionId is not populated by the legacy booking flow. Count redeemed
        // offers from the same booking facts used by BookingPricingService instead.
        var usageBookings = await _db.Bookings
            .Select(b => new
            {
                b.BookingId,
                b.UserId,
                b.PromotionId,
                b.CheckInDate,
                b.Nights,
                b.TotalGuests,
                b.SubtotalAmount,
                b.ExtraBedAmount,
                b.CreatedAt,
                b.Status
            })
            .ToListAsync();
        var redeemedBookings = usageBookings
            .Where(b => !string.Equals(b.Status, "cancelled", StringComparison.OrdinalIgnoreCase))
            .ToList();
        var firstBookingIds = usageBookings
            .GroupBy(b => b.UserId)
            .Select(g => g.OrderBy(b => b.CreatedAt).ThenBy(b => b.BookingId).First().BookingId)
            .ToHashSet();

        var promoUsageCounts = promoEntities.ToDictionary(
            p => p.PromotionId,
            p => redeemedBookings.Count(b =>
            {
                if (b.PromotionId == p.PromotionId) return true;

                return (p.ConditionType ?? string.Empty).ToLowerInvariant() switch
                {
                    "first_booking" => firstBookingIds.Contains(b.BookingId),
                    "min_nights" => b.Nights >= p.ConditionValue,
                    "min_amount" => b.SubtotalAmount + b.ExtraBedAmount > p.ConditionValue,
                    "min_guests" => b.TotalGuests >= p.ConditionValue,
                    "advance_days" => (b.CheckInDate.ToDateTime(TimeOnly.MinValue).Date - b.CreatedAt.Date).TotalDays >= (double)p.ConditionValue,
                    _ => false
                };
            }));

        var list = promoEntities.Select(p =>
        {
            var clean = GetCleanPromotionInfo(p.PromoCode, p.PromoName, p.Description);
            return new AdminPromotionItemViewModel
            {
                PromotionId = p.PromotionId,
                PromoCode = p.PromoCode,
                PromoName = clean.Name,
                Description = clean.Desc,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                ConditionType = p.ConditionType,
                ConditionValue = p.ConditionValue,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive ?? true,
                UsageCount = promoUsageCounts.GetValueOrDefault(p.PromotionId, 0)
            };
        }).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            list = list.Where(p => p.PromoCode.ToLower().Contains(s) ||
                                   p.CleanPromoName.ToLower().Contains(s) ||
                                   p.CleanDescription.ToLower().Contains(s)).ToList();
        }

        var vm = new AdminPromotionListViewModel
        {
            Promotions = list,
            Search = search,
            StatusFilter = statusFilter
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePromotionStatus(uint promotionId)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        var promo = await _db.Promotions.FindAsync(promotionId);
        if (promo == null)
        {
            TempData["ErrorMessage"] = "ไม่พบโปรโมชั่นที่ระบุ";
            return RedirectToAction(nameof(Promotions));
        }

        bool current = promo.IsActive ?? true;
        promo.IsActive = !current;
        promo.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        string statusText = promo.IsActive.Value ? "เปิดใช้งาน" : "ปิดการใช้งาน";
        TempData["SuccessMessage"] = $"เปลี่ยนสถานะโปรโมชั่น \"{promo.PromoName}\" เป็น \"{statusText}\" เรียบร้อยแล้ว";
        return RedirectToAction(nameof(Promotions));
    }

    // ==========================================
    // 4. รายการจอง (Bookings) - แสดงและปรับสถานะการจองจริง
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Bookings(string? search = null, string? statusFilter = null)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        var query = _db.Bookings
            .Include(b => b.User)
            .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
                    .ThenInclude(r => r.RoomType)
            .Include(b => b.Payments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            query = query.Where(b => b.BookingCode.ToLower().Contains(s) ||
                                     (b.User != null && (b.User.FirstName.ToLower().Contains(s) ||
                                                        b.User.LastName.ToLower().Contains(s) ||
                                                        b.User.Email.ToLower().Contains(s))));
        }

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            string sf = statusFilter.Trim().ToLower();
            query = query.Where(b => b.Status.ToLower() == sf);
        }

        var bookingEntities = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();

        var list = bookingEntities.Select(b =>
        {
            var roomNames = b.BookingRooms
                .Select(br => $"{br.Room?.RoomType?.TypeName ?? "Suite"} (ห้อง {br.Room?.RoomNumber ?? "-"})")
                .ToList();

            string roomSummary = roomNames.Any() ? string.Join(", ", roomNames) : "ห้องพัก";
            var lastPayment = b.Payments.OrderByDescending(p => p.PaymentId).FirstOrDefault();

            return new AdminBookingItemViewModel
            {
                BookingId = b.BookingId,
                BookingCode = b.BookingCode,
                GuestName = b.User != null ? $"{b.User.FirstName} {b.User.LastName}".Trim() : "ลูกค้า",
                GuestEmail = b.User?.Email ?? "-",
                GuestPhone = b.User?.Phone ?? "-",
                RoomSummary = roomSummary,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Nights = b.Nights,
                TotalGuests = b.TotalGuests,
                SubtotalAmount = b.SubtotalAmount,
                DiscountAmount = b.DiscountAmount,
                NetAmount = b.NetAmount,
                DepositAmount = b.DepositAmount,
                DepositPercent = b.DepositPercent,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                PaymentStatus = lastPayment?.Status switch
                {
                    "success" => "ชำระมัดจำแล้ว",
                    "pending" => "รอดำเนินการ",
                    "refunded" => "คืนเงินแล้ว",
                    _ => "ชำระแล้ว"
                }
            };
        }).ToList();

        var vm = new AdminBookingListViewModel
        {
            Bookings = list,
            Search = search,
            StatusFilter = statusFilter
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBookingStatus(uint bookingId, string status)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        var booking = await _db.Bookings.Include(b => b.Payments).FirstOrDefaultAsync(b => b.BookingId == bookingId);
        if (booking == null)
        {
            TempData["ErrorMessage"] = "ไม่พบรายการจองที่ระบุ";
            return RedirectToAction(nameof(Bookings));
        }

        string dbStatus = (status ?? "").Trim().ToLowerInvariant() switch
        {
            "confirmed" or "ยืนยัน" => "confirmed",
            "pending" or "รอดำเนินการ" => "pending",
            "checkedin" or "checked_in" or "เช็คอิน" => "checked_in",
            "checkedout" or "checked_out" or "เช็คเอาท์" => "checked_out",
            "completed" or "เสร็จสิ้น" => "completed",
            "cancelled" or "ยกเลิก" => "cancelled",
            "no_show" or "noshow" => "no_show",
            _ => "confirmed"
        };

        booking.Status = dbStatus;
        booking.UpdatedAt = DateTime.Now;

        if (dbStatus == "cancelled")
        {
            booking.CancelledAt = DateTime.Now;
            booking.RefundAmount = booking.DepositAmount;

            // บันทึกรายการคืนเงินค่ามัดจำ
            _db.Payments.Add(new Payment
            {
                BookingId = booking.BookingId,
                PaymentType = "refund",
                Amount = booking.DepositAmount,
                Status = "pending",
                Method = "bank_transfer",
                CreatedAt = DateTime.Now
            });
        }
        else if (dbStatus == "checked_in")
        {
            booking.CheckedInAt = DateTime.Now;
        }
        else if (dbStatus is "completed" or "checked_out")
        {
            booking.CheckedOutAt = DateTime.Now;
        }

        await _db.SaveChangesAsync();

        string actionNotice = dbStatus == "cancelled"
            ? $"ยกเลิกการจอง #{booking.BookingCode} เรียบร้อยแล้ว (สถานะ: คืนเงินค่ามัดจำ 5% จำนวน ฿{booking.DepositAmount:N0})"
            : $"อัปเดตสถานะการจอง #{booking.BookingCode} เป็น \"{status}\" เรียบร้อยแล้ว";

        TempData["SuccessMessage"] = actionNotice;
        return RedirectToAction(nameof(Bookings));
    }

    // ==========================================
    // 5. User Management (Users, AddUser, DeleteUser)
    // ==========================================
    public IActionResult Users(string? search = null, string? roleFilter = null)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        return RedirectToAction("Lab5_3", "Home");
    }

    public IActionResult UserList(string? search = null, string? roleFilter = null)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        return RedirectToAction("Lab5_3", "Home");
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        return View(new AddUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddUser(AddUserViewModel model)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_userService.GetByEmail(model.Email) != null)
        {
            ModelState.AddModelError("Email", "อีเมลนี้มีผู้ใช้งานแล้วในระบบ กรุณาใช้อีเมลอื่น");
            return View(model);
        }

        var newUser = new UserModel
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Role = model.Role,
            Status = model.Status,
            Password = model.Password,
            CreatedAt = DateTime.Now
        };

        bool created = _userService.Add(newUser);
        if (!created)
        {
            ModelState.AddModelError(string.Empty, "เกิดข้อผิดพลาด ไม่สามารถเพิ่มผู้ใช้ได้");
            return View(model);
        }

        TempData["SuccessMessage"] = $"เพิ่มผู้ใช้งาน \"{newUser.FullName}\" สำเร็จเรียบร้อยแล้ว";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteUser(int id)
    {
        var accessCheck = ValidateAdminAccess();
        if (accessCheck != null) return accessCheck;

        var user = _userService.GetById(id);
        if (user != null)
        {
            _userService.Delete(id);
            TempData["SuccessMessage"] = $"ลบผู้ใช้งาน \"{user.FullName}\" เรียบร้อยแล้ว";
        }
        else
        {
            TempData["ErrorMessage"] = "ไม่พบข้อมูลผู้ใช้ที่ต้องการลบ";
        }

        return RedirectToAction(nameof(Users));
    }

    // ==========================================
    // Helper: ตรวจสอบและเตรียมข้อมูลเริ่มต้นของโรงแรม (Real Data Guarantee)
    // ==========================================
    private void EnsureDefaultHotelData()
    {
        try
        {
            // ตรวจสอบและซ่อมแซมข้อมูลโปรโมชั่นในฐานข้อมูลที่มีตัวอักษรเพี้ยน (Mojibake Auto-Repair)
            var allPromos = _db.Promotions.ToList();
            bool promoUpdated = false;
            foreach (var p in allPromos)
            {
                if (ContainsMojibake(p.PromoName) || ContainsMojibake(p.Description) || (p.PromoName != null && p.PromoName.Contains('Ó')))
                {
                    var clean = GetCleanPromotionInfo(p.PromoCode, p.PromoName, p.Description);
                    p.PromoName = clean.Name;
                    p.Description = clean.Desc;
                    promoUpdated = true;
                }
            }
            if (promoUpdated)
            {
                _db.SaveChanges();
            }

            // 1. ตรวจสอบโปรโมชั่นทั้ง 5 เงื่อนไข
            if (!_db.Promotions.Any())
            {
                _db.Promotions.AddRange(
                    new Promotion
                    {
                        PromoCode = "NEWMEMBER10",
                        PromoName = "สมาชิกใหม่จองครั้งแรก",
                        Description = "ลด 10% สำหรับสมาชิกใหม่ที่ทำการจองครั้งแรก",
                        DiscountType = "percent",
                        DiscountValue = 10m,
                        ConditionType = "first_booking",
                        ConditionValue = 1m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        PromoCode = "STAY2NIGHTS15",
                        PromoName = "เข้าพัก 2 คืนขึ้นไป",
                        Description = "ลด 15% เมื่อจองและเข้าพักตั้งแต่ 2 คืนขึ้นไป",
                        DiscountType = "percent",
                        DiscountValue = 15m,
                        ConditionType = "min_nights",
                        ConditionValue = 2m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        PromoCode = "OVER5000",
                        PromoName = "ยอดจองเกิน 5,000 บาท",
                        Description = "ลดทันที 500 บาท เมื่อยอดค่าห้องพักรวมเกิน 5,000 บาท",
                        DiscountType = "fixed",
                        DiscountValue = 500m,
                        ConditionType = "min_amount",
                        ConditionValue = 5000m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        PromoCode = "GROUP4",
                        PromoName = "กลุ่ม 4 คนขึ้นไป",
                        Description = "ลด 20% เมื่อเข้าพักเป็นกลุ่มตั้งแต่ 4 คนขึ้นไป",
                        DiscountType = "percent",
                        DiscountValue = 20m,
                        ConditionType = "min_guests",
                        ConditionValue = 4m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        PromoCode = "ADVANCE30",
                        PromoName = "จองล่วงหน้า 30 วัน",
                        Description = "ลด 25% เมื่อจองล่วงหน้าอย่างน้อย 30 วันก่อนวันเช็คอิน",
                        DiscountType = "percent",
                        DiscountValue = 25m,
                        ConditionType = "advance_days",
                        ConditionValue = 30m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                _db.SaveChanges();
            }

            // 2. ตรวจสอบประเภทห้องพักและห้องพัก
            if (!_db.RoomTypes.Any())
            {
                var rt1 = new RoomType { TypeName = "Grand Deluxe Suite", BedType = "King Bed", MaxGuests = 2, MaxExtraBeds = 1, BasePrice = 3200m, SizeSqm = 45m, Description = "ห้องสวีทหรูหราพร้อมวิวเมือง กว้างขวางและสะดวกสบาย", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                var rt2 = new RoomType { TypeName = "Private Pool Villa", BedType = "King Bed", MaxGuests = 4, MaxExtraBeds = 2, BasePrice = 7500m, SizeSqm = 95m, Description = "พูลวิลล่าส่วนตัวพร้อมสระว่ายน้ำส่วนตัว บรรยากาศเงียบสงบ", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                var rt3 = new RoomType { TypeName = "Lanna Heritage Room", BedType = "Twin Bed", MaxGuests = 2, MaxExtraBeds = 1, BasePrice = 2800m, SizeSqm = 40m, Description = "ห้องพักสไตล์ล้านนาโมเดิร์น สัมผัสเอกลักษณ์เมืองเหนือ", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                var rt4 = new RoomType { TypeName = "Ocean Breeze Suite", BedType = "King Bed", MaxGuests = 3, MaxExtraBeds = 1, BasePrice = 4200m, SizeSqm = 55m, Description = "ห้องพักวิวทะเลสุดตระการตา สดชื่นด้วยสายลมธรรมชาติ", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                var rt5 = new RoomType { TypeName = "Seaside Deluxe Room", BedType = "King Bed", MaxGuests = 2, MaxExtraBeds = 1, BasePrice = 3500m, SizeSqm = 42m, Description = "ห้องพักริมทะเลหัวหิน ใกล้ชายหาด สิ่งอำนวยความสะดวกครบครัน", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                var rt6 = new RoomType { TypeName = "Tropical Pool Villa", BedType = "King Bed", MaxGuests = 4, MaxExtraBeds = 2, BasePrice = 8200m, SizeSqm = 110m, Description = "วิลล่าหรูเกาะสมุย ท่ามกลางสวนเขตร้อนและสระว่ายน้ำส่วนตัว", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };

                _db.RoomTypes.AddRange(rt1, rt2, rt3, rt4, rt5, rt6);
                _db.SaveChanges();

                _db.Rooms.AddRange(
                    new Room { RoomTypeId = rt1.RoomTypeId, RoomNumber = "101", FloorNo = 1, Status = "available", PricePerNight = 3200m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt1.RoomTypeId, RoomNumber = "102", FloorNo = 1, Status = "reserved", PricePerNight = 3200m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt2.RoomTypeId, RoomNumber = "201", FloorNo = 2, Status = "available", PricePerNight = 7500m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt2.RoomTypeId, RoomNumber = "202", FloorNo = 2, Status = "reserved", PricePerNight = 7500m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt3.RoomTypeId, RoomNumber = "301", FloorNo = 3, Status = "available", PricePerNight = 2800m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt3.RoomTypeId, RoomNumber = "302", FloorNo = 3, Status = "available", PricePerNight = 2800m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt4.RoomTypeId, RoomNumber = "401", FloorNo = 4, Status = "maintenance", PricePerNight = 4200m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt5.RoomTypeId, RoomNumber = "501", FloorNo = 5, Status = "available", PricePerNight = 3500m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new Room { RoomTypeId = rt6.RoomTypeId, RoomNumber = "601", FloorNo = 6, Status = "available", PricePerNight = 8200m, IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
                );
                _db.SaveChanges();
            }
        }
        catch
        {
            // ป้องกันข้อผิดพลาดกรณี seed ซ้ำหรือติด constraint
        }
    }

    private static bool ContainsMojibake(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        if (text.Any(c => c >= '\u0E01' && c <= '\u0E5B')) return false;
        return text.Any(c => (c >= 0x00A0 && c <= 0x00FF) || c == '╣' || c == '┤' || c == '¿' || c == 'ó' || c == 'Ó' || c == '©');
    }

    private static (string Name, string Desc) GetCleanPromotionInfo(string? code, string? currentName, string? currentDesc)
    {
        if (!string.IsNullOrWhiteSpace(currentName) && !ContainsMojibake(currentName) && !currentName.Contains('Ó') && !currentName.Contains('ó') && !currentName.Contains('©'))
        {
            string desc = (!string.IsNullOrWhiteSpace(currentDesc) && !ContainsMojibake(currentDesc) && !currentDesc.Contains('Ó')) ? currentDesc : "";
            return (currentName, desc);
        }

        string upper = (code ?? "").ToUpperInvariant().Trim();
        if (upper.Contains("NEW") || upper.Contains("MEMBER") || upper.Contains("10"))
        {
            return ("สมาชิกใหม่จองครั้งแรก", "ลด 10% สำหรับสมาชิกใหม่ที่ทำการจองห้องพักครั้งแรก");
        }
        if (upper.Contains("LONG") || upper.Contains("STAY") || upper.Contains("15") || upper.Contains("NIGHT"))
        {
            return ("เข้าพัก 2 คืนขึ้นไป", "ลด 15% เมื่อจองและเข้าพักตั้งแต่ 2 คืนขึ้นไป");
        }
        if (upper.Contains("500") || upper.Contains("BIG") || upper.Contains("OVER"))
        {
            return ("ยอดจองเกิน 5,000 บาท", "ลดทันที 500 บาท เมื่อยอดค่าห้องพักรวมเกิน 5,000 บาท");
        }
        if (upper.Contains("GROUP") || upper.Contains("20") || upper.Contains("4"))
        {
            return ("เข้าพักเป็นกลุ่ม 4 ท่านขึ้นไป", "ลด 20% เมื่อเข้าพักเป็นกลุ่มตั้งแต่ 4 ท่านขึ้นไป");
        }
        if (upper.Contains("EARLY") || upper.Contains("ADVANCE") || upper.Contains("25") || upper.Contains("30"))
        {
            return ("จองล่วงหน้า 30 วัน", "ลด 25% เมื่อจองล่วงหน้าอย่างน้อย 30 วันก่อนวันเช็คอิน");
        }

        return (currentName ?? code ?? "โปรโมชั่นพิเศษ", currentDesc ?? "");
    }
}
