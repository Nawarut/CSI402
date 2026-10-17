using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nawarut_8250_L005.Models;
using Nawarut_8250_L005.ViewModels;
using LoginViewModel = Nawarut_8250_L005.ViewModels.LoginViewModel;
using RegisterViewModel = Nawarut_8250_L005.ViewModels.RegisterViewModel;
using Nawarut_8250_L005.Entities;
using DbUser = Nawarut_8250_L005.Entities.User;


namespace Nawarut_8250_L005.Controllers;

public class HomeController : Controller
{
    private readonly Csi402dbContext _db;

    public HomeController(Csi402dbContext db)
    {
        _db = db;
    }

    public IActionResult Lab9()
    {
        //Create || Insert Value in SQL
        return View();
    }

    [HttpPost]
    public IActionResult Lab9(Labstudent data)
    {
        if(data.StdPassword != null)
        {
            if(ModelState.IsValid)
            {
                var CheckStd = (from a in _db.Labstudents where a.StdId == data.StdId select a).FirstOrDefault();
                // Select * from Labstudens where StdId == "@StdId"
                if(CheckStd == null)
                {
                    //Create || Insert Value in SQL
                    Labstudent s = new();
                    // Table2 a = new();
                    s.StdId = data.StdId;
                    // a.Code = data.StdId;
                    s.StdPassword = data.StdPassword;
                    s.StdName = data.StdName;
                    s.StdLastname = data.StdLastname;
                    _db.Add(s);
                    // _db.Add(a);
                    _db.SaveChanges();
                    return View(data);
                }
                else
                {
                    ViewBag.Status = "StdCode ซ้ำ";
                    return View(data);
                }
            }
            else
            {
                ViewBag.Status = "Error";
                return View(data);
            }
        }
        else
        {
            ViewBag.Status = "Password Null";
            return View(data);
        }
    }

