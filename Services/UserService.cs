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
        byte roleId = 3; // Default User
        if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "ADMIN");
            roleId = r?.RoleId ?? 1;
        }
        else if (string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "STAFF");
            roleId = r?.RoleId ?? 2;
        }
        else
        {
            var r = _db.Roles.FirstOrDefault(x => x.RoleCode == "USER" || x.RoleCode == "GUEST");
            roleId = r?.RoleId ?? 3;
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
        if (entity.RoleId == 1 || (entity.Role != null && string.Equals(entity.Role.RoleCode, "ADMIN", StringComparison.OrdinalIgnoreCase)))
        {
            roleName = "Admin";
        }
        else if (entity.RoleId == 2 || (entity.Role != null && string.Equals(entity.Role.RoleCode, "STAFF", StringComparison.OrdinalIgnoreCase)))
        {
            roleName = "Staff";
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
            Email = entity.Email,
            PhoneNumber = entity.Phone ?? string.Empty,
            Role = roleName,
            Status = status,
            Password = entity.PasswordHash,
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
                    new Role { RoleId = 2, RoleCode = "STAFF", RoleName = "พนักงาน", Description = "สิทธิ์พนักงานปฏิบัติการ" },
                    new Role { RoleId = 3, RoleCode = "USER", RoleName = "ผู้ใช้งานทั่วไป", Description = "สิทธิ์ลูกค้า/ผู้ใช้งานทั่วไป" }
                );
                _db.SaveChanges();
            }

            // Seed users if empty
            if (!_db.Users.Any())
            {
                _db.Users.AddRange(
                    new User
                    {
                        RoleId = 1,
                        Email = "admin@jojo.com",
                        PasswordHash = "admin123",
                        FirstName = "Administrator",
                        LastName = "(ผู้ดูแลระบบ)",
                        Phone = "081-234-5678",
                        Status = "active",
                        EmailVerified = true,
                        CreatedAt = DateTime.Now.AddDays(-30),
                        UpdatedAt = DateTime.Now.AddDays(-30)
                    },
                    new User
                    {
                        RoleId = 3,
                        Email = "somchai@gmail.com",
                        PasswordHash = "user123",
                        FirstName = "สมชาย",
                        LastName = "ใจดี",
                        Phone = "089-876-5432",
                        Status = "active",
                        EmailVerified = true,
                        CreatedAt = DateTime.Now.AddDays(-15),
                        UpdatedAt = DateTime.Now.AddDays(-15)
                    },
                    new User
                    {
                        RoleId = 2,
                        Email = "waraporn@hotel.com",
                        PasswordHash = "staff123",
                        FirstName = "วราภรณ์",
                        LastName = "สุขสวัสดิ์",
                        Phone = "082-111-2233",
                        Status = "active",
                        EmailVerified = true,
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UpdatedAt = DateTime.Now.AddDays(-10)
                    },
                    new User
                    {
                        RoleId = 3,
                        Email = "kittisak@example.com",
                        PasswordHash = "user123",
                        FirstName = "กิตติศักดิ์",
                        LastName = "มั่งมี",
                        Phone = "085-444-9988",
                        Status = "inactive",
                        EmailVerified = true,
                        CreatedAt = DateTime.Now.AddDays(-5),
                        UpdatedAt = DateTime.Now.AddDays(-5)
                    },
                    new User
                    {
                        RoleId = 1,
                        Email = "nawarut@jojo.com",
                        PasswordHash = "password123",
                        FirstName = "ณวรัตน์",
                        LastName = "อินมีศรี",
                        Phone = "086-555-7789",
                        Status = "active",
                        EmailVerified = true,
                        CreatedAt = DateTime.Now.AddDays(-2),
                        UpdatedAt = DateTime.Now.AddDays(-2)
                    }
                );
                _db.SaveChanges();
            }
        }
        catch
        {
            // Fallback gracefully if database table is already filled or locked
        }
    }
}
