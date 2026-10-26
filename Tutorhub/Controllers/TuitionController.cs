// 📁 Controllers/TuitionController.cs
// লোকেশন: Tutorbub/Controllers/TuitionController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutorbub.Data;
using Tutorbub.Models;
using System;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class TuitionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TuitionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // ===== TUITION CIRCULAR (Public Listing) =====
        // GET: /Tuition/Circular
        // ============================================================
        [HttpGet]
        public IActionResult Circular(string? search, string? area)
        {
            try
            {
                var query = _context.TuitionPosts
                    .Where(p => p.Status == "Open")
                    .AsQueryable();

                // ===== Search filter =====
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var q = search.ToLower().Trim();
                    query = query.Where(p =>
                        (p.Title ?? "").ToLower().Contains(q) ||
                        (p.Subjects ?? "").ToLower().Contains(q) ||
                        (p.Area ?? "").ToLower().Contains(q) ||
                        (p.StudentClass ?? "").ToLower().Contains(q) ||
                        (p.TuitionCode ?? "").ToLower().Contains(q));
                }

                // ===== Area filter =====
                if (!string.IsNullOrWhiteSpace(area) && area != "all")
                {
                    query = query.Where(p => (p.Area ?? "").ToLower().Contains(area.ToLower()));
                }

                var posts = query
                    .OrderByDescending(p => p.PostedDate)
                    .ToList();

                ViewBag.SearchQuery = search;
                ViewBag.SelectedArea = area ?? "all";

                return View(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Circular error: " + ex.Message);
                TempData["Error"] = "Unable to load tuition posts. Please try again.";
                return View(new System.Collections.Generic.List<TuitionPost>());
            }
        }

        // ============================================================
        // ===== TUITION DETAILS (Single Post) =====
        // GET: /Tuition/Details/5
        // ============================================================
        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                var post = _context.TuitionPosts
                    .Include(p => p.Applications)
                    .FirstOrDefault(p => p.Id == id);

                if (post == null)
                {
                    TempData["Error"] = "Tuition post not found.";
                    return RedirectToAction("Circular");
                }

                return View(post);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Details error: " + ex.Message);
                TempData["Error"] = "Unable to load tuition details.";
                return RedirectToAction("Circular");
            }
        }

        // ============================================================
        // ===== APPLY FOR TUITION (GET — Show Form) =====
        // GET: /Tuition/Apply/5
        // 🔒 Login Required
        // ============================================================
        [HttpGet]
        public IActionResult Apply(int id)
        {
            // ✅ Login check — না থাকলে Login পেজে পাঠাও
            if (HttpContext.Session.GetString("UserName") == null)
            {
                TempData["Error"] = "Please login to apply for this tuition.";
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Tuition/Apply/{id}" });
            }

            try
            {
                var post = _context.TuitionPosts.Find(id);
                if (post == null || post.Status != "Open")
                {
                    TempData["Error"] = "This tuition is no longer available.";
                    return RedirectToAction("Circular");
                }

                var model = new TuitionApplication
                {
                    TuitionPostId = id
                };

                // ===== Pre-fill from logged-in user =====
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        model.UserId = userId;
                        model.ApplicantName = user.FullName ?? "";
                        model.PhoneNumber = user.MobileNumber ?? "";
                        model.WhatsAppNumber = user.MobileNumber ?? "";
                        model.Email = user.Email ?? "";
                    }
                }

                ViewBag.Post = post;
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Apply GET error: " + ex.Message);
                TempData["Error"] = "Unable to load application form.";
                return RedirectToAction("Circular");
            }
        }

        // ============================================================
        // ===== APPLY FOR TUITION (POST — Save) =====
        // POST: /Tuition/Apply
        // 🔒 Login Required
        // 🔍 DEBUG MODE — Real error UI-তে দেখাবে
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(TuitionApplication model, string? postTitle)
        {
            // ✅ Login check — না থাকলে Login পেজে পাঠাও
            if (HttpContext.Session.GetString("UserName") == null)
            {
                TempData["Error"] = "Please login to submit your application.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // ============================================================
                // 🔍 DEBUG: ModelState errors check
                // ============================================================
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                    Console.WriteLine("===========================================");
                    Console.WriteLine("MODELSTATE ERRORS: " + errors);
                    Console.WriteLine("===========================================");

                    // ✅ ModelState error থাকলে UI-তে দেখাও
                    TempData["Error"] = "Validation Failed: " + errors;

                    var postForValidation = _context.TuitionPosts.Find(model.TuitionPostId);
                    ViewBag.Post = postForValidation;
                    return View(model);
                }

                // ===== Load Post =====
                var post = _context.TuitionPosts.Find(model.TuitionPostId);
                if (post == null || post.Status != "Open")
                {
                    TempData["Error"] = "This tuition is no longer available.";
                    return RedirectToAction("Circular");
                }

                // ============================================================
                // ===== Validation =====
                // ============================================================
                if (string.IsNullOrWhiteSpace(model.ApplicantName))
                    ModelState.AddModelError("ApplicantName", "Name is required.");

                if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                    ModelState.AddModelError("PhoneNumber", "Phone number is required.");

                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^01[3-9]\d{8}$");
                if (!string.IsNullOrWhiteSpace(model.PhoneNumber)
                    && !phoneRegex.IsMatch(model.PhoneNumber.Trim()))
                {
                    ModelState.AddModelError("PhoneNumber",
                        "Please enter a valid Bangladeshi phone number (e.g., 01712345678).");
                }

                if (!string.IsNullOrWhiteSpace(model.WhatsAppNumber)
                    && !phoneRegex.IsMatch(model.WhatsAppNumber.Trim()))
                {
                    ModelState.AddModelError("WhatsAppNumber",
                        "Please enter a valid WhatsApp number (e.g., 01712345678).");
                }

                if (string.IsNullOrWhiteSpace(model.CvLink))
                {
                    ModelState.AddModelError("CvLink", "CV link is required.");
                }
                else if (!model.CvLink.Trim().StartsWith("http://")
                      && !model.CvLink.Trim().StartsWith("https://"))
                {
                    ModelState.AddModelError("CvLink",
                        "Please enter a valid URL starting with http:// or https://");
                }

                // ✅ If Validation Failed → Show Form Again
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                    Console.WriteLine("MANUAL VALIDATION ERRORS: " + errors);
                    TempData["Error"] = "Please fix these errors: " + errors;

                    ViewBag.Post = post;
                    return View(model);
                }

                // ============================================================
                // ===== Prevent Duplicate Application =====
                // ============================================================
                var userIdStr = HttpContext.Session.GetString("UserId");
                int? userId = null;

                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int uid))
                {
                    userId = uid;

                    var existing = _context.TuitionApplications
                        .FirstOrDefault(a => a.TuitionPostId == model.TuitionPostId
                                          && a.UserId == userId);

                    if (existing != null)
                    {
                        TempData["Error"] = "You have already applied for this tuition.";
                        return RedirectToAction("Details", new { id = model.TuitionPostId });
                    }
                }

                // ============================================================
                // ===== Sanitize + Save =====
                // ============================================================
                model.UserId = userId;
                model.PhoneNumber = model.PhoneNumber.Trim();

                model.WhatsAppNumber = string.IsNullOrWhiteSpace(model.WhatsAppNumber)
                    ? model.PhoneNumber
                    : model.WhatsAppNumber.Trim();

                model.CvLink = model.CvLink.Trim();
                model.ApplicantName = model.ApplicantName.Trim();
                model.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
                model.CoverNote = string.IsNullOrWhiteSpace(model.CoverNote) ? null : model.CoverNote.Trim();
                model.Status = "Pending";
                model.AppliedAt = DateTime.UtcNow;

                // ============================================================
                // 🔍 DEBUG: Save করার আগে data print করি
                // ============================================================
                Console.WriteLine("===========================================");
                Console.WriteLine("SAVING APPLICATION:");
                Console.WriteLine($"  TuitionPostId: {model.TuitionPostId}");
                Console.WriteLine($"  UserId: {model.UserId}");
                Console.WriteLine($"  ApplicantName: {model.ApplicantName}");
                Console.WriteLine($"  PhoneNumber: {model.PhoneNumber}");
                Console.WriteLine($"  WhatsAppNumber: {model.WhatsAppNumber}");
                Console.WriteLine($"  Email: {model.Email ?? "(null)"}");
                Console.WriteLine($"  CvLink: {model.CvLink}");
                Console.WriteLine($"  CoverNote: {model.CoverNote ?? "(null)"}");
                Console.WriteLine($"  Status: {model.Status}");
                Console.WriteLine($"  AppliedAt: {model.AppliedAt} (Kind={model.AppliedAt.Kind})");
                Console.WriteLine("===========================================");

                _context.TuitionApplications.Add(model);
                _context.SaveChanges();

                Console.WriteLine("✅ Application saved successfully! Id=" + model.Id);

                // ============================================================
                // ===== Notify All Admins =====
                // ============================================================
                try
                {
                    var admins = _context.Users
                        .Where(u => u.Role == "Admin")
                        .ToList();

                    foreach (var admin in admins)
                    {
                        _context.Notifications.Add(new Notification
                        {
                            UserId = admin.Id,
                            Title = "📩 New Tuition Application",
                            Message = $"{model.ApplicantName} ({model.PhoneNumber}) applied for: {post.Title}",
                            Type = "info",
                            Icon = "fa-user-check",
                            Link = "/Admin/TuitionApplications",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    _context.SaveChanges();
                    Console.WriteLine($"✅ Notification sent to {admins.Count} admin(s)");
                }
                catch (Exception notifEx)
                {
                    // Non-fatal — application already saved
                    Console.WriteLine("⚠️ Admin notification error (non-fatal): " + notifEx.Message);
                }

                // ============================================================
                // ===== Redirect to Success Page =====
                // ============================================================
                TempData["Success"] = "Application submitted successfully! Admin will contact you soon.";
                return RedirectToAction("ApplySuccess", new { id = model.Id });
            }
            catch (Exception ex)
            {
                // ============================================================
                // 🔍 DEBUG: Real error UI-তে দেখাই
                // ============================================================
                Console.WriteLine("===========================================");
                Console.WriteLine("❌ APPLY POST ERROR");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Inner: " + ex.InnerException?.Message);
                Console.WriteLine("Inner-Inner: " + ex.InnerException?.InnerException?.Message);
                Console.WriteLine("Stack: " + ex.StackTrace);
                Console.WriteLine("===========================================");

                // ✅ REAL error UI-তে দেখাও — temporary debug
                var innerMsg = ex.InnerException?.InnerException?.Message
                            ?? ex.InnerException?.Message
                            ?? ex.Message;

                TempData["Error"] = "DEBUG: " + ex.Message + " | INNER: " + innerMsg;

                var post = _context.TuitionPosts.Find(model.TuitionPostId);
                ViewBag.Post = post;
                return View(model);
            }
        }

        // ============================================================
        // ===== APPLY SUCCESS PAGE =====
        // GET: /Tuition/ApplySuccess/12
        // ============================================================
        [HttpGet]
        public IActionResult ApplySuccess(int id)
        {
            try
            {
                var app = _context.TuitionApplications
                    .Include(a => a.TuitionPost)
                    .FirstOrDefault(a => a.Id == id);

                if (app == null)
                {
                    TempData["Error"] = "Application not found.";
                    return RedirectToAction("Circular");
                }

                return View(app);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ApplySuccess error: " + ex.Message);
                return RedirectToAction("Circular");
            }
        }
    }
}