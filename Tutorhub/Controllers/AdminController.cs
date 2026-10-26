// 📁 Controllers/AdminController.cs
// লোকেশন: Tutorbub/Controllers/AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutorbub.Data;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorbub.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context)
        {
            _dbHelper = new DatabaseHelper(configuration);
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // ============================================================
        // ===== DASHBOARD =====
        // ============================================================
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var users = _dbHelper.GetAllUsers();
            var requests = _dbHelper.GetAllTeacherRequests();
            var pendingCount = requests.Count(r => r.Status == "Pending");
            var tutorReqStats = _dbHelper.GetTutorRequestStats();

            ViewBag.UserCount = users.Count;
            ViewBag.PendingCount = pendingCount;
            ViewBag.PendingTutorRequests = tutorReqStats.Pending;

            return View(users);
        }

        // ============================================================
        // ===== USER MANAGEMENT =====
        // ============================================================

        [HttpGet]
        public IActionResult UserDetails(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = _dbHelper.GetUserById(id);
            if (user != null)
            {
                return Json(new
                {
                    success = true,
                    user = new
                    {
                        user.Id,
                        user.UserName,
                        user.FullName,
                        user.Email,
                        user.Role,
                        user.IsActive,
                        user.MobileNumber,
                        user.Gender,
                        user.AgeRange,
                        user.PrimaryDeviceType,
                        user.YearsOfExperience,
                        user.AreaType,
                        user.Country,
                        user.StreetAddress,
                        user.PermanentAddress,
                        user.EducationLevel,
                        user.CurrentStudyStatus,
                        user.ExamDegreeTitle,
                        user.InstitutionName,
                        user.PassingYear,
                        user.IsCSEStudent,
                        user.CvLink,
                        user.GithubProfile,
                        user.PortfolioLink,
                        user.LinkedInProfile,
                        user.ProfileImageLink,
                        CreatedAt = user.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                        LastLoginAt = user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm") ?? "Never"
                    }
                });
            }
            return Json(new { success = false, message = "User not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var currentUserId = HttpContext.Session.GetString("UserId");
            if (currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "Cannot delete your own account" });
            }

            if (_dbHelper.DeleteUser(id))
            {
                return Json(new { success = true, message = "User deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete user" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleUserStatus(int id, bool isActive)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var currentUserId = HttpContext.Session.GetString("UserId");
            if (currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "Cannot disable your own account" });
            }

            if (_dbHelper.ToggleUserStatus(id, isActive))
            {
                if (isActive)
                {
                    _dbHelper.CreateNotification(
                        id,
                        "✅ Account Activated",
                        "Your account has been activated. You can now log in.",
                        "success",
                        null,
                        "fa-check-circle"
                    );
                }
                else
                {
                    _dbHelper.CreateNotification(
                        id,
                        "⚠️ Account Deactivated",
                        "Your account has been deactivated by an administrator.",
                        "warning",
                        null,
                        "fa-exclamation-triangle"
                    );
                }

                return Json(new { success = true, message = "User status updated successfully" });
            }
            return Json(new { success = false, message = "Failed to update user status" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(int id, string newPassword)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                return Json(new { success = false, message = "Password must be at least 6 characters" });
            }

            if (_dbHelper.UpdatePassword(id, newPassword))
            {
                _dbHelper.CreateNotification(
                    id,
                    "🔑 Password Changed",
                    "Your account password was changed by an administrator.",
                    "warning",
                    null,
                    "fa-key"
                );

                return Json(new { success = true, message = "Password updated successfully" });
            }
            return Json(new { success = false, message = "Failed to update password" });
        }

        // ============================================================
        // ===== TEACHER REQUESTS =====
        // ============================================================

        public IActionResult TeacherRequests()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var requests = _dbHelper.GetAllTeacherRequests();
            var users = _dbHelper.GetAllUsers();

            ViewBag.UserCount = users.Count;
            ViewBag.PendingCount = requests.Count(r => r.Status == "Pending");

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessTeacherRequest(int id, string action, string? adminNote)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (action != "Approve" && action != "Reject")
            {
                return Json(new { success = false, message = "Invalid action" });
            }

            var status = action == "Approve" ? "Approved" : "Rejected";
            var request = _dbHelper.GetTeacherRequestById(id);

            if (_dbHelper.UpdateTeacherRequest(id, status, adminNote))
            {
                if (action == "Approve" && request != null)
                {
                    _dbHelper.MakeUserTeacher(request.UserId);

                    _dbHelper.CreateNotification(
                        request.UserId,
                        "🎓 You're now a Teacher!",
                        "Congratulations! Your teacher request has been approved. Access your Teacher Dashboard now.",
                        "success",
                        "/Teacher/TeacherDashboard",
                        "fa-chalkboard-teacher"
                    );
                }
                else if (action == "Reject" && request != null)
                {
                    var msg = string.IsNullOrEmpty(adminNote)
                        ? "Your teacher request was not approved. You can try again later."
                        : $"Your teacher request was not approved. Reason: {adminNote}";

                    _dbHelper.CreateNotification(
                        request.UserId,
                        "❌ Teacher Request Rejected",
                        msg,
                        "danger",
                        "/Teacher/RequestForm",
                        "fa-times-circle"
                    );
                }

                return Json(new { success = true, message = $"Teacher request {action.ToLower()}d successfully" });
            }
            return Json(new { success = false, message = "Failed to process request" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTeacherAccount(int id, int requestId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var currentUserId = HttpContext.Session.GetString("UserId");
            if (id > 0 && currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "Cannot delete your own account" });
            }

            bool userDeleted = false;
            bool requestDeleted = false;

            try
            {
                if (id > 0)
                {
                    userDeleted = _dbHelper.DeleteUser(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTeacherAccount - error deleting user: " + ex.Message);
            }

            try
            {
                requestDeleted = _dbHelper.DeleteTeacherRequestById(requestId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTeacherAccount - error deleting request: " + ex.Message);
            }

            if (userDeleted || requestDeleted)
            {
                return Json(new { success = true, message = "Teacher deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete: no matching user or request was found" });
        }

        [HttpGet]
        public IActionResult GetTeacherRequestDetails(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var request = _dbHelper.GetTeacherRequestById(id);
            if (request != null)
            {
                return Json(new
                {
                    success = true,
                    request = new
                    {
                        request.Id,
                        request.UserId,
                        request.FullName,
                        request.Email,
                        request.MobileNumber,
                        request.Education,
                        request.Institution,
                        request.SubjectExpertise,
                        request.Experience,
                        request.TeachingStyle,
                        request.AvailableDays,
                        request.PreferredTime,
                        request.HourlyRate,
                        request.CvLink,
                        request.WhyTeach,
                        request.Status,
                        RequestDate = request.RequestDate.ToString("dd MMM yyyy, HH:mm"),
                        request.AdminNote
                    }
                });
            }
            return Json(new { success = false, message = "Request not found" });
        }

        // ============================================================
        // ===== TUTOR REQUESTS (Student → Admin) =====
        // ============================================================

        [HttpGet]
        public IActionResult TutorRequests(string status = "all")
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var requests = _dbHelper.GetAllTutorRequests(status);
            var stats = _dbHelper.GetTutorRequestStats();

            ViewBag.TotalCount = stats.Total;
            ViewBag.PendingCount = stats.Pending;
            ViewBag.ContactedCount = stats.Contacted;
            ViewBag.CompletedCount = stats.Completed;
            ViewBag.CurrentStatus = status;

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTutorRequestStatus(int id, string status, string? adminNote)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(status))
                return Json(new { success = false, message = "Status is required." });

            var allowedStatuses = new[] { "Pending", "Contacted", "Completed", "Cancelled" };
            if (!allowedStatuses.Contains(status))
                return Json(new { success = false, message = "Invalid status value." });

            if (_dbHelper.UpdateTutorRequestStatus(id, status, adminNote))
                return Json(new { success = true, message = "Status updated successfully." });

            return Json(new { success = false, message = "Failed to update status." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTutorRequest(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.DeleteTutorRequest(id))
                return Json(new { success = true, message = "Request deleted successfully." });

            return Json(new { success = false, message = "Failed to delete request." });
        }

        // ============================================================
        // ===== NOTICE MANAGEMENT =====
        // ============================================================

        [HttpGet]
        public IActionResult ManageNotices()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var notices = _dbHelper.GetAllNotices();
            return View(notices);
        }

        [HttpGet]
        public IActionResult AddNotice()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            return View(new Notice
            {
                PublishedDate = DateTime.UtcNow,
                IsActive = true,
                IsAnnouncement = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNotice(Notice model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ViewBag.Error = "Title is required.";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                ViewBag.Error = "Content is required.";
                return View(model);
            }

            if (string.Equals(model.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                model.IsAnnouncement = false;
            }

            if (model.PublishedDate == default(DateTime) || model.PublishedDate.Year < 2000 || model.PublishedDate.Year > 2100)
            {
                model.PublishedDate = DateTime.UtcNow;
            }

            if (model.IsAnnouncement)
            {
                model.ImageUrl = string.Empty;
                model.IsActive = true;
            }

            model.CreatedAt = DateTime.UtcNow;

            if (_dbHelper.CreateNotice(model, out string? error))
            {
                TempData["Success"] = model.IsAnnouncement
                    ? "Announcement added successfully!"
                    : "News added successfully!";
                return RedirectToAction("ManageNotices");
            }

            ViewBag.Error = $"Failed to add notice: {error}";
            return View(model);
        }

        [HttpGet]
        public IActionResult EditNotice(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var notice = _dbHelper.GetNoticeById(id);
            if (notice == null)
            {
                TempData["Error"] = "Notice not found.";
                return RedirectToAction("ManageNotices");
            }

            return View(notice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNotice(Notice model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ViewBag.Error = "Title is required.";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                ViewBag.Error = "Content is required.";
                return View(model);
            }

            if (string.Equals(model.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                model.IsAnnouncement = false;
            }

            if (model.PublishedDate == default(DateTime) || model.PublishedDate.Year < 2000 || model.PublishedDate.Year > 2100)
            {
                model.PublishedDate = DateTime.UtcNow;
            }

            if (model.IsAnnouncement)
            {
                model.ImageUrl = string.Empty;
                model.IsActive = true;
            }

            if (_dbHelper.UpdateNotice(model, out string? error))
            {
                TempData["Success"] = model.IsAnnouncement
                    ? "Announcement updated successfully!"
                    : "News updated successfully!";
                return RedirectToAction("ManageNotices");
            }

            ViewBag.Error = $"Failed to update notice: {error}";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNotice(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.DeleteNotice(id))
                return Json(new { success = true, message = "Notice deleted successfully." });

            return Json(new { success = false, message = "Failed to delete notice." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadNoticeImage(IFormFile noticeImage)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (noticeImage == null || noticeImage.Length == 0)
            {
                return Json(new { success = false, message = "Please select an image." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(noticeImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Only JPG, PNG, GIF, or WEBP images are allowed." });
            }

            if (noticeImage.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "Image size must be less than 5MB." });
            }

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "notices");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileName = $"notice_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await noticeImage.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/notices/{fileName}";
                return Json(new { success = true, message = "Image uploaded successfully!", imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== SITE SETTINGS =====
        // ============================================================

        [HttpGet]
        public IActionResult Settings()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToAction("Login", "Account");

            var settings = _dbHelper.GetSiteSettings();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSettings(SiteSettings model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (model.SliderIntervalSeconds < 1)
                model.SliderIntervalSeconds = 5;
            if (model.SliderIntervalSeconds > 60)
                model.SliderIntervalSeconds = 60;

            // ✅ Clamp logo size (24–100)
            if (model.LogoSizePx < 24) model.LogoSizePx = 24;
            if (model.LogoSizePx > 100) model.LogoSizePx = 100;

            // ✅ Normalize logo shape
            var shape = (model.LogoShape ?? "circle").Trim().ToLowerInvariant();
            if (shape != "circle" && shape != "square" && shape != "rounded")
                shape = "circle";
            model.LogoShape = shape;

            if (_dbHelper.SaveSiteSettings(model, out string? error))
            {
                return Json(new { success = true, message = "Settings saved successfully!" });
            }

            return Json(new { success = false, message = $"Failed: {error}" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadHeroImage(IFormFile heroImage)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (heroImage == null || heroImage.Length == 0)
                return Json(new { success = false, message = "Please select an image." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(heroImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Only JPG, PNG, GIF, or WEBP images are allowed." });

            if (heroImage.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "Image size must be less than 5MB." });

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "hero");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"hero_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await heroImage.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/hero/{fileName}";
                return Json(new { success = true, message = "Image uploaded!", imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== ✅ UPLOAD TUTOR SECTION IMAGE (Home page) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadTutorSectionImage(IFormFile tutorSectionImage)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (tutorSectionImage == null || tutorSectionImage.Length == 0)
                return Json(new { success = false, message = "Please select an image." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(tutorSectionImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Only JPG, PNG, GIF, or WEBP images are allowed." });

            if (tutorSectionImage.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "Image size must be less than 5MB." });

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "tutor-section");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"tutor_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await tutorSectionImage.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/tutor-section/{fileName}";
                return Json(new { success = true, message = "Image uploaded!", imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== ✅ UPLOAD LOGO (Navbar / Footer) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLogo(IFormFile logoImage, string target = "navbar")
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (logoImage == null || logoImage.Length == 0)
                return Json(new { success = false, message = "Please select an image." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
            var extension = Path.GetExtension(logoImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Only JPG, PNG, GIF, WEBP, or SVG are allowed." });

            if (logoImage.Length > 3 * 1024 * 1024)
                return Json(new { success = false, message = "Logo size must be less than 3MB." });

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "logos");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var prefix = (target == "footer") ? "footer" : "navbar";
                var fileName = $"{prefix}_logo_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logoImage.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/logos/{fileName}";
                return Json(new { success = true, message = "Logo uploaded!", imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== TUITION CIRCULAR MANAGEMENT =====
        // ============================================================

        [HttpGet]
        public IActionResult TuitionPosts()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            try
            {
                var posts = _context.TuitionPosts
                    .Include(p => p.Applications)
                    .OrderByDescending(p => p.PostedDate)
                    .ToList();

                ViewBag.TotalPosts = posts.Count;
                ViewBag.OpenPosts = posts.Count(p => p.Status == "Open");
                ViewBag.TotalApplications = _context.TuitionApplications.Count();
                ViewBag.PendingApplications = _context.TuitionApplications.Count(a => a.Status == "Pending");

                return View(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TuitionPosts error: " + ex.Message);
                Console.WriteLine("INNER: " + ex.InnerException?.Message);
                TempData["Error"] = "Unable to load tuition posts. Details: " +
                                    (ex.InnerException?.Message ?? ex.Message);
                return View(new List<TuitionPost>());
            }
        }

        [HttpGet]
        public IActionResult AddTuitionPost()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            return View(new TuitionPost { Status = "Open", PostedDate = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTuitionPost(TuitionPost model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.PostedDate = DateTime.UtcNow;
                model.Status = string.IsNullOrWhiteSpace(model.Status) ? "Open" : model.Status;

                if (string.IsNullOrWhiteSpace(model.TuitionCode))
                {
                    var areaPart = string.IsNullOrWhiteSpace(model.Area)
                        ? "TUITION"
                        : new string(model.Area.Where(char.IsLetter).Take(8).ToArray()).ToUpper();

                    if (string.IsNullOrWhiteSpace(areaPart))
                        areaPart = "TUITION";

                    var rand = new Random().Next(1000, 9999);
                    model.TuitionCode = $"{areaPart}{rand}";
                }

                _context.TuitionPosts.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Tuition post added successfully!";
                return RedirectToAction("TuitionPosts");
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddTuitionPost ERROR: " + ex.Message);
                var innerMsg = ex.InnerException?.InnerException?.Message
                            ?? ex.InnerException?.Message
                            ?? ex.Message;

                ViewBag.Error = "Failed to add tuition post: " + innerMsg;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult EditTuitionPost(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var post = _context.TuitionPosts.Find(id);
            if (post == null)
            {
                TempData["Error"] = "Tuition post not found.";
                return RedirectToAction("TuitionPosts");
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTuitionPost(TuitionPost model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var existing = _context.TuitionPosts.Find(model.Id);
                if (existing == null)
                {
                    TempData["Error"] = "Tuition post not found.";
                    return RedirectToAction("TuitionPosts");
                }

                existing.Title = model.Title;
                existing.StudentGender = model.StudentGender;
                existing.StudentClass = model.StudentClass;
                existing.Subjects = model.Subjects;
                existing.DaysPerWeek = model.DaysPerWeek;
                existing.PreferredTime = model.PreferredTime;
                existing.Salary = model.Salary;
                existing.Area = model.Area;
                existing.MediaFee = model.MediaFee;
                existing.TuitionCode = model.TuitionCode;
                existing.SpecialNote = model.SpecialNote;
                existing.TutorGenderPreference = model.TutorGenderPreference;
                existing.Status = model.Status;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                TempData["Success"] = "Tuition post updated successfully!";
                return RedirectToAction("TuitionPosts");
            }
            catch (Exception ex)
            {
                Console.WriteLine("EditTuitionPost error: " + ex.Message);
                ViewBag.Error = "Failed to update tuition post: " +
                                (ex.InnerException?.Message ?? ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleTuitionStatus(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var post = _context.TuitionPosts.Find(id);
                if (post == null)
                    return Json(new { success = false, message = "Post not found" });

                post.Status = post.Status == "Open" ? "Closed" : "Open";
                post.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = $"Tuition marked as {post.Status}",
                    newStatus = post.Status
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ✅ NEW: Toggle Show on Home Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleShowOnHome(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var post = _context.TuitionPosts.Find(id);
                if (post == null)
                    return Json(new { success = false, message = "Post not found" });

                post.ShowOnHome = !post.ShowOnHome;
                post.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = post.ShowOnHome ? "Added to Home Page" : "Removed from Home Page",
                    isShowing = post.ShowOnHome
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTuitionPost(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var post = _context.TuitionPosts.Find(id);
                if (post == null)
                    return Json(new { success = false, message = "Post not found" });

                _context.TuitionPosts.Remove(post);
                _context.SaveChanges();

                return Json(new { success = true, message = "Tuition post deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ============================================================
        // ===== TUITION APPLICATIONS MANAGEMENT =====
        // ============================================================

        [HttpGet]
        public IActionResult TuitionApplications(string status = "all")
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            try
            {
                var query = _context.TuitionApplications
                    .Include(a => a.TuitionPost)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status) && status != "all")
                {
                    query = query.Where(a => a.Status == status);
                }

                var apps = query
                    .OrderByDescending(a => a.AppliedAt)
                    .ToList();

                ViewBag.CurrentStatus = status;
                ViewBag.TotalCount = _context.TuitionApplications.Count();
                ViewBag.PendingCount = _context.TuitionApplications.Count(a => a.Status == "Pending");
                ViewBag.ShortlistedCount = _context.TuitionApplications.Count(a => a.Status == "Shortlisted");
                ViewBag.SelectedCount = _context.TuitionApplications.Count(a => a.Status == "Selected");

                return View(apps);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TuitionApplications error: " + ex.Message);
                TempData["Error"] = "Unable to load applications. Details: " +
                                    (ex.InnerException?.Message ?? ex.Message);
                return View(new List<TuitionApplication>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateApplicationStatus(int id, string status, string? adminNote)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var allowed = new[] { "Pending", "Shortlisted", "Selected", "Rejected" };
                if (!allowed.Contains(status))
                    return Json(new { success = false, message = "Invalid status." });

                var app = _context.TuitionApplications.Find(id);
                if (app == null)
                    return Json(new { success = false, message = "Application not found." });

                app.Status = status;
                app.AdminNote = adminNote;
                app.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Json(new { success = true, message = "Application status updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTuitionApplication(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var app = _context.TuitionApplications.Find(id);
                if (app == null)
                    return Json(new { success = false, message = "Application not found." });

                _context.TuitionApplications.Remove(app);
                _context.SaveChanges();

                return Json(new { success = true, message = "Application deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}