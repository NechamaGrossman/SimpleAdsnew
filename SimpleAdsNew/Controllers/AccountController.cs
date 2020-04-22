using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsNew.Data;

namespace SimpleAdsNew.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAds;Integrated Security=true;";

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User u, string Password)
        {
            UserDb userDB = new UserDb(_connectionString);
            userDB.AddUser(u, Password);
            return Redirect("/Home/Index");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            UserDb userDB = new UserDb(_connectionString);
            User user = userDB.Login(Email, Password);
            if (user == null)
            {
                return Redirect("Account/Login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", user.Email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/Home/NewAd");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/Home/Index");
        }
    }
}