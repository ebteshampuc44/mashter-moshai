// 📁 Controllers/UserController.cs
// লোকেশন: Tutorbub/Controllers/UserController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;

namespace Tutorbub.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public UserController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ============================================================
        // ===== USER DASHBOARD =====
        // GET: /User/Dashboard
        // ============================================================
        public IActionResult Dashboard()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            // ✅ চেক: ইউজার লগইন করা আছে কিনা এবং সে User/Admin কিনা
            if (string.IsNullOrEmpty(userIdStr) || (userRole != "User" && userRole != "Admin"))
            {
                TempData["Error"] = "You must be logged in as a Student to access this page.";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);
            var user = _dbHelper.GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // ✅ স্ট্যাটস এবং পোস্ট লিস্ট নিয়ে আসা
            var stats = _dbHelper.GetUserDashboardStats(userId);
            var recentPosts = _dbHelper.GetUserCircularPosts(userId, 10);

            ViewBag.User = user;
            ViewBag.Stats = stats;
            ViewBag.RecentPosts = recentPosts;

            return View();
        }
    }
}