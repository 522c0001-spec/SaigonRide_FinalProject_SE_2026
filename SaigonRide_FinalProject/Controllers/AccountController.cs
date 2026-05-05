using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SaigonRide_FinalProject.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // Mock accounts
            if (email == "nguyen@example.com")
            {
                Session["UserID"] = 1; // ID from database
                Session["UserType"] = "Local";
                Session["FullName"] = "Nguyen";
                return RedirectToAction("Index", "Home");
            }
            else if (email == "john@example.com")
            {
                Session["UserID"] = 2; // ID from database
                Session["UserType"] = "Tourist";
                Session["FullName"] = "John Doe";
                return RedirectToAction("Index", "Home");
            }
            else if (email == "admin@saigonride.vn")
            {
                Session["UserID"] = 3; // ID from database
                Session["UserType"] = "Admin";
                Session["FullName"] = "System Admin";
                return RedirectToAction("Index", "Home");
            }

            // If user type wrong thing, show red error box
            TempData["ErrorMessage"] = "Invalid credentials. Try: nguyen@example.com, john@example.com, or admin@saigonride.vn";
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}