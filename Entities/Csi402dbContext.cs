using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Nawarut_8250_L005.Models;

namespace Nawarut_8250_L005.Entities;

public partial class Csi402dbContext : DbContext
{
    public Csi402dbContext()
    {
    }

    public Csi402dbContext(DbContextOptions<Csi402dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingRoom> BookingRooms { get; set; }

    public virtual DbSet<CancellationPolicy> CancellationPolicies { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomImage> RoomImages { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketMessage> TicketMessages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Labstudent> Labstudents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=csi402db;user=root;password=1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.11-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Labstudent>(entity =>
        {
            entity.HasKey(e => e.StdId);
            entity.ToTable("labstudent");
        });

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PRIMARY");

            entity.ToTable("amenities", tb => tb.HasComment("Ó©¬Ó©┤Ó╣êÓ©çÓ©¡Ó©│Ó©ÖÓ©ºÓ©óÓ©äÓ©ºÓ©▓Ó©íÓ©¬Ó©░Ó©öÓ©ºÓ©ü"));

            entity.HasIndex(e => e.AmenityName, "uq_amenity_name").IsUnique();

            entity.Property(e => e.AmenityId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©¬Ó©┤Ó╣êÓ©çÓ©¡Ó©│Ó©ÖÓ©ºÓ©óÓ©äÓ©ºÓ©▓Ó©íÓ©¬Ó©░Ó©öÓ©ºÓ©ü")
                .HasColumnName("amenity_id");
            entity.Property(e => e.AmenityName)
                .HasMaxLength(80)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡ Ó╣ÇÓ©èÓ╣êÓ©Ö Wi-Fi, TV, Ó©òÓ©╣Ó╣ëÓ╣ÇÓ©óÓ╣çÓ©Ö")
                .HasColumnName("amenity_name");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡Ó╣äÓ©¡Ó©äÓ©¡Ó©Ö")
                .HasColumnName("icon");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PRIMARY");

            entity.ToTable("audit_logs", tb => tb.HasComment("Audit Log"));

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "idx_audit_entity");

            entity.HasIndex(e => e.CreatedAt, "idx_audit_time");

            entity.HasIndex(e => e.UserId, "idx_audit_user");

            entity.Property(e => e.LogId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬ log")
                .HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasComment("Ó©üÓ©▓Ó©úÓ©üÓ©úÓ©░Ó©ùÓ©│ Ó╣ÇÓ©èÓ╣êÓ©Ö LOGIN, CREATE_BOOKING, REFUND")
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Detail)
                .HasComment("Ó©úÓ©▓Ó©óÓ©ÑÓ©░Ó╣ÇÓ©¡Ó©ÁÓ©óÓ©öÓ©üÓ╣êÓ©¡Ó©Ö/Ó©½Ó©ÑÓ©▒Ó©çÓ╣üÓ©üÓ╣ëÓ╣äÓ©é")
                .HasColumnType("json")
                .HasColumnName("detail");
            entity.Property(e => e.EntityId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó╣üÓ©ûÓ©ºÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©üÓ©ÁÓ╣êÓ©óÓ©ºÓ©éÓ╣ëÓ©¡Ó©ç")
                .HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasComment("Ó©òÓ©▓Ó©úÓ©▓Ó©çÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©üÓ©ÁÓ╣êÓ©óÓ©ºÓ©éÓ╣ëÓ©¡Ó©ç")
                .HasColumnName("entity_type");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasComment("IP Ó©£Ó©╣Ó╣ëÓ©üÓ©úÓ©░Ó©ùÓ©│")
                .HasColumnName("ip_address");
            entity.Property(e => e.UserId)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ©üÓ©úÓ©░Ó©ùÓ©│")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_audit_user");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PRIMARY");

            entity.ToTable("bookings", tb => tb.HasComment("Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç"));

            entity.HasIndex(e => e.ConfirmedBy, "fk_booking_confirm");

            entity.HasIndex(e => e.CreatedBy, "fk_booking_creator");

            entity.HasIndex(e => e.PromotionId, "fk_booking_promo");

            entity.HasIndex(e => new { e.CheckInDate, e.CheckOutDate }, "idx_booking_dates");

            entity.HasIndex(e => e.Status, "idx_booking_status");

            entity.HasIndex(e => e.UserId, "idx_booking_user");

            entity.HasIndex(e => e.BookingCode, "uq_booking_code").IsUnique();

            entity.Property(e => e.BookingId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç")
                .HasColumnName("booking_id");
            entity.Property(e => e.BookingCode)
                .HasMaxLength(20)
                .HasComment("Ó╣ÇÓ©ÑÓ©éÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©çÓ©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç Ó╣ÇÓ©èÓ╣êÓ©Ö BK20261115001")
                .HasColumnName("booking_code");
            entity.Property(e => e.CancelReason)
                .HasMaxLength(255)
                .HasComment("Ó╣ÇÓ©½Ó©òÓ©©Ó©£Ó©ÑÓ©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü")
                .HasColumnName("cancel_reason");
            entity.Property(e => e.CancelledAt)
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü")
                .HasColumnType("datetime")
                .HasColumnName("cancelled_at");
            entity.Property(e => e.CheckInDate)
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©ü")
                .HasColumnName("check_in_date");
            entity.Property(e => e.CheckOutDate)
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©¡Ó©¡Ó©ü")
                .HasColumnName("check_out_date");
            entity.Property(e => e.CheckedInAt)
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó╣ÇÓ©èÓ╣çÓ©äÓ©¡Ó©┤Ó©ÖÓ©êÓ©úÓ©┤Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("checked_in_at");
            entity.Property(e => e.CheckedOutAt)
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó╣ÇÓ©èÓ╣çÓ©äÓ╣ÇÓ©¡Ó©▓Ó©ùÓ╣îÓ©êÓ©úÓ©┤Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("checked_out_at");
            entity.Property(e => e.ConfirmedAt)
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©Ö")
                .HasColumnType("datetime")
                .HasColumnName("confirmed_at");
            entity.Property(e => e.ConfirmedBy)
                .HasComment("Ó©×Ó©ÖÓ©▒Ó©üÓ©çÓ©▓Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©Ö")
                .HasColumnName("confirmed_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©êÓ©¡Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ©¬Ó©úÓ╣ëÓ©▓Ó©çÓ©úÓ©▓Ó©óÓ©üÓ©▓Ó©ú (Ó©üÓ©úÓ©ôÓ©ÁÓ©×Ó©ÖÓ©▒Ó©üÓ©çÓ©▓Ó©ÖÓ©êÓ©¡Ó©çÓ╣üÓ©ùÓ©Ö)")
                .HasColumnName("created_by");
            entity.Property(e => e.DamageCharge)
                .HasPrecision(12, 2)
                .HasComment("Ó©äÓ╣êÓ©▓Ó╣ÇÓ©¬Ó©ÁÓ©óÓ©½Ó©▓Ó©ó/Ó©äÓ╣êÓ©▓Ó╣âÓ©èÓ╣ëÓ©êÓ╣êÓ©▓Ó©óÓ©½Ó©▒Ó©üÓ©êÓ©▓Ó©üÓ©íÓ©▒Ó©öÓ©êÓ©│")
                .HasColumnName("damage_charge");
            entity.Property(e => e.DepositAmount)
                .HasPrecision(12, 2)
                .HasComment("Ó©óÓ©¡Ó©öÓ©íÓ©▒Ó©öÓ©êÓ©│")
                .HasColumnName("deposit_amount");
            entity.Property(e => e.DepositDueAt)
                .HasComment("Ó©üÓ©│Ó©½Ó©ÖÓ©öÓ©èÓ©│Ó©úÓ©░Ó©íÓ©▒Ó©öÓ©êÓ©│ (Ó╣ÇÓ©üÓ©┤Ó©ÖÓ╣üÓ©ÑÓ╣ëÓ©ºÓ©½Ó©íÓ©öÓ©¡Ó©▓Ó©óÓ©©)")
                .HasColumnType("datetime")
                .HasColumnName("deposit_due_at");
            entity.Property(e => e.DepositPercent)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'5.00'")
                .HasComment("Ó╣ÇÓ©øÓ©¡Ó©úÓ╣îÓ╣ÇÓ©ïÓ╣çÓ©ÖÓ©òÓ╣îÓ©íÓ©▒Ó©öÓ©êÓ©│ Ó©ô Ó©ºÓ©▒Ó©ÖÓ©êÓ©¡Ó©ç")
                .HasColumnName("deposit_percent");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(12, 2)
                .HasComment("Ó©¬Ó╣êÓ©ºÓ©ÖÓ©ÑÓ©öÓ©úÓ©ºÓ©í")
                .HasColumnName("discount_amount");
            entity.Property(e => e.ExtraBedAmount)
                .HasPrecision(12, 2)
                .HasComment("Ó©äÓ╣êÓ©▓Ó╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©íÓ©úÓ©ºÓ©í")
                .HasColumnName("extra_bed_amount");
            entity.Property(e => e.NetAmount)
                .HasPrecision(12, 2)
                .HasComment("Ó©óÓ©¡Ó©öÓ©¬Ó©©Ó©ùÓ©ÿÓ©┤Ó©½Ó©ÑÓ©▒Ó©çÓ©ÑÓ©ö")
                .HasColumnName("net_amount");
            entity.Property(e => e.Nights)
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©äÓ©ÀÓ©Ö")
                .HasColumnName("nights");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasComment("Ó©½Ó©íÓ©▓Ó©óÓ╣ÇÓ©½Ó©òÓ©©")
                .HasColumnName("note");
            entity.Property(e => e.PromotionId)
                .HasComment("Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©ÖÓ©ùÓ©ÁÓ╣êÓ╣âÓ©èÓ╣ë (1 Ó╣éÓ©øÓ©úÓ©òÓ╣êÓ©¡ 1 Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç)")
                .HasColumnName("promotion_id");
            entity.Property(e => e.RefundAmount)
                .HasPrecision(12, 2)
                .HasComment("Ó©óÓ©¡Ó©öÓ©íÓ©▒Ó©öÓ©êÓ©│Ó©ùÓ©ÁÓ╣êÓ©äÓ©ÀÓ©ÖÓ©êÓ©úÓ©┤Ó©ç")
                .HasColumnName("refund_amount");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasComment("Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç")
                .HasColumnType("enum('pending','confirmed','checked_in','checked_out','completed','cancelled','no_show','expired')")
                .HasColumnName("status");
            entity.Property(e => e.SubtotalAmount)
                .HasPrecision(12, 2)
                .HasComment("Ó©äÓ╣êÓ©▓Ó©½Ó╣ëÓ©¡Ó©çÓ©úÓ©ºÓ©íÓ©üÓ╣êÓ©¡Ó©ÖÓ©ÑÓ©ö")
                .HasColumnName("subtotal_amount");
            entity.Property(e => e.TotalGuests)
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©£Ó©╣Ó╣ëÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©üÓ©úÓ©ºÓ©í")
                .HasColumnName("total_guests");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId)
                .HasComment("Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓Ó╣ÇÓ©êÓ╣ëÓ©▓Ó©éÓ©¡Ó©çÓ©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç")
                .HasColumnName("user_id");

            entity.HasOne(d => d.ConfirmedByNavigation).WithMany(p => p.BookingConfirmedByNavigations)
                .HasForeignKey(d => d.ConfirmedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_booking_confirm");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BookingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_booking_creator");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_booking_promo");

            entity.HasOne(d => d.User).WithMany(p => p.BookingUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_booking_user");
        });

        modelBuilder.Entity<BookingRoom>(entity =>
        {
            entity.HasKey(e => e.BookingRoomId).HasName("PRIMARY");

            entity.ToTable("booking_rooms", tb => tb.HasComment("Ó©½Ó╣ëÓ©¡Ó©çÓ╣âÓ©ÖÓ©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç"));

            entity.HasIndex(e => new { e.RoomId, e.CheckInDate, e.CheckOutDate }, "idx_br_room_dates");

            entity.HasIndex(e => new { e.BookingId, e.RoomId }, "uq_booking_room").IsUnique();

            entity.Property(e => e.BookingRoomId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©úÓ©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnName("booking_room_id");
            entity.Property(e => e.BookingId)
                .HasComment("Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç")
                .HasColumnName("booking_id");
            entity.Property(e => e.CheckInDate)
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©ü (Ó©¬Ó©│Ó╣ÇÓ©ÖÓ©▓Ó©êÓ©▓Ó©ü bookings)")
                .HasColumnName("check_in_date");
            entity.Property(e => e.CheckOutDate)
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©¡Ó©¡Ó©ü (Ó©¬Ó©│Ó╣ÇÓ©ÖÓ©▓Ó©êÓ©▓Ó©ü bookings)")
                .HasColumnName("check_out_date");
            entity.Property(e => e.ExtraBedPrice)
                .HasPrecision(10, 2)
                .HasComment("Ó©úÓ©▓Ó©äÓ©▓Ó╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©íÓ©òÓ╣êÓ©¡Ó©äÓ©ÀÓ©Ö Ó©ô Ó©ºÓ©▒Ó©ÖÓ©êÓ©¡Ó©ç")
                .HasColumnName("extra_bed_price");
            entity.Property(e => e.ExtraBeds)
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©í")
                .HasColumnName("extra_beds");
            entity.Property(e => e.Guests)
                .HasDefaultValueSql("'1'")
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©£Ó©╣Ó╣ëÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©üÓ╣âÓ©ÖÓ©½Ó╣ëÓ©¡Ó©çÓ©ÖÓ©ÁÓ╣ë")
                .HasColumnName("guests");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("Ó©óÓ©▒Ó©çÓ©íÓ©ÁÓ©£Ó©ÑÓ©üÓ©▒Ó©ÜÓ©òÓ©▓Ó©úÓ©▓Ó©çÓ©½Ó╣ëÓ©¡Ó©ç (0 = Ó©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü/Ó©êÓ©ÜÓ╣üÓ©ÑÓ╣ëÓ©º)")
                .HasColumnName("is_active");
            entity.Property(e => e.LineTotal)
                .HasPrecision(12, 2)
                .HasComment("Ó©úÓ©ºÓ©íÓ©½Ó╣ëÓ©¡Ó©çÓ©ÖÓ©ÁÓ╣ë (Ó©½Ó╣ëÓ©¡Ó©ç + Ó╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©í) Ó©ùÓ©©Ó©üÓ©äÓ©ÀÓ©Ö")
                .HasColumnName("line_total");
            entity.Property(e => e.PricePerNight)
                .HasPrecision(10, 2)
                .HasComment("Ó©úÓ©▓Ó©äÓ©▓Ó©½Ó╣ëÓ©¡Ó©çÓ©òÓ╣êÓ©¡Ó©äÓ©ÀÓ©Ö Ó©ô Ó©ºÓ©▒Ó©ÖÓ©êÓ©¡Ó©ç (snapshot)")
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomId)
                .HasComment("Ó©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnName("room_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingRooms)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("fk_br_booking");

            entity.HasOne(d => d.Room).WithMany(p => p.BookingRooms)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_br_room");
        });

        modelBuilder.Entity<CancellationPolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PRIMARY");

            entity.ToTable("cancellation_policies", tb => tb.HasComment("Ó©ÖÓ╣éÓ©óÓ©ÜÓ©▓Ó©óÓ©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©ü"));

            entity.HasIndex(e => e.MinDaysBefore, "uq_policy_days").IsUnique();

            entity.Property(e => e.PolicyId)
                .ValueGeneratedOnAdd()
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©ÖÓ╣éÓ©óÓ©ÜÓ©▓Ó©ó")
                .HasColumnName("policy_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasComment("Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©ó")
                .HasColumnName("description");
            entity.Property(e => e.MinDaysBefore)
                .HasComment("Ó©óÓ©üÓ╣ÇÓ©ÑÓ©┤Ó©üÓ©üÓ╣êÓ©¡Ó©ÖÓ╣ÇÓ©èÓ╣çÓ©äÓ©¡Ó©┤Ó©ÖÓ©¡Ó©óÓ╣êÓ©▓Ó©çÓ©ÖÓ╣ëÓ©¡Ó©óÓ©üÓ©ÁÓ╣êÓ©ºÓ©▒Ó©Ö")
                .HasColumnName("min_days_before");
            entity.Property(e => e.RefundPercent)
                .HasPrecision(5, 2)
                .HasComment("Ó╣ÇÓ©øÓ©¡Ó©úÓ╣îÓ╣ÇÓ©ïÓ╣çÓ©ÖÓ©òÓ╣îÓ©íÓ©▒Ó©öÓ©êÓ©│Ó©ùÓ©ÁÓ╣êÓ©äÓ©ÀÓ©Ö")
                .HasColumnName("refund_percent");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PRIMARY");

            entity.ToTable("departments", tb => tb.HasComment("Ó╣üÓ©£Ó©ÖÓ©ü"));

            entity.HasIndex(e => e.DepartmentName, "uq_departments_name").IsUnique();

            entity.Property(e => e.DepartmentId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó╣üÓ©£Ó©ÖÓ©ü")
                .HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(80)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡Ó╣üÓ©£Ó©ÖÓ©ü")
                .HasColumnName("department_name");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasComment("Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©ó")
                .HasColumnName("description");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("notifications", tb => tb.HasComment("Ó©üÓ©▓Ó©úÓ╣üÓ©êÓ╣ëÓ©çÓ╣ÇÓ©òÓ©ÀÓ©¡Ó©Ö"));

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "idx_notif_user");

            entity.Property(e => e.NotificationId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©üÓ©▓Ó©úÓ╣üÓ©êÓ╣ëÓ©çÓ╣ÇÓ©òÓ©ÀÓ©¡Ó©Ö")
                .HasColumnName("notification_id");
            entity.Property(e => e.Body)
                .HasMaxLength(500)
                .HasComment("Ó╣ÇÓ©ÖÓ©ÀÓ╣ëÓ©¡Ó©½Ó©▓")
                .HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasComment("Ó©¡Ó╣êÓ©▓Ó©ÖÓ╣üÓ©ÑÓ╣ëÓ©º (0/1)")
                .HasColumnName("is_read");
            entity.Property(e => e.LinkId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©úÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©ç")
                .HasColumnName("link_id");
            entity.Property(e => e.LinkType)
                .HasMaxLength(30)
                .HasComment("Ó©èÓ©ÖÓ©┤Ó©öÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©ç Ó╣ÇÓ©èÓ╣êÓ©Ö booking, ticket")
                .HasColumnName("link_type");
            entity.Property(e => e.Title)
                .HasMaxLength(120)
                .HasComment("Ó©½Ó©▒Ó©ºÓ©éÓ╣ëÓ©¡")
                .HasColumnName("title");
            entity.Property(e => e.UserId)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ©úÓ©▒Ó©Ü")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_notif_user");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.ResetId).HasName("PRIMARY");

            entity.ToTable("password_resets", tb => tb.HasComment("Ó╣éÓ©ùÓ╣ÇÓ©äÓ©ÖÓ©úÓ©ÁÓ╣ÇÓ©ïÓ╣çÓ©òÓ©úÓ©½Ó©▒Ó©¬Ó©£Ó╣êÓ©▓Ó©Ö/Ó©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©ÖÓ©¡Ó©ÁÓ╣ÇÓ©íÓ©Ñ"));

            entity.HasIndex(e => e.UserId, "idx_reset_user");

            entity.HasIndex(e => e.TokenHash, "uq_reset_token").IsUnique();

            entity.Property(e => e.ResetId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©ú")
                .HasColumnName("reset_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasComment("Ó©½Ó©íÓ©öÓ©¡Ó©▓Ó©óÓ©©")
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.Purpose)
                .HasDefaultValueSql("'reset_password'")
                .HasComment("Ó©ºÓ©▒Ó©òÓ©ûÓ©©Ó©øÓ©úÓ©░Ó©¬Ó©çÓ©äÓ╣î")
                .HasColumnType("enum('reset_password','verify_email')")
                .HasColumnName("purpose");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasComment("Ó╣éÓ©ùÓ╣ÇÓ©äÓ©Ö (SHA-256 hash)")
                .HasColumnName("token_hash");
            entity.Property(e => e.UsedAt)
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ╣âÓ©èÓ╣ëÓ╣üÓ©ÑÓ╣ëÓ©º")
                .HasColumnType("datetime")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ë")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_reset_user");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payments", tb => tb.HasComment("Ó©üÓ©▓Ó©úÓ©èÓ©│Ó©úÓ©░Ó╣ÇÓ©çÓ©┤Ó©Ö"));

            entity.HasIndex(e => e.ProcessedBy, "fk_pay_user");

            entity.HasIndex(e => e.BookingId, "idx_pay_booking");

            entity.HasIndex(e => new { e.Status, e.PaymentType }, "idx_pay_status");

            entity.Property(e => e.PaymentId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©▓Ó©óÓ©üÓ©▓Ó©úÓ©èÓ©│Ó©úÓ©░")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId)
                .HasComment("Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©ç")
                .HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©çÓ©úÓ©▓Ó©óÓ©üÓ©▓Ó©ú")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Method)
                .HasComment("Ó©èÓ╣êÓ©¡Ó©çÓ©ùÓ©▓Ó©çÓ©èÓ©│Ó©úÓ©░")
                .HasColumnType("enum('credit_card','promptpay','bank_transfer','cash')")
                .HasColumnName("method");
            entity.Property(e => e.PaidAt)
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©èÓ©│Ó©úÓ©░Ó©¬Ó©│Ó╣ÇÓ©úÓ╣çÓ©ê")
                .HasColumnType("datetime")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentType)
                .HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ù: Ó©íÓ©▒Ó©öÓ©êÓ©│/Ó©¬Ó╣êÓ©ºÓ©ÖÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©½Ó©ÑÓ©ÀÓ©¡/Ó©äÓ©ÀÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö/Ó©äÓ╣êÓ©▓Ó╣ÇÓ©¬Ó©ÁÓ©óÓ©½Ó©▓Ó©ó")
                .HasColumnType("enum('deposit','balance','refund','charge')")
                .HasColumnName("payment_type");
            entity.Property(e => e.ProcessedBy)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ©öÓ©│Ó╣ÇÓ©ÖÓ©┤Ó©ÖÓ©üÓ©▓Ó©ú (Ó╣ÇÓ©èÓ╣êÓ©Ö Ó©ÜÓ©▒Ó©ìÓ©èÓ©ÁÓ©ùÓ©ÁÓ╣êÓ©äÓ©ÀÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö)")
                .HasColumnName("processed_by");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasComment("Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░")
                .HasColumnType("enum('pending','success','failed','refunded')")
                .HasColumnName("status");
            entity.Property(e => e.TransactionRef)
                .HasMaxLength(100)
                .HasComment("Ó╣ÇÓ©ÑÓ©éÓ©¡Ó╣ëÓ©▓Ó©çÓ©¡Ó©┤Ó©çÓ©êÓ©▓Ó©üÓ©£Ó©╣Ó╣ëÓ╣âÓ©½Ó╣ëÓ©ÜÓ©úÓ©┤Ó©üÓ©▓Ó©úÓ©èÓ©│Ó©úÓ©░Ó╣ÇÓ©çÓ©┤Ó©Ö")
                .HasColumnName("transaction_ref");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pay_booking");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ProcessedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_pay_user");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PRIMARY");

            entity.ToTable("promotions", tb => tb.HasComment("Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©Ö"));

            entity.HasIndex(e => e.CreatedBy, "fk_promo_creator");

            entity.HasIndex(e => e.PromoCode, "uq_promo_code").IsUnique();

            entity.Property(e => e.PromotionId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©Ö")
                .HasColumnName("promotion_id");
            entity.Property(e => e.ConditionType)
                .HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ╣ÇÓ©çÓ©ÀÓ╣êÓ©¡Ó©ÖÓ╣äÓ©é")
                .HasColumnType("enum('first_booking','min_nights','min_amount','min_guests','advance_days')")
                .HasColumnName("condition_type");
            entity.Property(e => e.ConditionValue)
                .HasPrecision(10, 2)
                .HasComment("Ó©äÓ╣êÓ©▓Ó╣ÇÓ©çÓ©ÀÓ╣êÓ©¡Ó©ÖÓ╣äÓ©é (Ó©äÓ©ÀÓ©Ö/Ó©ÜÓ©▓Ó©ù/Ó©äÓ©Ö/Ó©ºÓ©▒Ó©Ö)")
                .HasColumnName("condition_value");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç (Ó╣üÓ©¡Ó©öÓ©íÓ©┤Ó©Ö)")
                .HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasComment("Ó©úÓ©▓Ó©óÓ©ÑÓ©░Ó╣ÇÓ©¡Ó©ÁÓ©óÓ©ö")
                .HasColumnName("description");
            entity.Property(e => e.DiscountType)
                .HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©¬Ó╣êÓ©ºÓ©ÖÓ©ÑÓ©ö: Ó╣ÇÓ©øÓ©¡Ó©úÓ╣îÓ╣ÇÓ©ïÓ╣çÓ©ÖÓ©òÓ╣î/Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©çÓ©┤Ó©Ö")
                .HasColumnType("enum('percent','fixed')")
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasComment("Ó©äÓ╣êÓ©▓Ó©¬Ó╣êÓ©ºÓ©ÖÓ©ÑÓ©ö (% Ó©½Ó©úÓ©ÀÓ©¡ Ó©ÜÓ©▓Ó©ù)")
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate)
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©¬Ó©┤Ó╣ëÓ©ÖÓ©¬Ó©©Ó©öÓ╣éÓ©øÓ©ú")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("Ó╣ÇÓ©øÓ©┤Ó©öÓ╣âÓ©èÓ╣ëÓ©çÓ©▓Ó©Ö (0/1)")
                .HasColumnName("is_active");
            entity.Property(e => e.PromoCode)
                .HasMaxLength(30)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó╣éÓ©øÓ©ú Ó╣ÇÓ©èÓ╣êÓ©Ö NEWMEMBER10")
                .HasColumnName("promo_code");
            entity.Property(e => e.PromoName)
                .HasMaxLength(100)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡Ó╣éÓ©øÓ©úÓ╣éÓ©íÓ©èÓ©▒Ó╣êÓ©Ö")
                .HasColumnName("promo_name");
            entity.Property(e => e.StartDate)
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ╣ÇÓ©úÓ©┤Ó╣êÓ©íÓ╣éÓ©øÓ©ú")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_promo_creator");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("roles", tb => tb.HasComment("Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ùÓ©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ë"));

            entity.HasIndex(e => e.RoleCode, "uq_roles_code").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù")
                .HasColumnName("role_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasComment("Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©óÓ©½Ó©ÖÓ╣ëÓ©▓Ó©ùÓ©ÁÓ╣ê")
                .HasColumnName("description");
            entity.Property(e => e.RoleCode)
                .HasMaxLength(30)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù Ó╣ÇÓ©èÓ╣êÓ©Ö GUEST, STAFF, ACCOUNTANT, SUPPORT, ADMIN")
                .HasColumnName("role_code");
            entity.Property(e => e.RoleName)
                .HasMaxLength(60)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù")
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PRIMARY");

            entity.ToTable("rooms", tb => tb.HasComment("Ó©½Ó╣ëÓ©¡Ó©çÓ©×Ó©▒Ó©ü"));

            entity.HasIndex(e => e.Status, "idx_rooms_status");

            entity.HasIndex(e => e.RoomTypeId, "idx_rooms_type");

            entity.HasIndex(e => e.RoomNumber, "uq_rooms_number").IsUnique();

            entity.Property(e => e.RoomId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnName("room_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FloorNo)
                .HasComment("Ó©èÓ©▒Ó╣ëÓ©Ö")
                .HasColumnName("floor_no");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("Ó╣ÇÓ©øÓ©┤Ó©öÓ╣âÓ©èÓ╣ëÓ©çÓ©▓Ó©Ö (0/1)")
                .HasColumnName("is_active");
            entity.Property(e => e.PricePerNight)
                .HasPrecision(10, 2)
                .HasComment("Ó©úÓ©▓Ó©äÓ©▓Ó╣ÇÓ©ëÓ©×Ó©▓Ó©░Ó©½Ó╣ëÓ©¡Ó©ç (Ó©ûÓ╣ëÓ©▓ NULL Ó╣âÓ©èÓ╣ë base_price Ó©éÓ©¡Ó©çÓ©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ù)")
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(10)
                .HasComment("Ó╣ÇÓ©ÑÓ©éÓ©½Ó╣ëÓ©¡Ó©ç Ó╣ÇÓ©èÓ╣êÓ©Ö 1205")
                .HasColumnName("room_number");
            entity.Property(e => e.RoomTypeId)
                .HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç (FK)")
                .HasColumnName("room_type_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'available'")
                .HasComment("Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░Ó©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnType("enum('available','reserved','occupied','cleaning','maintenance')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rooms_type");
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");

            entity.ToTable("room_images", tb => tb.HasComment("Ó©úÓ©╣Ó©øÓ©áÓ©▓Ó©×Ó©½Ó╣ëÓ©¡Ó©ç"));

            entity.HasIndex(e => e.RoomTypeId, "idx_images_type");

            entity.Property(e => e.ImageId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©úÓ©╣Ó©ø")
                .HasColumnName("image_id");
            entity.Property(e => e.Caption)
                .HasMaxLength(150)
                .HasComment("Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©óÓ©úÓ©╣Ó©ø")
                .HasColumnName("caption");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasComment("Ó©ùÓ©ÁÓ╣êÓ©¡Ó©óÓ©╣Ó╣êÓ╣äÓ©ƒÓ©ÑÓ╣îÓ©úÓ©╣Ó©ø")
                .HasColumnName("image_url");
            entity.Property(e => e.IsCover)
                .HasComment("Ó╣ÇÓ©øÓ╣çÓ©ÖÓ©úÓ©╣Ó©øÓ©øÓ©ü (0/1)")
                .HasColumnName("is_cover");
            entity.Property(e => e.RoomTypeId)
                .HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnName("room_type_id");
            entity.Property(e => e.SortOrder)
                .HasComment("Ó©ÑÓ©│Ó©öÓ©▒Ó©ÜÓ╣üÓ©¬Ó©öÓ©ç")
                .HasColumnName("sort_order");

            entity.HasOne(d => d.RoomType).WithMany(p => p.RoomImages)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("fk_images_type");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PRIMARY");

            entity.ToTable("room_types", tb => tb.HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©çÓ©×Ó©▒Ó©ü"));

            entity.HasIndex(e => e.TypeName, "uq_room_types_name").IsUnique();

            entity.Property(e => e.RoomTypeId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnName("room_type_id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasComment("Ó©úÓ©▓Ó©äÓ©▓Ó©×Ó©ÀÓ╣ëÓ©ÖÓ©ÉÓ©▓Ó©ÖÓ©òÓ╣êÓ©¡Ó©äÓ©ÀÓ©Ö (Ó©ÜÓ©▓Ó©ù)")
                .HasColumnName("base_price");
            entity.Property(e => e.BedType)
                .HasMaxLength(50)
                .HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ╣ÇÓ©òÓ©ÁÓ©óÓ©ç Ó╣ÇÓ©èÓ╣êÓ©Ö King, Twin")
                .HasColumnName("bed_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©¬Ó©úÓ╣ëÓ©▓Ó©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasComment("Ó©úÓ©▓Ó©óÓ©ÑÓ©░Ó╣ÇÓ©¡Ó©ÁÓ©óÓ©öÓ©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("Ó╣ÇÓ©øÓ©┤Ó©öÓ©éÓ©▓Ó©ó (0/1)")
                .HasColumnName("is_active");
            entity.Property(e => e.MaxExtraBeds)
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ╣ÇÓ©òÓ©ÁÓ©óÓ©çÓ╣ÇÓ©¬Ó©úÓ©┤Ó©íÓ©¬Ó©╣Ó©çÓ©¬Ó©©Ó©ö")
                .HasColumnName("max_extra_beds");
            entity.Property(e => e.MaxGuests)
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©£Ó©╣Ó╣ëÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©×Ó©▒Ó©üÓ©¬Ó©╣Ó©çÓ©¬Ó©©Ó©öÓ©òÓ╣êÓ©¡Ó©½Ó╣ëÓ©¡Ó©ç")
                .HasColumnName("max_guests");
            entity.Property(e => e.SizeSqm)
                .HasPrecision(6, 2)
                .HasComment("Ó©éÓ©ÖÓ©▓Ó©öÓ©½Ó╣ëÓ©¡Ó©ç (Ó©òÓ©ú.Ó©í.)")
                .HasColumnName("size_sqm");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ù Ó╣ÇÓ©èÓ╣êÓ©Ö Deluxe King Suite")
                .HasColumnName("type_name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Amenities).WithMany(p => p.RoomTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "RoomTypeAmenity",
                    r => r.HasOne<Amenity>().WithMany()
                        .HasForeignKey("AmenityId")
                        .HasConstraintName("fk_rta_amenity"),
                    l => l.HasOne<RoomType>().WithMany()
                        .HasForeignKey("RoomTypeId")
                        .HasConstraintName("fk_rta_type"),
                    j =>
                    {
                        j.HasKey("RoomTypeId", "AmenityId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("room_type_amenities", tb => tb.HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç-Ó©¬Ó©┤Ó╣êÓ©çÓ©¡Ó©│Ó©ÖÓ©ºÓ©óÓ©äÓ©ºÓ©▓Ó©íÓ©¬Ó©░Ó©öÓ©ºÓ©ü"));
                        j.HasIndex(new[] { "AmenityId" }, "fk_rta_amenity");
                        j.IndexerProperty<ushort>("RoomTypeId")
                            .HasComment("Ó©øÓ©úÓ©░Ó╣ÇÓ©áÓ©ùÓ©½Ó╣ëÓ©¡Ó©ç")
                            .HasColumnName("room_type_id");
                        j.IndexerProperty<ushort>("AmenityId")
                            .HasComment("Ó©¬Ó©┤Ó╣êÓ©çÓ©¡Ó©│Ó©ÖÓ©ºÓ©óÓ©äÓ©ºÓ©▓Ó©íÓ©¬Ó©░Ó©öÓ©ºÓ©ü")
                            .HasColumnName("amenity_id");
                    });
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.SettingKey).HasName("PRIMARY");

            entity.ToTable("system_settings", tb => tb.HasComment("Ó©äÓ╣êÓ©▓Ó©òÓ©▒Ó╣ëÓ©çÓ©úÓ©░Ó©ÜÓ©Ü"));

            entity.HasIndex(e => e.UpdatedBy, "fk_set_user");

            entity.Property(e => e.SettingKey)
                .HasMaxLength(50)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡Ó©äÓ╣êÓ©▓Ó©òÓ©▒Ó╣ëÓ©ç")
                .HasColumnName("setting_key");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasComment("Ó©äÓ©│Ó©¡Ó©ÿÓ©┤Ó©ÜÓ©▓Ó©ó")
                .HasColumnName("description");
            entity.Property(e => e.SettingValue)
                .HasMaxLength(255)
                .HasComment("Ó©äÓ╣êÓ©▓")
                .HasColumnName("setting_value");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnName("updated_by");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SystemSettings)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_set_user");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PRIMARY");

            entity.ToTable("tickets", tb => tb.HasComment("Ticket Ó©øÓ©▒Ó©ìÓ©½Ó©▓Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓"));

            entity.HasIndex(e => e.AssignedTo, "fk_ticket_assignee");

            entity.HasIndex(e => e.BookingId, "fk_ticket_booking");

            entity.HasIndex(e => e.CustomerId, "fk_ticket_customer");

            entity.HasIndex(e => e.DepartmentId, "fk_ticket_dept");

            entity.HasIndex(e => new { e.Status, e.Priority }, "idx_ticket_status");

            entity.HasIndex(e => e.TicketCode, "uq_ticket_code").IsUnique();

            entity.Property(e => e.TicketId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬ Ticket")
                .HasColumnName("ticket_id");
            entity.Property(e => e.AssignedTo)
                .HasComment("Ó╣ÇÓ©êÓ╣ëÓ©▓Ó©½Ó©ÖÓ╣ëÓ©▓Ó©ùÓ©ÁÓ╣ê CS Ó©£Ó©╣Ó╣ëÓ©úÓ©▒Ó©ÜÓ╣ÇÓ©úÓ©ÀÓ╣êÓ©¡Ó©ç")
                .HasColumnName("assigned_to");
            entity.Property(e => e.BookingId)
                .HasComment("Ó©üÓ©▓Ó©úÓ©êÓ©¡Ó©çÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©üÓ©ÁÓ╣êÓ©óÓ©ºÓ©éÓ╣ëÓ©¡Ó©ç")
                .HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ╣üÓ©êÓ╣ëÓ©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId)
                .HasComment("Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓Ó©£Ó©╣Ó╣ëÓ╣üÓ©êÓ╣ëÓ©ç")
                .HasColumnName("customer_id");
            entity.Property(e => e.DepartmentId)
                .HasComment("Ó╣üÓ©£Ó©ÖÓ©üÓ©øÓ©ÑÓ©▓Ó©óÓ©ùÓ©▓Ó©çÓ©ùÓ©ÁÓ╣êÓ©¬Ó╣êÓ©çÓ©òÓ╣êÓ©¡")
                .HasColumnName("department_id");
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("'normal'")
                .HasComment("Ó©äÓ©ºÓ©▓Ó©íÓ╣ÇÓ©úÓ╣êÓ©çÓ©öÓ╣êÓ©ºÓ©Ö")
                .HasColumnType("enum('low','normal','urgent')")
                .HasColumnName("priority");
            entity.Property(e => e.ResolvedAt)
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ╣üÓ©üÓ╣ëÓ©øÓ©▒Ó©ìÓ©½Ó©▓Ó╣ÇÓ©¬Ó©úÓ╣çÓ©ê")
                .HasColumnType("datetime")
                .HasColumnName("resolved_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'open'")
                .HasComment("Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░")
                .HasColumnType("enum('open','in_progress','escalated','resolved','closed')")
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(150)
                .HasComment("Ó©½Ó©▒Ó©ºÓ©éÓ╣ëÓ©¡")
                .HasColumnName("subject");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(20)
                .HasComment("Ó╣ÇÓ©ÑÓ©é Ticket Ó╣ÇÓ©èÓ╣êÓ©Ö TK-001")
                .HasColumnName("ticket_code");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.TicketAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ticket_assignee");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ticket_booking");

            entity.HasOne(d => d.Customer).WithMany(p => p.TicketCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_customer");

            entity.HasOne(d => d.Department).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ticket_dept");
        });

        modelBuilder.Entity<TicketMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PRIMARY");

            entity.ToTable("ticket_messages", tb => tb.HasComment("Ó©éÓ╣ëÓ©¡Ó©äÓ©ºÓ©▓Ó©íÓ╣âÓ©Ö Ticket"));

            entity.HasIndex(e => e.SenderId, "fk_tm_sender");

            entity.HasIndex(e => e.TicketId, "idx_tm_ticket");

            entity.Property(e => e.MessageId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©éÓ╣ëÓ©¡Ó©äÓ©ºÓ©▓Ó©í")
                .HasColumnName("message_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣ÇÓ©ºÓ©ÑÓ©▓Ó©ùÓ©ÁÓ╣êÓ©¬Ó╣êÓ©ç")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsInternal)
                .HasComment("Ó©ÜÓ©▒Ó©ÖÓ©ùÓ©ÂÓ©üÓ©áÓ©▓Ó©óÓ╣âÓ©Ö Ó©ÑÓ©╣Ó©üÓ©äÓ╣ëÓ©▓Ó╣äÓ©íÓ╣êÓ╣ÇÓ©½Ó╣çÓ©Ö (0/1)")
                .HasColumnName("is_internal");
            entity.Property(e => e.Message)
                .HasComment("Ó©éÓ╣ëÓ©¡Ó©äÓ©ºÓ©▓Ó©í")
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.SenderId)
                .HasComment("Ó©£Ó©╣Ó╣ëÓ©¬Ó╣êÓ©ç")
                .HasColumnName("sender_id");
            entity.Property(e => e.TicketId)
                .HasComment("Ticket")
                .HasColumnName("ticket_id");

            entity.HasOne(d => d.Sender).WithMany(p => p.TicketMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tm_sender");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketMessages)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("fk_tm_ticket");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users", tb => tb.HasComment("Ó©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ëÓ©úÓ©░Ó©ÜÓ©Ü"));

            entity.HasIndex(e => e.DepartmentId, "fk_users_dept");

            entity.HasIndex(e => e.RoleId, "idx_users_role");

            entity.HasIndex(e => e.Email, "uq_users_email").IsUnique();

            entity.Property(e => e.UserId)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©£Ó©╣Ó╣ëÓ╣âÓ©èÓ╣ë")
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó©ºÓ©▒Ó©ÖÓ©ùÓ©ÁÓ╣êÓ©¬Ó©íÓ©▒Ó©äÓ©ú")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartmentId)
                .HasComment("Ó╣üÓ©£Ó©ÖÓ©ü (Ó╣ÇÓ©ëÓ©×Ó©▓Ó©░Ó©×Ó©ÖÓ©▒Ó©üÓ©çÓ©▓Ó©Ö)")
                .HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasComment("Ó©¡Ó©ÁÓ╣ÇÓ©íÓ©Ñ Ó╣âÓ©èÓ╣ëÓ©ÑÓ╣çÓ©¡Ó©üÓ©¡Ó©┤Ó©Ö")
                .HasColumnName("email");
            entity.Property(e => e.EmailVerified)
                .HasComment("Ó©óÓ©ÀÓ©ÖÓ©óÓ©▒Ó©ÖÓ©¡Ó©ÁÓ╣ÇÓ©íÓ©ÑÓ╣üÓ©ÑÓ╣ëÓ©º (0/1)")
                .HasColumnName("email_verified");
            entity.Property(e => e.FailedLogins)
                .HasComment("Ó©êÓ©│Ó©ÖÓ©ºÓ©ÖÓ©äÓ©úÓ©▒Ó╣ëÓ©çÓ©ÑÓ╣çÓ©¡Ó©üÓ©¡Ó©┤Ó©ÖÓ©£Ó©┤Ó©öÓ©òÓ©┤Ó©öÓ©òÓ╣êÓ©¡Ó©üÓ©▒Ó©Ö")
                .HasColumnName("failed_logins");
            entity.Property(e => e.FirstName)
                .HasMaxLength(80)
                .HasComment("Ó©èÓ©ÀÓ╣êÓ©¡")
                .HasColumnName("first_name");
            entity.Property(e => e.LastLoginAt)
                .HasComment("Ó╣ÇÓ©éÓ╣ëÓ©▓Ó©¬Ó©╣Ó╣êÓ©úÓ©░Ó©ÜÓ©ÜÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("last_login_at");
            entity.Property(e => e.LastName)
                .HasMaxLength(80)
                .HasComment("Ó©ÖÓ©▓Ó©íÓ©¬Ó©üÓ©©Ó©Ñ")
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasComment("Ó©úÓ©½Ó©▒Ó©¬Ó©£Ó╣êÓ©▓Ó©ÖÓ©ùÓ©ÁÓ╣êÓ╣ÇÓ©éÓ╣ëÓ©▓Ó©úÓ©½Ó©▒Ó©¬Ó╣üÓ©ÑÓ╣ëÓ©º (bcrypt/argon2)")
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasComment("Ó╣ÇÓ©ÜÓ©¡Ó©úÓ╣îÓ╣éÓ©ùÓ©ú")
                .HasColumnName("phone");
            entity.Property(e => e.RoleId)
                .HasComment("Ó©ÜÓ©ùÓ©ÜÓ©▓Ó©ù (FK roles)")
                .HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'active'")
                .HasComment("Ó©¬Ó©ûÓ©▓Ó©ÖÓ©░Ó©ÜÓ©▒Ó©ìÓ©èÓ©Á")
                .HasColumnType("enum('active','inactive','locked')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Ó╣üÓ©üÓ╣ëÓ╣äÓ©éÓ©ÑÓ╣êÓ©▓Ó©¬Ó©©Ó©ö")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Department).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("fk_users_dept");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
