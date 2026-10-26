// 📁 Controllers/HomeController.cs
// লোকেশন: Tutorbub/Controllers/HomeController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;

namespace Tutorhub.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public HomeController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== হোম পেইজ =====
        public IActionResult Index()
        {
            // ✅ সাইট সেটিংস এবং হোম পেইজের টিউশন পোস্ট লোড করা
            try
            {
                var settings = _dbHelper.GetSiteSettings();
                ViewBag.SiteSettings = settings;

                // ✅ হোম পেইজে দেখানোর জন্য সিলেক্ট করা টিউশন পোস্টগুলো লোড করা (সর্বোচ্চ ৩টি)
                var homeTuitions = _dbHelper.GetHomePageTuitionPosts(3);
                ViewBag.HomeTuitions = homeTuitions;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading home page data: " + ex.Message);
                ViewBag.SiteSettings = new SiteSettings();
                ViewBag.HomeTuitions = new List<TuitionPost>();
            }

            return View();
        }

        // ===== Privacy পেজ =====
        public IActionResult Privacy()
        {
            return View();
        }

        // ===== About পেজ =====
        public IActionResult About()
        {
            return View();
        }

        // ===== Contact পেজ =====
        public IActionResult Contact()
        {
            return View();
        }

        // ===== Error পেজ =====
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}