    public IActionResult Lab9_1()
    {
        var Student = (from s in _db.Labstudents select s).ToList();
        // Select * From Labstudent
        return View(Student);
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Lab2()
    {
        string Name,Lastname,Room,Years,Language;
        Name="Nawarut";
        Lastname="Inmeesri";
        Room= "005";
        Years= "3";
        Language= "Html";
        ViewBag.Name = Name;
        ViewBag.Lastname = Lastname;
        ViewBag.Room = Room;
        ViewBag.Years = Years;
        ViewBag.Language = Language;
        int q,w,e,r,t,a,u,i,o,p,sum;
        q = 8;
        w = 7;
        e = 5;
        r = 4;
        t = 2;
        a = 7;
        u = 5;
        i = 3;
        o = 4;
        p = 1;
        ViewBag.q = q;
        ViewBag.w = w;
        ViewBag.e = e;
        ViewBag.r = r;
        ViewBag.t = t;
        ViewBag.a = a;
        ViewBag.u = u;
        ViewBag.i = i;
        ViewBag.o = o;
        ViewBag.p = p;
        sum = q+w+e+r+t+a+u+i+o+p;
        ViewBag.sum = sum;
        if (sum>80)
        {
            ViewBag.Result = "A";
        }
         if (sum>76)
        {
            ViewBag.Result = "B+";
        }
         if (sum>70)
        {
            ViewBag.Result = "B";
        }
         if (sum>66)
        {
            ViewBag.Result = "C+";
        }
         if (sum>60)
        {
            ViewBag.Result = "C";
        }
         if (sum>56)
        {
            ViewBag.Result = "D+";
        }
         if (sum>50)
        {
            ViewBag.Result = "D";
        }
         if (sum<49)
        {
            ViewBag.Result = "F";
        }

        return View();
    }

    // ==========================================
    // Helper: ตรวจสอบและเตรียมข้อมูลพื้นฐาน (Roles & Initial Users)
    // ==========================================
    private void EnsureDefaultData()
    {
        try
        {
            if (!_db.Roles.Any())
            {
                _db.Roles.AddRange(
                    new Role { RoleCode = "ADMIN", RoleName = "Admin", Description = "ผู้ดูแลระบบ" },
                    new Role { RoleCode = "STAFF", RoleName = "Staff", Description = "พนักงาน" },
                    new Role { RoleCode = "ACCOUNTANT", RoleName = "Accountant", Description = "ฝ่ายบัญชี" },
                    new Role { RoleCode = "RENTER", RoleName = "Renter", Description = "ผู้เช่า" },
                    new Role { RoleCode = "GUEST", RoleName = "Guest", Description = "ผู้เยี่ยมชม" }
                );
                _db.SaveChanges();
            }

            if (!_db.Users.Any())
            {
                var adminRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "ADMIN") ?? _db.Roles.First();
                var renterRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "RENTER") ?? _db.Roles.First();
                var staffRole = _db.Roles.FirstOrDefault(r => r.RoleCode == "STAFF") ?? _db.Roles.First();

                _db.Users.AddRange(
                    new DbUser
                    {
                        FirstName = "Nawarut",
                        LastName = "Inmeesri",
                        Email = "nawarut@jojo.com",
                        Phone = "086-555-7799",
                        PasswordHash = "admin123",
                        RoleId = adminRole.RoleId,
                        Status = "active",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new DbUser
                    {
                        FirstName = "Anan",
                        LastName = "Sukjai",
                        Email = "anan@example.com",
                        Phone = "081-111-1111",
                        PasswordHash = "123456",
                        RoleId = renterRole.RoleId,
                        Status = "active",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new DbUser
                    {
                        FirstName = "Mali",
                        LastName = "Jairak",
                        Email = "mali@example.com",
                        Phone = "082-222-2222",
                        PasswordHash = "123456",
                        RoleId = staffRole.RoleId,
                        Status = "active",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                _db.SaveChanges();
            }
        }
        catch
        {
            // ป้องกันข้อผิดพลาดกรณีการเชื่อมต่อหรือข้อมูลมีอยู่แล้ว
        }
    }

    // ==========================================
    // 1. Login (Lab5) - [R] ตรวจสอบสิทธิ์ผู้ใช้จากฐานข้อมูล
    // ==========================================
    [HttpGet]
    public IActionResult Lab5()
    {
        EnsureDefaultData();
        return View();
    }

    [HttpPost]
    public IActionResult Lab5(LoginViewModel data)
    {
        if (!ModelState.IsValid)
        {
            return View(data);
        }

        EnsureDefaultData();

        // ตรวจสอบอีเมลและรหัสผ่านจากฐานข้อมูลจริงด้วย LINQ เหมือนใน Lab9
        // SELECT * FROM users WHERE email = @Email AND password_hash = @Password LIMIT 1
        var user = (from u in _db.Users
                    where u.Email == data.Email && u.PasswordHash == data.Password
                    select u).FirstOrDefault();

        if (user != null)
        {
            if (user.Status == "inactive" || user.Status == "locked")
            {
                ViewBag.Error = "บัญชีนี้ถูกระงับการใช้งาน กรุณาติดต่อผู้ดูแลระบบ";
                return View(data);
            }

            HttpContext.Session.SetInt32("UserId", (int)user.UserId);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetString("UserEmail", user.Email);

            TempData["SuccessMessage"] = $"เข้าสู่ระบบสำเร็จ! ยินดีต้อนรับคุณ {user.FirstName} {user.LastName}";
            return RedirectToAction("Lab5_3");
        }

        ViewBag.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
        return View(data);
    }

    public IActionResult Login() => RedirectToAction("Lab5");

    // ==========================================
    // 2. Register or AddUser (Lab5_2) - [C] บันทึกข้อมูลลงฐานข้อมูล
    // ==========================================
    [HttpGet]
    public IActionResult Lab5_2()
    {
        EnsureDefaultData();
        return View();
    }

    [HttpPost]
    public IActionResult Lab5_2(RegisterViewModel data)
    {
        if (data.Password != data.ConfirmPassword)
        {
            ViewBag.Error = "รหัสผ่านไม่ตรงกัน";
            return View(data);
        }

        if (!ModelState.IsValid)
        {
            return View(data);
        }

        EnsureDefaultData();

        // 1. ตรวจสอบว่า Email ซ้ำในฐานข้อมูลหรือไม่ ด้วย LINQ (Check Email เหมือน CheckStd ใน Lab9)
        // SELECT * FROM users WHERE email = @Email LIMIT 1
        var checkUser = (from u in _db.Users
                         where u.Email == data.Email
                         select u).FirstOrDefault();

        if (checkUser != null)
        {
            ViewBag.Error = "อีเมลนี้มีผู้ใช้งานแล้วในระบบ";
            return View(data);
        }

        // ค้นหา Role จากตาราง roles
        var role = (from r in _db.Roles
                    where r.RoleCode == data.Role || r.RoleName == data.Role
                    select r).FirstOrDefault() ?? _db.Roles.FirstOrDefault();

        // 2. สร้าง Entity DbUser ใหม่ และกำหนดค่า (Insert Value in SQL)
        DbUser newUser = new()
        {
            FirstName = data.Firstname ?? "",
            LastName = data.Lastname ?? "",
            Email = data.Email ?? "",
            Phone = data.Phone,
            PasswordHash = data.Password ?? "",
            Status = "active",
            RoleId = role != null ? role.RoleId : (byte)1,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // 3. บันทึกลงฐานข้อมูลด้วย EF Core เหมือนใน Lab9 (_db.Add & _db.SaveChanges)
        _db.Users.Add(newUser);
        _db.SaveChanges();

        TempData["SuccessMessage"] = $"สมัครสมาชิก/เพิ่มผู้ใช้ \"{newUser.FirstName} {newUser.LastName}\" สำเร็จเรียบร้อยแล้ว";
        return RedirectToAction("Lab5_3");
    }

    public IActionResult Register() => RedirectToAction("Lab5_2");

    // ==========================================
    // 3. UserList (Lab5_3) - [R] ดึงข้อมูลผู้ใช้ทั้งหมดจากฐานข้อมูล
    // ==========================================
    [HttpGet]
    public IActionResult Lab5_3()
    {
        EnsureDefaultData();

        // ดึงข้อมูลผู้ใช้ทั้งหมดจากฐานข้อมูลจริงด้วย LINQ เหมือนใน Lab9_1
        // SELECT * FROM users JOIN roles ON users.role_id = roles.role_id ORDER BY user_id DESC
        var usersFromDb = (from u in _db.Users.Include(u => u.Role)
                           orderby u.UserId descending
                           select u).ToList();

        var model = new UserListViewModel
        {
            Users = usersFromDb.Select(u => new UserViewModel
            {
                UserId = u.UserId,
                Firstname = u.FirstName,
                Lastname = u.LastName,
                Email = u.Email,
                Phone = u.Phone ?? "-",
                Role = u.Role != null ? u.Role.RoleName : "User",
                Status = u.Status == "active" ? "Active" : "Suspended",
                SlotNo = "-"
            }).ToList()
        };

        return View(model);
    }

    public IActionResult UserList() => RedirectToAction("Lab5_3");

    // ==========================================
    // 4. DeleteUser - [D] ลบข้อมูลผู้ใช้จากฐานข้อมูล
    // ==========================================
    [HttpPost]
    public IActionResult DeleteUser(uint id)
    {
        // ค้นหาผู้ใช้ที่ต้องการลบด้วย LINQ
        var user = (from u in _db.Users where u.UserId == id select u).FirstOrDefault();
        if (user != null)
        {
            _db.Users.Remove(user);
            _db.SaveChanges();
            TempData["SuccessMessage"] = $"ลบผู้ใช้งาน {user.FirstName} {user.LastName} สำเร็จแล้ว";
        }
        else
        {
            TempData["Error"] = "ไม่พบข้อมูลผู้ใช้ที่ต้องการลบ";
        }

        return RedirectToAction("Lab5_3");
    }

}


