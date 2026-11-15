using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Entities;
using UserModel = Nawarut_8250_L005.Models.User;

namespace Nawarut_8250_L005.Services;

public interface IUserService
{
    List<UserModel> GetAll();
    UserModel? GetById(int id);
    UserModel? GetByEmail(string email);
    UserModel? Authenticate(string email, string password);
    bool Add(UserModel user);
    bool Delete(int id);
}

public class UserService : IUserService
{
    private readonly Csi402dbContext _db;

    public UserService(Csi402dbContext db)
    {
        _db = db;
        EnsureSeedData();
    }

    public List<UserModel> GetAll()
    {
        return _db.Users
            .Include(u => u.Role)
            .OrderByDescending(u => u.UserId)
            .AsEnumerable()
            .Select(MapToModel)
            .ToList();
    }

    public UserModel? GetById(int id)
    {
        var entity = _db.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.UserId == (uint)id);

        return entity != null ? MapToModel(entity) : null;
    }

    public UserModel? GetByEmail(string email)
    {
        var entity = _db.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

        return entity != null ? MapToModel(entity) : null;
    }

    public UserModel? Authenticate(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        string cleanEmail = email.Trim().ToLower();
        string cleanPassword = password.Trim();

        var entity = _db.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email.ToLower() == cleanEmail && u.PasswordHash == cleanPassword);

        if (entity != null)
        {
            entity.LastLoginAt = DateTime.Now;
            try
            {
                _db.SaveChanges();
            }
            catch
            {
                // Ignore tracking error on login timestamp update
            }
            return MapToModel(entity);
        }

        return null;
    }

    public bool Add(UserModel user)
    {
        if (_db.Users.Any(u => u.Email.ToLower() == user.Email.ToLower()))
        {
            return false; // Email already exists
        }

        // Determine RoleId
        byte roleId = 5; // Default User
        string roleUpper = (user.Role ?? "").ToUpperInvariant();
        if (roleUpper.Contains("ADMIN"))
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "ADMIN");
            roleId = r?.RoleId ?? 1;
        }
        else if (roleUpper.Contains("STAFF") || roleUpper.Contains("RECEPTION"))
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "STAFF" || x.RoleCode == "RECEPTION");
            roleId = r?.RoleId ?? 2;
        }
        else if (roleUpper.Contains("ACCOUNT"))
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "ACCOUNTANT" || x.RoleCode == "ACCOUNTING");
            roleId = r?.RoleId ?? 3;
        }
        else if (roleUpper.Contains("SUPPORT") || roleUpper.Contains("SERVICE"))
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "SUPPORT" || x.RoleCode == "CUSTOMERSERVICE");
            roleId = r?.RoleId ?? 4;
        }
        else
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "USER" || x.RoleCode == "GUEST");
            roleId = r?.RoleId ?? 5;
        }

        // Parse FullName into FirstName and LastName
        var parts = (user.FullName ?? string.Empty).Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        string firstName = parts.Length > 0 ? parts[0] : user.Email;
        string lastName = parts.Length > 1 ? parts[1] : "-";

        var entity = new User
        {
            RoleId = roleId,
            Email = user.Email.Trim(),
            PasswordHash = string.IsNullOrEmpty(user.Password) ? "1234" : user.Password.Trim(),
            FirstName = firstName,
            LastName = lastName,
            Phone = string.IsNullOrWhiteSpace(user.PhoneNumber) ? null : user.PhoneNumber.Trim(),
            Status = string.Equals(user.Status, "Inactive", StringComparison.OrdinalIgnoreCase) ? "inactive" : "active",
            EmailVerified = true,
            CreatedAt = user.CreatedAt == default ? DateTime.Now : user.CreatedAt,
            UpdatedAt = DateTime.Now
        };

        _db.Users.Add(entity);
        _db.SaveChanges();

        user.Id = (int)entity.UserId;
        return true;
    }

    public bool Delete(int id)
    {
        var entity = _db.Users.Find((uint)id);
        if (entity != null)
        {
            _db.Users.Remove(entity);
            _db.SaveChanges();
            return true;
        }
        return false;
    }

    private static UserModel MapToModel(User entity)
    {
        string roleName = "User";
        var code = (entity.Role?.RoleCode ?? "").ToUpperInvariant();
        var name = (entity.Role?.RoleName ?? "").ToUpperInvariant();
        var email = (entity.Email ?? "").Trim().ToLowerInvariant();

        // 1. Explicit email checks for demo/seeded accounts
        if (email == "admin@jojo.com")
        {
            roleName = "Admin";
        }
        else if (email == "reception@jojo.com")
        {
            roleName = "Staff";
        }
        else if (email == "accountant@jojo.com")
        {
            roleName = "Accountant";
        }
        else if (email == "support@jojo.com")
        {
            roleName = "CustomerService";
        }
        else if (email == "somchai@gmail.com" || email == "nawaroodinmeesri1@gmail.com")
        {
            roleName = "User";
        }
        // 2. Role code / Role name checks (Guest/User first to prevent accidental admin mapping)
        else if (code.Contains("GUEST") || code.Contains("USER") || code.Contains("RENTER") ||
                 name.Contains("ลูกค้า") || name.Contains("ผู้ใช้งาน") || name.Contains("ผู้เช่า") ||
                 code == "GUEST" || code == "USER")
        {
            roleName = "User";
        }
        else if (code.Contains("ADMIN") || name.Contains("ADMIN") || name.Contains("ผู้ดูแลระบบ"))
        {
            roleName = "Admin";
        }
        else if (code.Contains("STAFF") || code.Contains("RECEPTION") || name.Contains("ต้อนรับ") || name.Contains("STAFF"))
        {
            roleName = "Staff";
        }
        else if (code.Contains("ACCOUNT") || name.Contains("บัญชี") || name.Contains("การเงิน"))
        {
            roleName = "Accountant";
        }
        else if (code.Contains("SUPPORT") || code.Contains("SERVICE") || name.Contains("บริการ") || name.Contains("ลูกค้า"))
        {
            roleName = "CustomerService";
        }
        else
        {
            roleName = "User";
        }

        string status = string.Equals(entity.Status, "inactive", StringComparison.OrdinalIgnoreCase) 
            ? "Inactive" 
            : "Active";

        return new UserModel
        {
            Id = (int)entity.UserId,
            FullName = $"{entity.FirstName} {entity.LastName}".Trim(),
            Email = entity.Email ?? string.Empty,
            PhoneNumber = entity.Phone ?? string.Empty,
            Role = roleName,
            Status = status,
            Password = entity.PasswordHash ?? string.Empty,
            CreatedAt = entity.CreatedAt
        };
    }

    private void EnsureSeedData()
    {
        try
        {
            // Seed roles if empty
            if (!_db.Roles.Any())
            {
                _db.Roles.AddRange(
                    new Role { RoleId = 1, RoleCode = "ADMIN", RoleName = "ผู้ดูแลระบบ", Description = "สิทธิ์ระดับผู้ดูแลระบบทั้งหมด" },
                    new Role { RoleId = 2, RoleCode = "STAFF", RoleName = "พนักงานต้อนรับ", Description = "พนักงานต้อนรับ / เจ้าหน้าที่ดูแลห้องพัก" },
                    new Role { RoleId = 3, RoleCode = "ACCOUNTANT", RoleName = "ฝ่ายบัญชีและการเงิน", Description = "เจ้าหน้าที่ฝ่ายบัญชีและการเงิน" },
                    new Role { RoleId = 4, RoleCode = "SUPPORT", RoleName = "ฝ่ายบริการลูกค้า", Description = "เจ้าหน้าที่บริการลูกค้าและประสานงาน" },
                    new Role { RoleId = 5, RoleCode = "USER", RoleName = "ผู้ใช้งานทั่วไป", Description = "ลูกค้า / ผู้ใช้งานทั่วไป" }
                );
                _db.SaveChanges();
            }
            else
            {
                // Ensure all roles exist
                if (!_db.Roles.Any(r => r.RoleCode == "ACCOUNTANT"))
                {
                    _db.Roles.Add(new Role { RoleCode = "ACCOUNTANT", RoleName = "ฝ่ายบัญชีและการเงิน", Description = "เจ้าหน้าที่ฝ่ายบัญชีและการเงิน" });
                    _db.SaveChanges();
                }
                if (!_db.Roles.Any(r => r.RoleCode == "SUPPORT"))
                {
                    _db.Roles.Add(new Role { RoleCode = "SUPPORT", RoleName = "ฝ่ายบริการลูกค้า", Description = "เจ้าหน้าที่บริการลูกค้าและประสานงาน" });
                    _db.SaveChanges();
                }
            }

            var adminRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "ADMIN") ?? _db.Roles.First();
            var staffRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "STAFF" || r.RoleCode == "RECEPTION") ?? adminRole;
            var accRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "ACCOUNTANT" || r.RoleCode == "ACCOUNTING") ?? staffRole;
            var supportRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "SUPPORT" || r.RoleCode == "CUSTOMERSERVICE") ?? staffRole;
            var userRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "USER" || r.RoleCode == "GUEST") ?? staffRole;

            // Ensure seed users for all 5 roles
            if (!_db.Users.Any(u => u.Email == "admin@jojo.com"))
            {
                _db.Users.Add(new User
                {
                    RoleId = adminRole.RoleId,
                    Email = "admin@jojo.com",
                    PasswordHash = "admin123",
                    FirstName = "Administrator",
                    LastName = "(ผู้ดูแลระบบ)",
                    Phone = "081-234-5678",
                    Status = "active",
                    EmailVerified = true,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    UpdatedAt = DateTime.Now.AddDays(-30)
                });
            }

            if (!_db.Users.Any(u => u.Email == "reception@jojo.com"))
            {
                _db.Users.Add(new User
                {
                    RoleId = staffRole.RoleId,
                    Email = "reception@jojo.com",
                    PasswordHash = "staff123",
                    FirstName = "วราภรณ์",
                    LastName = "พนักงานต้อนรับ",
                    Phone = "082-111-2233",
                    Status = "active",
                    EmailVerified = true,
                    CreatedAt = DateTime.Now.AddDays(-14),
                    UpdatedAt = DateTime.Now.AddDays(-14)
                });
            }

            if (!_db.Users.Any(u => u.Email == "accountant@jojo.com"))
            {
                _db.Users.Add(new User
                {
                    RoleId = accRole.RoleId,
                    Email = "accountant@jojo.com",
                    PasswordHash = "acc123",
                    FirstName = "กิตติศักดิ์",
                    LastName = "ฝ่ายการเงิน",
                    Phone = "085-444-9988",
                    Status = "active",
                    EmailVerified = true,
                    CreatedAt = DateTime.Now.AddDays(-12),
                    UpdatedAt = DateTime.Now.AddDays(-12)
                });
            }

            if (!_db.Users.Any(u => u.Email == "support@jojo.com"))
            {
                _db.Users.Add(new User
                {
                    RoleId = supportRole.RoleId,
                    Email = "support@jojo.com",
                    PasswordHash = "support123",
                    FirstName = "ศศิธร",
                    LastName = "บริการลูกค้า",
                    Phone = "087-333-4455",
                    Status = "active",
                    EmailVerified = true,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                });
            }

            if (!_db.Users.Any(u => u.Email == "somchai@gmail.com"))
            {
                _db.Users.Add(new User
                {
                    RoleId = userRole.RoleId,
                    Email = "somchai@gmail.com",
                    PasswordHash = "user123",
                    FirstName = "สมชาย",
                    LastName = "ใจดี",
                    Phone = "089-876-5432",
                    Status = "active",
                    EmailVerified = true,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now.AddDays(-15)
                });
            }
            else
            {
                var somchaiUser = _db.Users.FirstOrDefault(u => u.Email == "somchai@gmail.com");
                if (somchaiUser != null && somchaiUser.RoleId != userRole.RoleId)
                {
                    somchaiUser.RoleId = userRole.RoleId;
                }
            }

            var jojoUser = _db.Users.FirstOrDefault(u => u.Email == "nawaroodinmeesri1@gmail.com");
            if (jojoUser != null && jojoUser.RoleId != userRole.RoleId)
            {
                jojoUser.RoleId = userRole.RoleId;
            }

            _db.SaveChanges();
        }
        catch
        {
            // Fallback gracefully if database table is already filled or locked
        }
    }
}
