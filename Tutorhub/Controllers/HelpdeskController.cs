using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorbub.Controllers
{
    public class HelpdeskController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IWebHostEnvironment _env;

        public HelpdeskController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _dbHelper = new DatabaseHelper(configuration);
            _env = env;
        }

        // ============================================================
        // ===== MAIN PAGE =====
        // ============================================================
        [HttpGet]
        public IActionResult Index(string filter = "all", string? search = null)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Helpdesk" });
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            int.TryParse(userIdStr, out int userId);

            if (filter != "all" && filter != "mine" && filter != "admin")
                filter = "all";

            var posts = _dbHelper.GetHelpdeskPosts(filter, userId, search);

            ViewBag.CurrentFilter = filter;
            ViewBag.SearchQuery = search;
            ViewBag.CurrentUserId = userId;
            ViewBag.IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

            return View(posts);
        }

        // ============================================================
        // ===== GET POST COMMENTS (AJAX) =====
        // ============================================================
        [HttpGet]
        public IActionResult GetPostComments(int postId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var comments = _dbHelper.GetHelpdeskComments(postId);

            var result = comments.Select(c => new
            {
                c.Id,
                c.Content,
                c.MediaUrl,
                c.MediaType,
                c.IsAdminReply,
                c.CreatedAt,
                c.UserFullName,
                c.UserName,
                c.UserRole,
                c.UserProfileImage,
                c.Initials,
                TimeAgo = c.TimeAgo,
                c.ReplyCount,
                Replies = c.Replies.Select(r => new
                {
                    r.Id,
                    r.Content,
                    r.MediaUrl,
                    r.MediaType,
                    r.IsAdminReply,
                    r.CreatedAt,
                    r.UserFullName,
                    r.UserName,
                    r.UserRole,
                    r.UserProfileImage,
                    r.Initials,
                    TimeAgo = r.TimeAgo
                })
            });

            return Json(new { success = true, comments = result });
        }

        // ============================================================
        // ===== CREATE POST =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(string title, string content, string? mediaUrl, string? mediaType)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId) || userId == 0)
                return Json(new { success = false, message = "User not found" });

            if (string.IsNullOrWhiteSpace(title))
                return Json(new { success = false, message = "Title is required." });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Content is required." });

            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

            var post = new HelpdeskPost
            {
                UserId = userId,
                Title = title.Trim(),
                Content = content.Trim(),
                MediaUrl = string.IsNullOrWhiteSpace(mediaUrl) ? null : mediaUrl.Trim(),
                MediaType = string.IsNullOrWhiteSpace(mediaType) ? null : mediaType.Trim(),
                IsAdminPost = isAdmin
            };

            if (_dbHelper.CreateHelpdeskPost(post, out string? error))
            {
                // Admin হলে সব user কে notification
                if (isAdmin)
                {
                    try
                    {
                        var allUsers = _dbHelper.GetAllUsers()
                            .Where(u => u.Role != "Admin")
                            .ToList();

                        foreach (var u in allUsers)
                        {
                            _dbHelper.CreateNotification(
                                u.Id,
                                "📢 New Announcement from Admin",
                                $"Admin posted: {post.Title}",
                                "info",
                                "/Helpdesk",
                                "fa-bullhorn"
                            );
                        }
                    }
                    catch { }
                }

                return Json(new { success = true, message = "Post created successfully!", postId = post.Id });
            }

            return Json(new { success = false, message = error ?? "Failed to create post." });
        }

        // ============================================================
        // ===== UPLOAD MEDIA (Image/Video) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMedia(IFormFile file)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file selected." });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var imageExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var videoExts = new[] { ".mp4", ".webm", ".ogg", ".mov" };

            string mediaType;
            long maxSize;
            string[] allowed;

            if (imageExts.Contains(ext))
            {
                mediaType = "image";
                maxSize = 5 * 1024 * 1024;  // 5MB
                allowed = imageExts;
            }
            else if (videoExts.Contains(ext))
            {
                mediaType = "video";
                maxSize = 50 * 1024 * 1024;  // 50MB
                allowed = videoExts;
            }
            else
            {
                return Json(new { success = false, message = "Unsupported file type." });
            }

            if (!allowed.Contains(ext))
                return Json(new { success = false, message = "Invalid file type." });

            if (file.Length > maxSize)
            {
                var limitMB = mediaType == "image" ? 5 : 50;
                return Json(new { success = false, message = $"File too large. Max {limitMB}MB allowed." });
            }

            try
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "helpdesk");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"hd_{DateTime.Now.Ticks}{ext}";
                var fullPath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var url = $"/uploads/helpdesk/{fileName}";
                return Json(new { success = true, url, mediaType });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Upload error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== ADD COMMENT (with reply support) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int postId, string content, int? parentCommentId = null,
            string? mediaUrl = null, string? mediaType = null)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId) || userId == 0)
                return Json(new { success = false, message = "User not found" });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Comment cannot be empty." });

            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            var post = _dbHelper.GetHelpdeskPostById(postId);

            if (post == null)
                return Json(new { success = false, message = "Post not found." });

            var comment = new HelpdeskComment
            {
                PostId = postId,
                UserId = userId,
                Content = content.Trim(),
                MediaUrl = string.IsNullOrWhiteSpace(mediaUrl) ? null : mediaUrl.Trim(),
                MediaType = string.IsNullOrWhiteSpace(mediaType) ? null : mediaType.Trim(),
                IsAdminReply = isAdmin,
                ParentCommentId = parentCommentId
            };

            if (!_dbHelper.CreateHelpdeskComment(comment, out string? error))
                return Json(new { success = false, message = error ?? "Failed to add comment." });

            // ============================================================
            // ===== NOTIFICATIONS =====
            // ============================================================
            try
            {
                var commenterName = HttpContext.Session.GetString("UserFullName")
                                    ?? HttpContext.Session.GetString("UserName")
                                    ?? "Someone";

                // ✅ Case 1: Admin reply দিলে → Post owner কে notification
                if (isAdmin && post.UserId != userId)
                {
                    _dbHelper.CreateNotification(
                        post.UserId,
                        "💬 Admin replied to your post",
                        $"Admin commented on \"{post.Title}\": {Truncate(content, 100)}",
                        "success",
                        "/Helpdesk",
                        "fa-comment-dots"
                    );
                }
                // ✅ Case 2: User reply দিলে → Admin দের notification + Post owner কে
                else
                {
                    // Admin দের notification
                    var admins = _dbHelper.GetAllUsers().Where(u => u.Role == "Admin").ToList();
                    foreach (var admin in admins)
                    {
                        _dbHelper.CreateNotification(
                            admin.Id,
                            "💬 New comment on Helpdesk",
                            $"{commenterName} commented on \"{post.Title}\": {Truncate(content, 100)}",
                            "info",
                            "/Helpdesk",
                            "fa-comment"
                        );
                    }

                    // Post owner নিজে comment না করলে owner কে notification
                    if (post.UserId != userId)
                    {
                        _dbHelper.CreateNotification(
                            post.UserId,
                            "💬 New reply on your post",
                            $"{commenterName} replied: {Truncate(content, 100)}",
                            "info",
                            "/Helpdesk",
                            "fa-reply"
                        );
                    }
                }

                // ✅ Case 3: Reply হলে → Parent comment owner কে notification
                if (parentCommentId.HasValue)
                {
                    var parentComments = _dbHelper.GetHelpdeskComments(postId);
                    var parent = FindCommentById(parentComments, parentCommentId.Value);

                    if (parent != null && parent.UserId != userId)
                    {
                        _dbHelper.CreateNotification(
                            parent.UserId,
                            "↩️ New reply to your comment",
                            $"{commenterName} replied: {Truncate(content, 100)}",
                            "info",
                            "/Helpdesk",
                            "fa-reply"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Helpdesk notification error: " + ex.Message);
            }

            return Json(new
            {
                success = true,
                message = "Comment added successfully!",
                commentId = comment.Id
            });
        }

        // ============================================================
        // ===== DELETE POST =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userIdStr = HttpContext.Session.GetString("UserId");
            int.TryParse(userIdStr, out int userId);

            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

            if (_dbHelper.DeleteHelpdeskPost(id, userId, isAdmin))
                return Json(new { success = true, message = "Post deleted." });

            return Json(new { success = false, message = "Failed to delete post." });
        }

        // ============================================================
        // ===== DELETE COMMENT =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(int id)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userIdStr = HttpContext.Session.GetString("UserId");
            int.TryParse(userIdStr, out int userId);

            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

            if (_dbHelper.DeleteHelpdeskComment(id, userId, isAdmin))
                return Json(new { success = true, message = "Comment deleted." });

            return Json(new { success = false, message = "Failed to delete comment." });
        }

        // ============================================================
        // ===== ACKNOWLEDGE POST (Admin) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AcknowledgePost(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.AcknowledgeHelpdeskPost(id, true))
                return Json(new { success = true, message = "Marked as acknowledged." });

            return Json(new { success = false, message = "Failed." });
        }

        // ============================================================
        // ===== UPDATE STATUS (Admin) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.UpdateHelpdeskPostStatus(id, status))
                return Json(new { success = true, message = "Status updated." });

            return Json(new { success = false, message = "Failed." });
        }

        // ============================================================
        // ===== HELPERS =====
        // ============================================================
        private static HelpdeskComment? FindCommentById(List<HelpdeskComment> list, int id)
        {
            foreach (var c in list)
            {
                if (c.Id == id) return c;
                var nested = FindCommentById(c.Replies, id);
                if (nested != null) return nested;
            }
            return null;
        }

        private static string Truncate(string text, int max)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= max ? text : text.Substring(0, max) + "...";
        }
    }
}