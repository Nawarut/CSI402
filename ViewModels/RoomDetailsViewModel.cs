using System.ComponentModel.DataAnnotations;

namespace Nawarut_8250_L005.ViewModels
{
    // ==========================================
    // ViewModel สำหรับแสดงรายละเอียดห้องพัก 1 ห้อง
    // ==========================================
    public class RoomDetailItemViewModel
    {
        public uint RoomId { get; set; }
        public string RoomNumber { get; set; } = "";
        public byte? FloorNo { get; set; }
        public string Status { get; set; } = "available";
        public decimal PricePerNight { get; set; }

        // ข้อมูลจาก RoomType
        public ushort RoomTypeId { get; set; }
        public string TypeName { get; set; } = "";
        public string? Description { get; set; }
        public string BedType { get; set; } = "";
        public decimal? SizeSqm { get; set; }
        public byte MaxGuests { get; set; }
        public byte MaxExtraBeds { get; set; }
        public decimal BasePrice { get; set; }

        // รายการ Amenities
        public List<AmenityItemViewModel> Amenities { get; set; } = new();

        // รูปภาพ
        public List<RoomImageItemViewModel> Images { get; set; } = new();

        // สถานะว่างหรือไม่
        public bool IsAvailable => Status == "available";
    }

    public class AmenityItemViewModel
    {
        public ushort AmenityId { get; set; }
        public string Name { get; set; } = "";
        public string? Icon { get; set; }
    }

    public class RoomImageItemViewModel
    {
        public string ImageUrl { get; set; } = "";
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
    }

    // ==========================================
    // ViewModel สำหรับ Cart (ห้องที่เลือกไว้)
    // ==========================================
    public class RoomCartItemViewModel
    {
        public uint RoomId { get; set; }
        public string RoomNumber { get; set; } = "";
        public string TypeName { get; set; } = "";
        public string BedType { get; set; } = "";
        public decimal PricePerNight { get; set; }
        public decimal ExtraBedPricePerNight { get; set; } = 500m;

        // ตัวเลือกที่ผู้ใช้กรอก
        public int Guests { get; set; } = 1;
        public int ExtraBeds { get; set; } = 0;

        // คำนวณ
        public int Nights { get; set; } = 1;
        public decimal LineTotal => (PricePerNight + ExtraBeds * ExtraBedPricePerNight) * Nights;
    }

    // ==========================================
    // ViewModel หลักสำหรับหน้า Room Details
    // ==========================================
    public class RoomDetailsPageViewModel
    {
        // ข้อมูลห้องที่กำลังดูรายละเอียด
        public RoomDetailItemViewModel Room { get; set; } = new();

        // ห้องอื่นในประเภทเดียวกัน (ให้เลือกเพิ่ม)
        public List<RoomDetailItemViewModel> RelatedRooms { get; set; } = new();

        // ตะกร้าห้องพักที่เลือกไว้ในการจองครั้งนี้
        public List<RoomCartItemViewModel> Cart { get; set; } = new();

        // ข้อมูลการค้นหาจากหน้า Home
        public string CheckIn { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
        public string CheckOut { get; set; } = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        public int Adults { get; set; } = 2;
        public int Children { get; set; } = 0;

        // ราคาค่าเตียงเสริมต่อคืน
        public decimal ExtraBedPricePerNight { get; set; } = 500m;

        // สรุปยอด
        public int TotalNights
        {
            get
            {
                if (DateTime.TryParse(CheckIn, out var ci) && DateTime.TryParse(CheckOut, out var co))
                    return Math.Max(1, (int)(co - ci).TotalDays);
                return 1;
            }
        }

        public decimal CartSubtotal => Cart.Sum(c => c.LineTotal);

        // ผลลัพธ์การคำนวณราคา ส่วนลดโปรโมชั่น และค่ามัดจำ
        public Nawarut_8250_L005.Services.BookingPricingResult Pricing { get; set; } = new();
    }

    // ==========================================
    // ViewModel สำหรับ Form เพิ่มห้องลงตะกร้า
    // ==========================================
    public class AddRoomToCartViewModel
    {
        [Required]
        public uint RoomId { get; set; }

        [Range(1, 20, ErrorMessage = "จำนวนผู้เข้าพักต้องอยู่ระหว่าง 1-20 ท่าน")]
        public int Guests { get; set; } = 1;

        [Range(0, 5, ErrorMessage = "จำนวนเตียงเสริมต้องอยู่ระหว่าง 0-5")]
        public int ExtraBeds { get; set; } = 0;

        public string CheckIn { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
        public string CheckOut { get; set; } = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
    }
}

