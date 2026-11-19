using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using Nawarut_8250_L005.Services;
using Nawarut_8250_L005.ViewModels;
using System.Text.Json;

namespace Nawarut_8250_L005.Controllers;

public class RoomController : Controller
{
    private readonly Csi402dbContext _db;
    private readonly IBookingPricingService _pricingService;
    private const string CartSessionKey = "RoomCart";
    private const decimal ExtraBedPrice = 500m; // ราคาเตียงเสริมต่อคืน

    public RoomController(Csi402dbContext db, IBookingPricingService pricingService)
    {
        _db = db;
        _pricingService = pricingService;
    }

    // ==========================================
    // GET: /Room/Details/{id}
    // หน้ารายละเอียดห้องพัก
    // ==========================================
    [HttpGet]
    public IActionResult Details(uint id, string? checkIn, string? checkOut, int adults = 2, int children = 0)
    {
        // ดึงข้อมูลห้องพร้อม RoomType, Amenities, Images
        string idStr = id.ToString();
        var room = (from r in _db.Rooms
                        .Include(r => r.RoomType)
                            .ThenInclude(rt => rt.Amenities)
                        .Include(r => r.RoomType)
                            .ThenInclude(rt => rt.RoomImages)
                    where (r.RoomId == id || r.RoomNumber == idStr) && r.IsActive == true
                    select r).FirstOrDefault();

        if (room == null)
        {
            room = _db.Rooms
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Amenities)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.RoomImages)
                .FirstOrDefault(r => r.IsActive == true);
        }

        if (room == null)
        {
            TempData["Error"] = "ไม่พบห้องพักที่ต้องการ";
            return RedirectToAction("Index", "Home");
        }

        // ดึงห้องอื่นในประเภทเดียวกันที่ยังว่าง
        var relatedRooms = (from r in _db.Rooms
                                .Include(r => r.RoomType)
                                    .ThenInclude(rt => rt.RoomImages)
                            where r.RoomTypeId == room.RoomTypeId
                               && r.RoomId != id
                               && r.IsActive == true
                            select r).Take(6).ToList();

        // โหลด Cart จาก Session
        var cart = GetCartFromSession();

        // กำหนดวันที่ default
        var ciStr = checkIn ?? DateTime.Today.ToString("yyyy-MM-dd");
        var coStr = checkOut ?? DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        if (!DateTime.TryParse(ciStr, out _)) ciStr = DateTime.Today.ToString("yyyy-MM-dd");
        if (!DateTime.TryParse(coStr, out _)) coStr = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

        int nights = 1;
        DateTime ciDate = DateTime.Today.AddDays(1);
        if (DateTime.TryParse(ciStr, out var ci2) && DateTime.TryParse(coStr, out var co2))
        {
            nights = Math.Max(1, (int)(co2 - ci2).TotalDays);
            ciDate = ci2;
        }

        // อัปเดต Nights ใน Cart
        foreach (var item in cart) item.Nights = nights;

        // ตรวจสอบว่าผู้ใช้ปัจจุบันเป็นสมาชิกใหม่จองครั้งแรกหรือไม่
        var userId = HttpContext.Session.GetInt32("UserId");
        bool isNewMember = true;
        if (userId.HasValue)
        {
            try
            {
                isNewMember = !_db.Bookings.Any(b => b.UserId == (uint)userId.Value);
            }
            catch
            {
                isNewMember = true;
            }
        }

        // คำนวณราคา ส่วนลดโปรโมชั่น และเงินมัดจำ 5%
        var roomPrice = room.PricePerNight ?? room.RoomType.BasePrice;
        var pricing = _pricingService.Calculate(
            roomPricePerNight: roomPrice,
            nights: nights,
            extraBeds: 0,
            extraBedPricePerNight: ExtraBedPrice,
            totalGuests: adults,
            checkInDate: ciDate,
            bookingDate: DateTime.Today,
            isNewMember: isNewMember
        );

        var vm = new RoomDetailsPageViewModel
        {
            Room = MapRoomToViewModel(room),
            RelatedRooms = relatedRooms.Select(MapRoomToViewModel).ToList(),
            Cart = cart,
            CheckIn = ciStr,
            CheckOut = coStr,
            Adults = adults,
            Children = children,
            ExtraBedPricePerNight = ExtraBedPrice,
            Pricing = pricing
        };

        return View(vm);
    }

    // ==========================================
    // POST: /Room/AddToCart
    // เพิ่มห้องลงตะกร้า (AJAX หรือ Form POST)
    // ==========================================
    [HttpPost]
    public IActionResult AddToCart(AddRoomToCartViewModel form)
    {
        if (!TryGetStayDates(form.CheckIn, form.CheckOut, out var stayCheckIn, out var stayCheckOut))
        {
            TempData["Error"] = "กรุณาเลือกวันเช็กอินและเช็กเอาท์ให้ถูกต้อง";
            return RedirectToAction("Details", new { id = form.RoomId });
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง";
            return RedirectToAction("Details", new { id = form.RoomId, checkIn = form.CheckIn, checkOut = form.CheckOut });
        }

        // ดึงข้อมูลห้อง
        var room = (from r in _db.Rooms.Include(r => r.RoomType)
                    where r.RoomId == form.RoomId && r.IsActive == true
                    select r).FirstOrDefault();

        if (room == null)
        {
            TempData["Error"] = "ไม่พบห้องพักในระบบ";
            return RedirectToAction("Index", "Home");
        }

        // ตรวจสอบ MaxGuests
        if (form.Guests > room.RoomType.MaxGuests)
        {
            TempData["Error"] = $"ห้อง {room.RoomNumber} รองรับผู้เข้าพักสูงสุด {room.RoomType.MaxGuests} ท่าน";
            return RedirectToAction("Details", new { id = form.RoomId, checkIn = form.CheckIn, checkOut = form.CheckOut });
        }

        // ตรวจสอบ MaxExtraBeds
        if (form.ExtraBeds > room.RoomType.MaxExtraBeds)
        {
            TempData["Error"] = $"ห้อง {room.RoomNumber} รองรับเตียงเสริมสูงสุด {room.RoomType.MaxExtraBeds} เตียง";
            return RedirectToAction("Details", new { id = form.RoomId, checkIn = form.CheckIn, checkOut = form.CheckOut });
        }

        int nights = 1;
        nights = (int)(stayCheckOut - stayCheckIn).TotalDays;

        // โหลด Cart และเพิ่ม/อัปเดตห้อง
        var cart = GetCartFromSession();
        var existing = cart.FirstOrDefault(c => c.RoomId == form.RoomId);
        if (existing != null)
        {
            // อัปเดตตัวเลือก
            existing.Guests = form.Guests;
            existing.ExtraBeds = form.ExtraBeds;
            existing.Nights = nights;
            TempData["SuccessMessage"] = $"อัปเดตห้อง {room.RoomNumber} ในรายการจองแล้ว";
        }
        else
        {
            cart.Add(new RoomCartItemViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                TypeName = room.RoomType.TypeName,
                BedType = room.RoomType.BedType,
                PricePerNight = room.PricePerNight ?? room.RoomType.BasePrice,
                ExtraBedPricePerNight = ExtraBedPrice,
                Guests = form.Guests,
                ExtraBeds = form.ExtraBeds,
                Nights = nights
            });
            TempData["SuccessMessage"] = $"เพิ่มห้อง {room.RoomNumber} ({room.RoomType.TypeName}) ลงในรายการจองแล้ว";
        }

        SaveCartToSession(cart);
        return RedirectToAction("Details", new { id = form.RoomId, checkIn = form.CheckIn, checkOut = form.CheckOut });
    }

    // ==========================================
    // POST: /Room/RemoveFromCart/{roomId}
    // ลบห้องออกจากตะกร้า
    // ==========================================
    [HttpPost]
    public IActionResult RemoveFromCart(uint roomId, string? checkIn, string? checkOut)
    {
        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(c => c.RoomId == roomId);
        if (item != null)
        {
            cart.Remove(item);
            SaveCartToSession(cart);
            TempData["SuccessMessage"] = $"ลบห้อง {item.RoomNumber} ออกจากรายการจองแล้ว";
        }

        // ถ้า Cart ว่าง ให้กลับหน้า Home
        if (!cart.Any())
            return RedirectToAction("Index", "Home");

        // กลับไปดูห้องแรกใน Cart
        return RedirectToAction("Details", new
        {
            id = cart.FirstOrDefault()?.RoomId ?? roomId,
            checkIn,
            checkOut
        });
    }

    // ==========================================
    // POST: /Room/ClearCart
    // ล้างตะกร้าทั้งหมด
    // ==========================================
    [HttpPost]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove(CartSessionKey);
        TempData["SuccessMessage"] = "ล้างรายการห้องพักทั้งหมดแล้ว";
        return RedirectToAction("Index", "Home");
    }

    // ==========================================
    // GET: /Room/Checkout
    // สรุปการจองก่อนยืนยัน พร้อมคำนวณโปรโมชั่นและมัดจำ 5%
    // ==========================================
    [HttpGet]
    public IActionResult Checkout(string? checkIn, string? checkOut, int adults = 2, int children = 0)
    {
        // ตรวจสอบว่า Login แล้วหรือยัง
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["Error"] = "กรุณาเข้าสู่ระบบก่อนทำการจองห้องพัก";
            return RedirectToAction("Login", "Account",
                new { returnUrl = Url.Action("Checkout", "Room", new { checkIn, checkOut, adults, children }) });
        }

        var cart = GetCartFromSession();
        if (!cart.Any())
        {
            TempData["Error"] = "ยังไม่มีห้องพักในรายการจอง กรุณาเลือกห้องก่อน";
            return RedirectToAction("Index", "Home");
        }

        var ciStr = checkIn ?? DateTime.Today.ToString("yyyy-MM-dd");
        var coStr = checkOut ?? DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        if (!TryGetStayDates(ciStr, coStr, out var ciDate, out var coDate))
        {
            TempData["Error"] = "วันเช็กเอาท์ต้องอยู่หลังวันเช็กอิน และวันเช็กอินต้องไม่อยู่ในอดีต";
            return RedirectToAction("Details", new { id = cart.First().RoomId });
        }
        int nights = (int)(coDate - ciDate).TotalDays;

        foreach (var item in cart) item.Nights = nights;

        // ตรวจสอบสถานะสมาชิกใหม่
        bool isNewMember = true;
        try
        {
            isNewMember = !_db.Bookings.Any(b => b.UserId == (uint)userId.Value);
        }
        catch
        {
            isNewMember = true;
        }

        // คำนวณราคา ส่วนลดโปรโมชั่น 5 เงื่อนไข และค่ามัดจำ 5%
        var roomInputs = cart.Select(c => (c.PricePerNight, c.ExtraBeds, c.Guests));
        var pricing = _pricingService.CalculateMultiRoom(
            rooms: roomInputs,
            nights: nights,
            extraBedPricePerNight: ExtraBedPrice,
            checkInDate: ciDate,
            bookingDate: DateTime.Today,
            isNewMember: isNewMember
        );

        var vm = new RoomDetailsPageViewModel
        {
            Cart = cart,
            CheckIn = ciStr,
            CheckOut = coStr,
            Adults = adults,
            Children = children,
            ExtraBedPricePerNight = ExtraBedPrice,
            Pricing = pricing
        };

        return View(vm);
    }

    // ==========================================
    // POST: /Room/ConfirmBooking
    // ยืนยันการจอง บันทึกลงฐานข้อมูลและคำนวณมัดจำ 5%
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmBooking(string checkIn, string checkOut, int adults = 2, int children = 0)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["Error"] = "เซสชันหมดอายุ กรุณาเข้าสู่ระบบใหม่อีกครั้ง";
            return RedirectToAction("Login", "Account");
        }

        var cart = GetCartFromSession();
        if (!cart.Any())
        {
            TempData["Error"] = "ไม่พบรายการห้องพักที่เลือก กรุณาเลือกห้องพักก่อนทำการจอง";
            return RedirectToAction("Index", "Home");
        }

        var ciStr = checkIn ?? DateTime.Today.ToString("yyyy-MM-dd");
        var coStr = checkOut ?? DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        if (!TryGetStayDates(ciStr, coStr, out var ciDate, out var coDate))
        {
            TempData["Error"] = "วันเช็กเอาท์ต้องอยู่หลังวันเช็กอิน และวันเช็กอินต้องไม่อยู่ในอดีต";
            return RedirectToAction("Checkout");
        }
        int nights = (int)(coDate - ciDate).TotalDays;

        // ตรวจสอบสถานะสมาชิกใหม่
        bool isNewMember = true;
        try
        {
            isNewMember = !_db.Bookings.Any(b => b.UserId == (uint)userId.Value);
        }
        catch
        {
            isNewMember = true;
        }

        // คำนวณราคาด้วย Pricing Service
        var roomInputs = cart.Select(c => (c.PricePerNight, c.ExtraBeds, c.Guests));
        var pricing = _pricingService.CalculateMultiRoom(
            rooms: roomInputs,
            nights: nights,
            extraBedPricePerNight: ExtraBedPrice,
            checkInDate: ciDate,
            bookingDate: DateTime.Today,
            isNewMember: isNewMember
        );

        string bookingCode = $"BK{DateTime.Now:yyyyMMddHHmmss}";
        uint createdBookingId = 0;

        try
        {
            var booking = new Booking
            {
                BookingCode = bookingCode,
                UserId = (uint)userId.Value,
                CheckInDate = DateOnly.FromDateTime(ciDate),
                CheckOutDate = DateOnly.FromDateTime(coDate),
                Nights = (ushort)nights,
                TotalGuests = (ushort)pricing.TotalGuests,
                Status = "pending",
                SubtotalAmount = pricing.RoomSubtotal,
                ExtraBedAmount = pricing.ExtraBedTotal,
                DiscountAmount = pricing.TotalDiscountAmount,
                NetAmount = pricing.NetAmount,
                DepositPercent = 5.0m,
                DepositAmount = pricing.DepositAmount,
                DepositDueAt = DateTime.Now.AddHours(24)
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();
            createdBookingId = booking.BookingId;

            // เพิ่มรายการห้องใน BookingRoom
            foreach (var item in cart)
            {
                var br = new BookingRoom
                {
                    BookingId = booking.BookingId,
                    RoomId = item.RoomId,
                    CheckInDate = DateOnly.FromDateTime(ciDate),
                    CheckOutDate = DateOnly.FromDateTime(coDate),
                    PricePerNight = item.PricePerNight,
                    Guests = (byte)item.Guests,
                    ExtraBeds = (byte)item.ExtraBeds,
                    ExtraBedPrice = ExtraBedPrice,
                    LineTotal = item.LineTotal
                };
                _db.BookingRooms.Add(br);

                var r = _db.Rooms.Find(item.RoomId);
                if (r != null)
                {
                    r.Status = "reserved";
                    r.UpdatedAt = DateTime.Now;
                }
            }

            _db.SaveChanges();
        }
        catch
        {
            TempData["ErrorMessage"] = "บันทึกรายการจองไม่สำเร็จ กรุณาลองใหม่อีกครั้ง";
            return RedirectToAction("Checkout", new { checkIn = ciStr, checkOut = coStr, adults, children });
        }

        if (createdBookingId == 0)
        {
            TempData["ErrorMessage"] = "บันทึกรายการจองไม่สำเร็จ กรุณาลองใหม่อีกครั้ง";
            return RedirectToAction("Checkout", new { checkIn = ciStr, checkOut = coStr, adults, children });
        }

        // ล้างตะกร้าห้องพัก
        HttpContext.Session.Remove(CartSessionKey);

        return RedirectToAction("Checkout", "Payments", new { bookingId = createdBookingId, type = "deposit" });
    }

    // ==========================================
    // Helper Methods
    // ==========================================
    private static bool ContainsMojibake(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        // If it contains valid Thai characters, it's valid Thai!
        if (text.Any(c => c >= '\u0E01' && c <= '\u0E5B')) return false;
        // If it contains characters from CP874 misinterpreted as Latin-1
        return text.Any(c => (c >= 0x00A0 && c <= 0x00FF) || c == '╣' || c == '┤' || c == '¿' || c == 'ó' || c == 'Ó' || c == '©');
    }

    private RoomDetailItemViewModel MapRoomToViewModel(Room room)
    {
        string typeName = room.RoomType?.TypeName ?? "Deluxe Suite";
        string? desc = room.RoomType?.Description;
        if (string.IsNullOrWhiteSpace(desc) || ContainsMojibake(desc))
        {
            desc = CleanDescription(typeName);
        }

        var amenities = (room.RoomType?.Amenities ?? new List<Amenity>())
            .Select(a => new AmenityItemViewModel
            {
                AmenityId = a.AmenityId,
                Name = CleanAmenityName(a.AmenityId, a.AmenityName),
                Icon = a.Icon
            }).ToList();

        if (!amenities.Any())
        {
            amenities = GetDefaultAmenities();
        }

        return new RoomDetailItemViewModel
        {
            RoomId = room.RoomId,
            RoomNumber = room.RoomNumber,
            FloorNo = room.FloorNo,
            Status = room.Status,
            PricePerNight = room.PricePerNight ?? room.RoomType?.BasePrice ?? 3500m,
            RoomTypeId = room.RoomTypeId,
            TypeName = typeName,
            Description = desc,
            BedType = room.RoomType?.BedType ?? "King Bed",
            SizeSqm = room.RoomType?.SizeSqm ?? 45,
            MaxGuests = room.RoomType?.MaxGuests ?? 2,
            MaxExtraBeds = room.RoomType?.MaxExtraBeds ?? 1,
            BasePrice = room.RoomType?.BasePrice ?? 3500m,
            Amenities = amenities,
            Images = (room.RoomType?.RoomImages ?? new List<RoomImage>()).Select(i => new RoomImageItemViewModel
            {
                ImageUrl = i.ImageUrl,
                Caption = i.Caption,
                IsCover = i.IsCover
            }).ToList()
        };
    }

    private static string CleanDescription(string typeName)
    {
        if (typeName.Contains("Pool Villa", StringComparison.OrdinalIgnoreCase))
            return "วิลลาส่วนตัวสุดหรูพร้อมสระว่ายน้ำส่วนตัว รายล้อมด้วยธรรมชาติอันเงียบสงบ วิวทะเลและแมกไม้อันงดงาม พร้อมบริการอาหารเช้ากูร์เมต์และความเป็นส่วนตัวสูงสุดเพื่อการพักผ่อนอย่างแท้จริง";
        if (typeName.Contains("Lanna", StringComparison.OrdinalIgnoreCase))
            return "ห้องพักเอกลักษณ์ล้านนาโมเดิร์น โอบล้อมด้วยธรรมชาติ อากาศบริสุทธิ์ ระเบียงชมวิวสวนสวย และสถาปัตยกรรมไม้สักอันทรงคุณค่า";
        if (typeName.Contains("Ocean", StringComparison.OrdinalIgnoreCase) || typeName.Contains("Beach", StringComparison.OrdinalIgnoreCase) || typeName.Contains("Seaside", StringComparison.OrdinalIgnoreCase))
            return "ห้องพักวิวทะเลสุดตระการตา สัมผัสสายลมทะเลบริสุทธิ์และพระอาทิตย์ตกดินอันงดงาม เตียงนอนคุณภาพสูง พร้อมสิ่งอำนวยความสะดวกครบครัน";
        if (typeName.Contains("Suite", StringComparison.OrdinalIgnoreCase))
            return "ห้องสวีทหรูหรากว้างขวาง ตกแต่งอย่างประณีตพร้อมเตียงคิงไซส์ วิวเมืองมุมสูง อ่างอาบน้ำหรูหรา และพื้นที่พักผ่อนที่ตอบโจทย์ความสุขอย่างแท้จริง";
        return $"สัมผัสความสุขแห่งการพักผ่อนในห้อง {typeName} ที่ได้รับการออกแบบอย่างประณีต พร้อมสิ่งอำนวยความสะดวกครบครัน และการบริการระดับพรีเมียมของ JoJo Hotel";
    }

    private static string CleanAmenityName(ushort id, string? name)
    {
        if (!string.IsNullOrWhiteSpace(name) && !ContainsMojibake(name) && !name.Contains('ó') && !name.Contains('Ó') && !name.Contains('©'))
        {
            return name;
        }

        return id switch
        {
            1 => "Wi-Fi ความเร็วสูง",
            2 => "เครื่องปรับอากาศปรับอุณหภูมิ",
            3 => "สมาร์ททีวี 4K",
            4 => "ตู้เย็นและมินิบาร์",
            5 => "อ่างอาบน้ำหรูหรา",
            6 => "ไดร์เป่าผม",
            7 => "ตู้นิรภัยภายในห้อง",
            8 => "ชุดชงกาแฟและชาพรีเมียม",
            9 => "เครื่องทำน้ำอุ่น",
            10 => "ระเบียงส่วนตัว",
            11 => "สระว่ายน้ำส่วนตัว",
            12 => "อาหารเช้ากูร์เมต์",
            _ => (name != null && name.Contains("Wi-Fi", StringComparison.OrdinalIgnoreCase)) ? "Wi-Fi ความเร็วสูง" : "สิ่งอำนวยความสะดวกพรีเมียม"
        };
    }

    private static List<AmenityItemViewModel> GetDefaultAmenities()
    {
        return new List<AmenityItemViewModel>
        {
            new() { AmenityId = 1, Name = "Wi-Fi ความเร็วสูง" },
            new() { AmenityId = 2, Name = "เครื่องปรับอากาศปรับอุณหภูมิ" },
            new() { AmenityId = 3, Name = "สมาร์ททีวี 4K" },
            new() { AmenityId = 5, Name = "อ่างอาบน้ำหรูหรา" },
            new() { AmenityId = 8, Name = "ชุดชงกาแฟและชาพรีเมียม" },
            new() { AmenityId = 12, Name = "อาหารเช้ากูร์เมต์" }
        };
    }

    private List<RoomCartItemViewModel> GetCartFromSession()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json)) return new List<RoomCartItemViewModel>();
        try
        {
            return JsonSerializer.Deserialize<List<RoomCartItemViewModel>>(json) ?? new List<RoomCartItemViewModel>();
        }
        catch
        {
            return new List<RoomCartItemViewModel>();
        }
    }

    private void SaveCartToSession(List<RoomCartItemViewModel> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CartSessionKey, json);
    }

    private static bool TryGetStayDates(string? checkIn, string? checkOut, out DateTime ci, out DateTime co)
    {
        var hasCheckIn = DateTime.TryParse(checkIn, out var parsedCheckIn);
        var hasCheckOut = DateTime.TryParse(checkOut, out var parsedCheckOut);
        ci = hasCheckIn ? parsedCheckIn.Date : DateTime.MinValue;
        co = hasCheckOut ? parsedCheckOut.Date : DateTime.MinValue;
        var valid = hasCheckIn && hasCheckOut;
        return valid && ci >= DateTime.Today && co > ci;
    }
}
