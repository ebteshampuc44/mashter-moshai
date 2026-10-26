// 📁 Controllers/TeacherController.cs
// লোকেশন: Tutorbub/Controllers/TeacherController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class TeacherController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public TeacherController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ============================================================
        // ===== টিচার ফর্ম দেখা =====
        // ============================================================
        [HttpGet]
        public IActionResult RequestForm()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var status = _dbHelper.GetUserTeacherRequestStatus(userId);

            if (status == "Pending")
            {
                TempData["Info"] = "You have already submitted a teacher request. Please wait for admin approval.";
                return RedirectToAction("Index", "Profile");
            }
            else if (status == "Approved")
            {
                TempData["Info"] = "You are already a teacher!";
                return RedirectToAction("TeacherDashboard");
            }

            var user = _dbHelper.GetUserById(userId);
            var model = new TeacherRequest
            {
                UserId = userId,
                FullName = user?.FullName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                MobileNumber = user?.MobileNumber ?? string.Empty
            };

            return View(model);
        }

        // ============================================================
        // ===== টিচার ফর্ম সাবমিট =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestForm(TeacherRequest model)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                model.RequestDate = DateTime.UtcNow;
                model.Status = "Pending";

                bool success = _dbHelper.CreateTeacherRequest(model, out string? dbError);

                if (success)
                {
                    TempData["Success"] = "Your teacher request has been submitted successfully! Please wait for admin approval.";
                    return View(model);
                }
                else
                {
                    ViewBag.Error = string.IsNullOrWhiteSpace(dbError)
                        ? "Failed to submit request. Please try again."
                        : $"Failed to submit request. Database error: {dbError}";
                }
            }
            else
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList();

                ViewBag.Error = errors.Any()
                    ? "Please fix the following: " + string.Join(" ", errors)
                    : "Please fill in all required fields correctly.";
            }

            return View(model);
        }

        // ============================================================
        // ===== টিচার ড্যাশবোর্ড =====
        // ============================================================
        public IActionResult TeacherDashboard()
        {
            // ===== Auth check =====
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(userId);
            if (user?.Role != "Teacher")
            {
                TempData["Error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Profile");
            }

            // ============================================================
            // ===== ✅ ইউজারের সব Tuition Applications লোড =====
            // ============================================================
            List<TuitionApplication> applications = new List<TuitionApplication>();
            try
            {
                applications = _dbHelper.GetUserTuitionApplications(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TeacherDashboard - GetUserTuitionApplications error: " + ex.Message);
            }

            // ============================================================
            // ===== ✅ স্ট্যাটাস কাউন্ট =====
            // ============================================================
            var pendingCount = applications.Count(a => a.Status == "Pending");
            var shortlistedCount = applications.Count(a => a.Status == "Shortlisted");
            var selectedCount = applications.Count(a => a.Status == "Selected");
            var rejectedCount = applications.Count(a => a.Status == "Rejected");

            // ============================================================
            // ===== ✅ Teacher Dashboard Stats (মোট পোস্ট, কমেন্ট, লাইক, রেসপন্স) =====
            // ============================================================
            TeacherDashboardStats stats;
            List<TeacherPostStat> postStats;
            try
            {
                stats = _dbHelper.GetTeacherDashboardStats(userId);
                postStats = _dbHelper.GetTeacherPostStats(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TeacherDashboard - Stats error: " + ex.Message);
                stats = new TeacherDashboardStats();
                postStats = new List<TeacherPostStat>();
            }

            // ============================================================
            // ===== ✅ নতুন: টিচারের কাছে আসা Direct Tutor Requests লোড =====
            // ============================================================
            List<TutorRequest> directRequests = new List<TutorRequest>();
            try
            {
                // আপনার DatabaseHelper-এ থাকা GetTeacherDirectRequests মেথডটি কল করা হচ্ছে
                directRequests = _dbHelper.GetTeacherDirectRequests(user.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TeacherDashboard - GetTeacherDirectRequests error: " + ex.Message);
            }

            // ============================================================
            // ===== ✅ ViewModel তৈরি =====
            // ============================================================
            var model = new TeacherDashboardViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber ?? string.Empty,
                SubjectExpertise = "Not specified",
                Experience = "Not specified",
                ProfileImageLink = user.ProfileImageLink ?? string.Empty,
                Rating = 0,

                // ✅ Tuition Applications
                TuitionApplications = applications,
                TotalApplications = applications.Count,
                PendingApplications = pendingCount,
                ShortlistedApplications = shortlistedCount,
                SelectedApplications = selectedCount,
                RejectedApplications = rejectedCount,

                // ✅ নতুন: Teacher Post Stats
                Stats = stats,
                PostStats = postStats,

                // ✅ নতুন: Direct Tutor Requests
                TutorRequests = directRequests
            };

            return View(model);
        }
    }
}