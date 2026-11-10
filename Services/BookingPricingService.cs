using System;
using System.Collections.Generic;
using System.Linq;

namespace Nawarut_8250_L005.Services
{
    // ==========================================
    // รายละเอียดโปรโมชั่นแต่ละรายการ
    // ==========================================
    public class PromotionItemResult
    {
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ConditionText { get; set; } = "";
        public bool IsEligible { get; set; }
        public string EligibilityReason { get; set; } = "";
        public decimal DiscountRate { get; set; } // e.g. 0.10 for 10%
        public bool IsFixedAmount { get; set; } // true for flat 500 THB
        public decimal FixedAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsApplied { get; set; } = true;
    }

    // ==========================================
    // ผลลัพธ์การคำนวณราคา ส่วนลด และเงินมัดจำ
    // ==========================================
    public class BookingPricingResult
    {
        // ยอดตั้งต้น
        public decimal RoomSubtotal { get; set; }
        public decimal ExtraBedTotal { get; set; }
        public decimal SubtotalAmount => RoomSubtotal + ExtraBedTotal;

        // ข้อมูลการเข้าพัก
        public int Nights { get; set; }
        public int TotalGuests { get; set; }
        public int DaysInAdvance { get; set; }
        public bool IsNewMember { get; set; }

        // รายการโปรโมชั่นทั้ง 5 เงื่อนไข
        public List<PromotionItemResult> Promotions { get; set; } = new();

        // สรุปโปรโมชั่นที่เข้าเกณฑ์
        public List<PromotionItemResult> EligiblePromotions => Promotions.Where(p => p.IsEligible).ToList();

        // รวมส่วนลดทั้งหมด (รวมทุกโปรโมชั่นที่ผ่านเงื่อนไข โดยยอดลดไม่เกิน Subtotal)
        public decimal TotalDiscountAmount
        {
            get
            {
                var sum = Promotions.Where(p => p.IsEligible && p.IsApplied).Sum(p => p.DiscountAmount);
                return Math.Min(SubtotalAmount, Math.Round(sum, 2));
            }
        }

        // ส่วนลดที่ดีที่สุดรายการเดียว (กรณีเลือกใช้โปรเดี่ยวสูงสุด)
        public PromotionItemResult? BestPromotion => Promotions.Where(p => p.IsEligible).OrderByDescending(p => p.DiscountAmount).FirstOrDefault();

        // ยอดรวมสุทธิหลังหักส่วนลด (Net Total)
        public decimal NetAmount => Math.Max(0m, SubtotalAmount - TotalDiscountAmount);

        // ค่ามัดจำที่ต้องจ่ายล่วงหน้า 5% ของยอดสุทธิ
        public const decimal DepositRate = 0.05m; // 5%
        public decimal DepositPercent => DepositRate * 100m; // 5
        public decimal DepositAmount => Math.Round(NetAmount * DepositRate, 2);

        // ยอดคงเหลือชำระ ณ วันเข้าพัก (95%)
        public decimal RemainingAmount => NetAmount - DepositAmount;
    }

    public interface IBookingPricingService
    {
        BookingPricingResult Calculate(
            decimal roomPricePerNight,
            int nights,
            int extraBeds = 0,
            decimal extraBedPricePerNight = 500m,
            int totalGuests = 2,
            DateTime? checkInDate = null,
            DateTime? bookingDate = null,
            bool isNewMember = false);

        BookingPricingResult CalculateMultiRoom(
            IEnumerable<(decimal RoomPrice, int ExtraBeds, int Guests)> rooms,
            int nights,
            decimal extraBedPricePerNight = 500m,
            DateTime? checkInDate = null,
            DateTime? bookingDate = null,
            bool isNewMember = false);
    }

