using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nawarut_8250_L005.Models;

namespace Nawarut_8250_L005.Controllers;

public class HomeController : Controller
{
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
        // if(sum > 10)
        // {
        //     ViewBag.Result = "True";
        // }
        // else if(sum>0)
        // {
        //     ViewBag.Result = "True2";
        // }
        // else
        // {
        //     ViewBag.Result = "false";
        // }

        return View();
    }

    public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

}

