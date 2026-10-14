using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nawarut_8250_L005.Models;
using Nawarut_8250_L005.ViewModels;
using LoginViewModel = Nawarut_8250_L005.ViewModels.LoginViewModel;
using RegisterViewModel = Nawarut_8250_L005.ViewModels.RegisterViewModel;
using Nawarut_8250_L005.Entities;


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
    // 1. Login (Lab5)
    // ==========================================
    [HttpGet]
    public IActionResult Lab5()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Lab5(LoginViewModel data)
    {
        if (!ModelState.IsValid)
        {
            return View(data);
        }

        // ตรวจสอบอีเมลในฐานข้อมูลจำลอง (UserListViewModel)
        var user = UserListViewModel.UsersDataList().Users
            .FirstOrDefault(u => string.Equals(u.Email, data.Email, StringComparison.OrdinalIgnoreCase));

        if (user != null || data.Email == "supphanat@2.com" || data.Email?.EndsWith("@example.com") == true)
        {
            TempData["SuccessMessage"] = "เข้าสู่ระบบสำเร็จ";
            return RedirectToAction("Lab5_3");
        }

        ViewBag.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
        return View(data);
    }

    public IActionResult Login() => RedirectToAction("Lab5");

    // ==========================================
    // 2. Register or AddUser (Lab5_2)
    // ==========================================
    [HttpGet]
    public IActionResult Lab5_2()
    {
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

        var userList = UserListViewModel.UsersDataList();
        userList.Users.Add(new UserViewModel
        {
            Firstname = data.Firstname,
            Lastname = data.Lastname,
            Email = data.Email,
            Phone = data.Phone,
            Password = data.Password,
            Role = string.IsNullOrWhiteSpace(data.Role) ? "Renter" : data.Role,
            SlotNo = string.IsNullOrWhiteSpace(data.SlotNo) ? "-" : data.SlotNo,
            Status = string.IsNullOrWhiteSpace(data.Status) ? "Active" : data.Status
        });

        return RedirectToAction("Lab5_3");
    }

    public IActionResult Register() => RedirectToAction("Lab5_2");

    // ==========================================
    // 3. Index หรืออื่นๆ / UserList (Lab5_3)
    // ==========================================
    [HttpGet]
    public IActionResult Lab5_3()
    {
        return View(UserListViewModel.UsersDataList());
    }

    public IActionResult UserList() => RedirectToAction("Lab5_3");

}


