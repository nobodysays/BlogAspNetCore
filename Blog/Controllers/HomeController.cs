using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignIn(UserViewModel userViewModel)
        {
            var user = new User
            {
                Username = userViewModel.Username,
                Password = userViewModel.Password
            };
            var users = from u in dbContext.Users
                        where u.Username == userViewModel.Username &&
                        u.Password == userViewModel.Password
                        select u;
            if(users.Count()!=0)
            {
                HttpContext.Session.SetString("currentUser", user.Username);
                return RedirectToAction("Profile", "User");
            }
                                        
            return RedirectToAction(nameof(Index));
        }
       
        [HttpPost]
        public IActionResult SignUp(UserViewModel userViewModel)
        {
            var user = new User
            {
                Username = userViewModel.Username,
                Password = userViewModel.Password
            };
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            HttpContext.Session.SetString("currentUser", user.Username);
            return RedirectToAction("Profile", "User");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
