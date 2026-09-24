using System;
using System.Collections.Generic;
using System.Linq;
using Nawarut_8250_L005.Models;

namespace Nawarut_8250_L005.Services;

public interface IUserService
{
    List<User> GetAll();
    User? GetById(int id);
    User? GetByEmail(string email);
    User? Authenticate(string email, string password);
    bool Add(User user);
    bool Delete(int id);
}

public class UserService : IUserService
{
    private static readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            FullName = "Administrator (ผู้ดูแลระบบ)",
            Email = "admin@jojo.com",
            PhoneNumber = "081-234-5678",
            Role = "Admin",
            Status = "Active",
            Password = "admin123",
            CreatedAt = DateTime.Now.AddDays(-30)
        },
        new User
        {
            Id = 2,
            FullName = "สมชาย ใจดี",
            Email = "somchai@gmail.com",
            PhoneNumber = "089-876-5432",
            Role = "User",
            Status = "Active",
            Password = "user123",
            CreatedAt = DateTime.Now.AddDays(-15)
        },
        new User
        {
            Id = 3,
            FullName = "วราภรณ์ สุขสวัสดิ์",
            Email = "waraporn@hotel.com",
            PhoneNumber = "082-111-2233",
            Role = "Staff",
            Status = "Active",
            Password = "staff123",
            CreatedAt = DateTime.Now.AddDays(-10)
        },
        new User
        {
            Id = 4,
            FullName = "กิตติศักดิ์ มั่งมี",
            Email = "kittisak@example.com",
            PhoneNumber = "085-444-9988",
            Role = "User",
            Status = "Inactive",
            Password = "user123",
            CreatedAt = DateTime.Now.AddDays(-5)
        },
        new User
        {
            Id = 5,
            FullName = "ณวรัตน์ อินมีศรี",
            Email = "nawarut@jojo.com",
            PhoneNumber = "086-555-7799",
            Role = "Admin",
            Status = "Active",
            Password = "admin123",
            CreatedAt = DateTime.Now.AddDays(-2)
        }
    };

    private static int _nextId = 6;
    private static readonly object _lock = new();

    public List<User> GetAll()
    {
        lock (_lock)
        {
            return _users.OrderByDescending(u => u.Id).ToList();
        }
    }

    public User? GetById(int id)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
    }

    public User? GetByEmail(string email)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }

    public User? Authenticate(string email, string password)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password);
        }
    }

    public bool Add(User user)
    {
        lock (_lock)
        {
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Email already exists
            }

            user.Id = _nextId++;
            if (user.CreatedAt == default)
            {
                user.CreatedAt = DateTime.Now;
            }

            _users.Add(user);
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _users.Remove(user);
                return true;
            }
            return false;
        }
    }
}
