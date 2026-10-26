// 📁 Controllers/FindTutorController.cs
// লোকেশন: Tutorbub/Controllers/FindTutorController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tutorbub.Controllers
{
    public class FindTutorController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IWebHostEnvironment _env;

        public FindTutorController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _dbHelper = new DatabaseHelper(configuration);
            _env = env;
        }

        // ============================================================
        // ===== মেইন পেজ — Approved Tutors এবং Teach Posts =====
        // ============================================================
        public IActionResult Index()
        {
            List<TutorProfile> tutors = new List<TutorProfile>();

            try
            {
                tutors = _dbHelper.GetApprovedTutors();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FindTutorController.Index - Error: " + ex.Message);
                tutors = new List<TutorProfile>();
            }

            ViewBag.TutorCount = tutors.Count;

            return View("FindTutor", tutors);
        }

        // ============================================================
        // ===== API: নির্দিষ্ট টিউটর =====
        // ============================================================
        [HttpGet]
        public IActionResult GetTutor(int id)
        {
            try
            {
                var tutors = _dbHelper.GetApprovedTutors();
                var tutor = tutors.FirstOrDefault(t => t.Id == id);
                if (tutor == null)
                    return Json(new { success = false, message = "Tutor not found" });
                return Json(new { success = true, tutor });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ===== API: Tutor ID দিয়ে খোঁজা =====
        // ============================================================
        [HttpGet]
        public IActionResult GetTutorByTutorId(string tutorId)
        {
            if (string.IsNullOrWhiteSpace(tutorId))
                return Json(new { success = false, message = "Tutor ID is required" });

            try
            {
                var tutors = _dbHelper.GetApprovedTutors();
                var tutor = tutors.FirstOrDefault(t =>
                    t.TutorId.Equals(tutorId, StringComparison.OrdinalIgnoreCase));
                if (tutor == null)
                    return Json(new { success = false, message = "Tutor not found with this ID" });
                return Json(new { success = true, tutorId = tutor.TutorId, fullName = tutor.FullName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ===== ✅ API: টিচারের প্রোফাইল ডাটা (পোস্ট থেকে ক্লিক করলে) =====
        // ============================================================
        [HttpGet]
        public IActionResult GetTeacherProfile(int userId)
        {
            try
            {
                var user = _dbHelper.GetUserById(userId);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                return Json(new
                {
                    success = true,
                    user = new
                    {
                        user.Id,
                        user.FullName,
                        user.Email,
                        user.MobileNumber,
                        user.Gender,
                        user.AgeRange,
                        user.EducationLevel,
                        user.ExamDegreeTitle,
                        user.InstitutionName,
                        user.PassingYear,
                        user.Country,
                        user.StreetAddress,
                        user.PermanentAddress,
                        user.YearsOfExperience,
                        user.CvLink,
                        user.GithubProfile,
                        user.PortfolioLink,
                        user.LinkedInProfile,
                        user.ProfileImageLink
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ===== SUBMIT TUTOR REQUEST (Student → Admin/Teacher) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitTutorRequest(
            string? tutorId,
            string? tutorName,
            string studentName,
            string phoneNumber,
            string? whatsAppNumber,
            string? email,
            string? studentClass,
            string? requiredSubject,
            string? preferredArea,
            string? teachingMode,
            string? preferredDays,
            string? preferredTime,
            string? monthlyBudget,
            string? additionalNotes,
            string requestType,
            string? requestId)
        {
            try
            {
                // ===== Validation =====
                if (string.IsNullOrWhiteSpace(studentName))
                    return Json(new { success = false, message = "Student name is required." });

                if (string.IsNullOrWhiteSpace(phoneNumber))
                    return Json(new { success = false, message = "Phone number is required." });

                var phoneClean = phoneNumber.Trim();
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^01[3-9]\d{8}$");
                if (!phoneRegex.IsMatch(phoneClean))
                    return Json(new { success = false, message = "Please enter a valid Bangladeshi phone number (e.g., 01712345678)." });

                var waClean = string.IsNullOrWhiteSpace(whatsAppNumber)
                    ? phoneClean
                    : whatsAppNumber.Trim();

                if (!phoneRegex.IsMatch(waClean))
                    return Json(new { success = false, message = "Please enter a valid WhatsApp number (e.g., 01712345678)." });

                // ===== Build Model =====
                var req = new TutorRequest
                {
                    TutorId = string.IsNullOrWhiteSpace(tutorId) ? null : tutorId.Trim(),
                    TutorName = string.IsNullOrWhiteSpace(tutorName) ? null : tutorName.Trim(),
                    StudentName = studentName.Trim(),
                    PhoneNumber = phoneClean,
                    WhatsAppNumber = waClean,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                    StudentClass = string.IsNullOrWhiteSpace(studentClass) ? null : studentClass.Trim(),
                    RequiredSubject = string.IsNullOrWhiteSpace(requiredSubject) ? null : requiredSubject.Trim(),
                    PreferredArea = string.IsNullOrWhiteSpace(preferredArea) ? null : preferredArea.Trim(),
                    TeachingMode = string.IsNullOrWhiteSpace(teachingMode) ? null : teachingMode.Trim(),
                    PreferredDays = string.IsNullOrWhiteSpace(preferredDays) ? null : preferredDays.Trim(),
                    PreferredTime = string.IsNullOrWhiteSpace(preferredTime) ? null : preferredTime.Trim(),
                    MonthlyBudget = string.IsNullOrWhiteSpace(monthlyBudget) ? null : monthlyBudget.Trim(),
                    AdditionalNotes = string.IsNullOrWhiteSpace(additionalNotes) ? null : additionalNotes.Trim(),
                    RequestType = string.IsNullOrWhiteSpace(requestType) ? "SpecificTutor" : requestType.Trim(),
                    RequestId = string.IsNullOrWhiteSpace(requestId)
                        ? GenerateRequestId()
                        : requestId.Trim(),
                    Status = "Pending"
                };

                // ===== Save to DB =====
                if (_dbHelper.CreateTutorRequest(req, out string? dbError))
                {
                    // ===== Notify All Admins =====
                    try
                    {
                        var admins = _dbHelper.GetAllUsers()
                            .Where(u => u.Role == "Admin")
                            .ToList();

                        string typeLabel = req.RequestType switch
                        {
                            "SpecificTutor" => "Tutor Request",
                            "NeedTutor" => "New Tutor Request",
                            "AreaTuition" => "Area Tuition Request",
                            _ => "Tutor Request"
                        };

                        string title = $"📩 {typeLabel}";
                        string message = $"{req.StudentName} ({req.PhoneNumber}) submitted a request.";

                        if (!string.IsNullOrEmpty(req.TutorName))
                            message += $" Tutor: {req.TutorName}.";
                        if (!string.IsNullOrEmpty(req.RequiredSubject))
                            message += $" Subject: {req.RequiredSubject}.";

                        foreach (var admin in admins)
                        {
                            _dbHelper.CreateNotification(
                                admin.Id,
                                title,
                                message,
                                "info",
                                "/Admin/TutorRequests",
                                "fa-user-plus"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Admin notification error (non-fatal): " + ex.Message);
                    }

                    // ===== ✅ নতুন: Notify the Specific Teacher (Direct Request) =====
                    try
                    {
                        if (!string.IsNullOrEmpty(req.TutorId))
                        {
                            var allUsers = _dbHelper.GetAllUsers();
                            User? targetTeacher = null;

                            // TutorId (TCH-1001) থেকে রিয়েল ইউজার বের করা
                            if (req.TutorId.StartsWith("TCH-"))
                            {
                                var parts = req.TutorId.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[1], out int generatedId))
                                {
                                    // GetApprovedTutors() এর লজিক অনুযায়ী TutorId = "TCH-" + (1000 + requestId)
                                    // তাই requestId = generatedId - 1000
                                    int teacherRequestId = generatedId - 1000;

                                    // এই requestId এর জন্য কোন ইউজারটি ছিলেন সেটি বের করি
                                    var allRequests = _dbHelper.GetAllTeacherRequests();
                                    var teacherReq = allRequests.FirstOrDefault(r => r.Id == teacherRequestId);

                                    if (teacherReq != null)
                                    {
                                        targetTeacher = allUsers.FirstOrDefault(u => u.Id == teacherReq.UserId);
                                    }
                                }
                            }

                            // যদি টিউটর পাওয়া যায়, তবে তাকে নোটিফাই করি
                            if (targetTeacher != null)
                            {
                                _dbHelper.CreateNotification(
                                    targetTeacher.Id,
                                    "📩 New Tutor Request!",
                                    $"{req.StudentName} ({req.PhoneNumber}) wants to contact you for {req.RequiredSubject} tuition.",
                                    "info",
                                    "/Teacher/TeacherDashboard?section=direct-requests",
                                    "fa-envelope-open-text"
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Teacher notification error (non-fatal): " + ex.Message);
                    }

                    // ===== Success Response =====
                    return Json(new
                    {
                        success = true,
                        message = "Request submitted successfully! Our admin team will contact you soon.",
                        requestId = req.RequestId,
                        id = req.Id
                    });
                }

                return Json(new
                {
                    success = false,
                    message = string.IsNullOrWhiteSpace(dbError)
                        ? "Failed to submit request. Please try again."
                        : $"Failed to submit request: {dbError}"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("SubmitTutorRequest error: " + ex.Message);
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        // ============================================================
        // ===== TEACH POSTS (পড়াতে চাই) =====
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeachPost(string content, IFormFile? imageFile)
        {
            if (HttpContext.Session.GetString("UserRole") != "Teacher")
                return Json(new { success = false, message = "Only teachers can post." });

            if (string.IsNullOrWhiteSpace(content) && imageFile == null)
                return Json(new { success = false, message = "Content or image is required." });

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return Json(new { success = false, message = "Invalid user." });

            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext) || imageFile.Length > 5 * 1024 * 1024)
                    return Json(new { success = false, message = "Invalid image or size over 5MB." });

                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "teach-posts");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var fileName = $"post_{DateTime.Now.Ticks}{ext}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                imageUrl = $"/uploads/teach-posts/{fileName}";
            }

            var post = new Tutorbub.Models.TeachWantedPost
            {
                UserId = userId,
                Content = content?.Trim() ?? "",
                ImageUrl = imageUrl
            };

            if (_dbHelper.CreateTeachWantedPost(post, out string? error))
                return Json(new { success = true });

            return Json(new { success = false, message = error });
        }

        // ✅ পোস্ট এডিট
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeachPost(int postId, string? content, IFormFile? imageFile, bool removeImage = false)
        {
            if (!int.TryParse(HttpContext.Session.GetString("UserId"), out int userId))
                return Json(new { success = false, message = "Please login." });

            content = content?.Trim() ?? "";

            bool imageChanged = false;
            string? newImageUrl = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext) || imageFile.Length > 5 * 1024 * 1024)
                    return Json(new { success = false, message = "Invalid image or size over 5MB." });

                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "teach-posts");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var fileName = $"post_{DateTime.Now.Ticks}{ext}";
                using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                newImageUrl = $"/uploads/teach-posts/{fileName}";
                imageChanged = true;
            }
            else if (removeImage)
            {
                imageChanged = true; // newImageUrl = null  → ছবি মুছে যাবে
            }

            if (_dbHelper.UpdateTeachWantedPost(postId, userId, content, imageChanged, newImageUrl,
                    out string? oldImageUrl, out string? finalImageUrl, out string? error))
            {
                // ছবি বদলালে/মুছলে পুরোনো ফাইল ডিস্ক থেকে মুছে ফেলা
                if (imageChanged) DeleteTeachImageFile(oldImageUrl);

                return Json(new { success = true, content = content, imageUrl = finalImageUrl ?? "" });
            }

            // ডাটাবেসে সেভ না হলে নতুন আপলোড করা ফাইল ফেলে দেওয়া
            if (newImageUrl != null) DeleteTeachImageFile(newImageUrl);
            return Json(new { success = false, message = error ?? "Failed to update post." });
        }

        // ✅ পোস্ট ডিলিট
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTeachPost(int postId)
        {
            if (!int.TryParse(HttpContext.Session.GetString("UserId"), out int userId))
                return Json(new { success = false, message = "Please login." });

            bool isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

            if (_dbHelper.DeleteTeachWantedPost(postId, userId, isAdmin, out string? imageUrl, out string? error))
            {
                DeleteTeachImageFile(imageUrl);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = error ?? "Failed to delete post." });
        }

        private void DeleteTeachImageFile(string? imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith("/uploads/teach-posts/")) return;
                var fileName = Path.GetFileName(imageUrl);
                var path = Path.Combine(_env.WebRootPath, "uploads", "teach-posts", fileName);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTeachImageFile error: " + ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTeachComment(int postId, string content, int? parentId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Please login to comment." });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Comment cannot be empty." });

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return Json(new { success = false, message = "Invalid user." });

            content = content.Trim();
            if (content.Length > 2000)
                return Json(new { success = false, message = "Comment is too long." });

            string senderName = HttpContext.Session.GetString("UserFullName")
                                ?? HttpContext.Session.GetString("UserName")
                                ?? "Someone";

            // ✅ Facebook-style: রিপ্লাইয়ের রিপ্লাই হলেও ParentId হবে সবচেয়ে উপরের মেইন কমেন্ট
            int? rootParentId = null;
            Tutorbub.Models.TeachWantedComment? repliedTo = null;

            if (parentId.HasValue)
            {
                var existing = _dbHelper.GetTeachWantedComments(postId);
                foreach (var top in existing)
                {
                    if (top.Id == parentId.Value)
                    {
                        repliedTo = top; rootParentId = top.Id; break;
                    }
                    var nested = top.Replies.FirstOrDefault(r => r.Id == parentId.Value);
                    if (nested != null)
                    {
                        repliedTo = nested; rootParentId = top.Id; break;
                    }
                }

                if (repliedTo == null)
                    return Json(new { success = false, message = "The comment you are replying to no longer exists." });
            }

            var comment = new Tutorbub.Models.TeachWantedComment
            {
                PostId = postId,
                UserId = userId,
                Content = content,
                ParentId = rootParentId
            };

            if (_dbHelper.AddTeachWantedComment(comment, out string? error))
            {
                try
                {
                    var post = _dbHelper.GetAllTeachWantedPosts().FirstOrDefault(p => p.Id == postId);
                    if (post != null)
                    {
                        // "@[Name] " মেনশন অংশটা নোটিফিকেশন থেকে বাদ
                        string preview = System.Text.RegularExpressions.Regex.Replace(content, @"^@\[[^\]]+\]\s*", "");
                        string notifMsg = $"{senderName} commented: {preview}";

                        // ১. পোস্ট ওনারকে নোটিফাই
                        if (post.UserId != userId)
                        {
                            _dbHelper.CreateNotification(
                                post.UserId, "💬 New Comment", notifMsg, "info", "/FindTutor", "fa-comment"
                            );
                        }

                        // ২. যাকে রিপ্লাই দেওয়া হয়েছে তাকে নোটিফাই
                        if (repliedTo != null && repliedTo.UserId != userId && repliedTo.UserId != post.UserId)
                        {
                            _dbHelper.CreateNotification(
                                repliedTo.UserId, "↩️ New Reply", notifMsg, "info", "/FindTutor", "fa-reply"
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Notification error: " + ex.Message);
                }

                // ✅ পেজ রিলোড ছাড়াই UI-তে দেখানোর জন্য নতুন কমেন্টের ডাটা ফেরত
                return Json(new
                {
                    success = true,
                    comment = new
                    {
                        id = comment.Id,
                        postId = postId,
                        parentId = rootParentId,
                        userFullName = senderName,
                        userProfileImage = HttpContext.Session.GetString("ProfileImageLink") ?? "",
                        content = comment.Content,
                        createdAtUtc = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                        likeCount = 0,
                        isLiked = false
                    }
                });
            }

            return Json(new { success = false, message = error });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleTeachLike(int postId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return Json(new { success = false, message = "Please login to like." });

            var result = _dbHelper.ToggleTeachWantedLike(postId, userId);
            return Json(new { success = true, isLiked = result.isLiked, count = result.newCount });
        }

        // ✅ কমেন্টে Like / Unlike
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleTeachCommentLike(int commentId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return Json(new { success = false, message = "Please login to like." });

            var result = _dbHelper.ToggleTeachCommentLike(commentId, userId);
            return Json(new { success = true, isLiked = result.isLiked, count = result.count });
        }

        // ============================================================
        // ===== Helper: Generate Request ID =====
        // ============================================================
        private static string GenerateRequestId()
        {
            var d = DateTime.Now;
            var datePart = d.Year.ToString()
                + d.Month.ToString("D2")
                + d.Day.ToString("D2");

            var random = new Random();
            var seq = random.Next(100, 999);

            return $"REQ-{datePart}-{seq}";
        }
    }
}