    public class BookingPricingService : IBookingPricingService
    {
        public BookingPricingResult Calculate(
            decimal roomPricePerNight,
            int nights,
            int extraBeds = 0,
            decimal extraBedPricePerNight = 500m,
            int totalGuests = 2,
            DateTime? checkInDate = null,
            DateTime? bookingDate = null,
            bool isNewMember = false)
        {
            return CalculateMultiRoom(
                new[] { (roomPricePerNight, extraBeds, totalGuests) },
                nights,
                extraBedPricePerNight,
                checkInDate,
                bookingDate,
                isNewMember);
        }

        public BookingPricingResult CalculateMultiRoom(
            IEnumerable<(decimal RoomPrice, int ExtraBeds, int Guests)> rooms,
            int nights,
            decimal extraBedPricePerNight = 500m,
            DateTime? checkInDate = null,
            DateTime? bookingDate = null,
            bool isNewMember = false)
        {
            var roomList = rooms?.ToList() ?? new List<(decimal, int, int)>();
            var validNights = Math.Max(1, nights);

            decimal roomTotal = roomList.Sum(r => r.RoomPrice * validNights);
            decimal extraBedTotal = roomList.Sum(r => r.ExtraBeds * extraBedPricePerNight * validNights);
            decimal subtotal = roomTotal + extraBedTotal;

            int totalGuests = roomList.Sum(r => r.Guests);
            if (totalGuests <= 0) totalGuests = 1;

            var today = (bookingDate ?? DateTime.Today).Date;
            var ci = (checkInDate ?? today.AddDays(1)).Date;
            int daysInAdvance = Math.Max(0, (int)(ci - today).TotalDays);

            var result = new BookingPricingResult
            {
                RoomSubtotal = roomTotal,
                ExtraBedTotal = extraBedTotal,
                Nights = validNights,
                TotalGuests = totalGuests,
                DaysInAdvance = daysInAdvance,
                IsNewMember = isNewMember,
                Promotions = new List<PromotionItemResult>()
            };

            // ==========================================
            // เงื่อนไขโปรโมชั่น 5 ข้อตามข้อกำหนด:
            // 1. สมาชิกใหม่จองครั้งแรก: ลด 10%
            // 2. เข้าพัก 2 คืนขึ้นไป: ลด 15%
            // 3. ยอดรวมเกิน 5,000 บาท: ลดทันที 500 บาท
            // 4. เข้าพักเป็นกลุ่มตั้งแต่ 4 คนขึ้นไป: ลด 20%
            // 5. จองล่วงหน้าอย่างน้อย 30 วันก่อนวันเช็คอิน: ลด 25%
            // ==========================================

            // 1. สมาชิกใหม่จองครั้งแรก: ลด 10%
            var promo1 = new PromotionItemResult
            {
                Code = "PROMO_NEWMEMBER10",
                Title = "สมาชิกใหม่จองครั้งแรก",
                ConditionText = "ลด 10%",
                Description = "สิทธิพิเศษต้อนรับสมาชิกใหม่สำหรับการจองครั้งแรก",
                DiscountRate = 0.10m,
                IsEligible = isNewMember,
                EligibilityReason = isNewMember
                    ? "คุณได้รับสิทธิ์สมาชิกใหม่สำหรับการจองครั้งแรก"
                    : "สำหรับสมาชิกใหม่ที่ยังไม่เคยมีประวัติการจอง",
                DiscountAmount = isNewMember ? Math.Round(subtotal * 0.10m, 2) : 0m,
                IsApplied = isNewMember
            };
            result.Promotions.Add(promo1);

            // 2. เข้าพัก 2 คืนขึ้นไป: ลด 15%
            bool eligibleNights = validNights >= 2;
            var promo2 = new PromotionItemResult
            {
                Code = "PROMO_STAY2NIGHTS15",
                Title = "เข้าพัก 2 คืนขึ้นไป",
                ConditionText = "ลด 15%",
                Description = "รับส่วนลด 15% เมื่อเข้าพักต่อเนื่องตั้งแต่ 2 คืนขึ้นไป",
                DiscountRate = 0.15m,
                IsEligible = eligibleNights,
                EligibilityReason = eligibleNights
                    ? $"คุณเข้าพัก {validNights} คืน (ตรงตามเงื่อนไข >= 2 คืน)"
                    : $"ปัจจุบันเข้าพัก {validNights} คืน (เพิ่มอีก 1 คืนเพื่อรับส่วนลด 15%)",
                DiscountAmount = eligibleNights ? Math.Round(subtotal * 0.15m, 2) : 0m,
                IsApplied = eligibleNights
            };
            result.Promotions.Add(promo2);

            // 3. ยอดรวมเกิน 5,000 บาท: ลดทันที 500 บาท
            bool eligibleAmount = subtotal > 5000m;
            decimal promo3Amount = eligibleAmount ? Math.Min(subtotal, 500m) : 0m;
            var promo3 = new PromotionItemResult
            {
                Code = "PROMO_OVER5000",
                Title = "ยอดรวมเกิน 5,000 บาท",
                ConditionText = "ลดทันที 500 บาท",
                Description = "รับส่วนลดทันที 500 บาทเมื่อมียอดจองรวมเกิน 5,000 บาท",
                IsFixedAmount = true,
                FixedAmount = 500m,
                IsEligible = eligibleAmount,
                EligibilityReason = eligibleAmount
                    ? $"ยอดรวม ฿{subtotal:N0} (เกิน 5,000 บาท ลดทันที 500 บาท)"
                    : $"ยอดรวม ฿{subtotal:N0} (ต้องการอีก ฿{(5001m - subtotal):N0} เพื่อรับสิทธิ์)",
                DiscountAmount = promo3Amount,
                IsApplied = eligibleAmount
            };
            result.Promotions.Add(promo3);

            // 4. เข้าพักเป็นกลุ่มตั้งแต่ 4 คนขึ้นไป: ลด 20%
            bool eligibleGroup = totalGuests >= 4;
            var promo4 = new PromotionItemResult
            {
                Code = "PROMO_GROUP4",
                Title = "เข้าพักเป็นกลุ่มตั้งแต่ 4 คนขึ้นไป",
                ConditionText = "ลด 20%",
                Description = "รับส่วนลดพิเศษ 20% สำหรับกลุ่มผู้เข้าพักตั้งแต่ 4 ท่านขึ้นไป",
                DiscountRate = 0.20m,
                IsEligible = eligibleGroup,
                EligibilityReason = eligibleGroup
                    ? $"จำนวนผู้เข้าพัก {totalGuests} ท่าน (ตรงตามเงื่อนไข >= 4 ท่าน)"
                    : $"จำนวนผู้เข้าพัก {totalGuests} ท่าน (ต้องการผู้เข้าพักตั้งแต่ 4 ท่านขึ้นไป)",
                DiscountAmount = eligibleGroup ? Math.Round(subtotal * 0.20m, 2) : 0m,
                IsApplied = eligibleGroup
            };
            result.Promotions.Add(promo4);

            // 5. จองล่วงหน้าอย่างน้อย 30 วันก่อนวันเช็คอิน: ลด 25%
            bool eligibleEarly = daysInAdvance >= 30;
            var promo5 = new PromotionItemResult
            {
                Code = "PROMO_EARLYBIRD30",
                Title = "จองล่วงหน้าอย่างน้อย 30 วันก่อนวันเช็คอิน",
                ConditionText = "ลด 25%",
                Description = "รับส่วนลด Early Bird สูงสุด 25% เมื่อจองล่วงหน้าอย่างน้อย 30 วัน",
                DiscountRate = 0.25m,
                IsEligible = eligibleEarly,
                EligibilityReason = eligibleEarly
                    ? $"จองล่วงหน้า {daysInAdvance} วัน (ตรงตามเงื่อนไข >= 30 วัน)"
                    : $"จองล่วงหน้า {daysInAdvance} วัน (ต้องการจองล่วงหน้าอย่างน้อย 30 วัน)",
                DiscountAmount = eligibleEarly ? Math.Round(subtotal * 0.25m, 2) : 0m,
                IsApplied = eligibleEarly
            };
            result.Promotions.Add(promo5);

            return result;
        }
    }
}

