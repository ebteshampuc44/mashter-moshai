using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class HelpdeskPost
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }  // 'image' | 'video' | null
        public bool IsAdminPost { get; set; } = false;
        public bool IsAcknowledged { get; set; } = false;
        public string Status { get; set; } = "Open";  // Open | Solved | Closed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ===== Display Helpers (populated by JOIN) =====
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public string UserRole { get; set; } = "User";
        public int CommentCount { get; set; }

        public List<HelpdeskComment> Comments { get; set; } = new();

        // ===== Computed =====
        public string Initials
        {
            get
            {
                var name = UserFullName ?? "U";
                var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length >= 2)
                    return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
                return words.Length == 1 && words[0].Length > 0
                    ? words[0][0].ToString().ToUpper()
                    : "U";
            }
        }

        public string TimeAgo
        {
            get
            {
                var span = DateTime.UtcNow - CreatedAt;
                if (span.TotalSeconds < 60) return "just now";
                if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
                if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
                if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
                if (span.TotalDays < 30) return $"{(int)(span.TotalDays / 7)}w ago";
                return CreatedAt.ToString("dd MMM yyyy");
            }
        }
    }

    public class HelpdeskComment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public bool IsAdminReply { get; set; } = false;
        public int? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ===== Display Helpers =====
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public string UserRole { get; set; } = "User";
        public int ReplyCount { get; set; }
        public List<HelpdeskComment> Replies { get; set; } = new();

        public string Initials
        {
            get
            {
                var name = UserFullName ?? "U";
                var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length >= 2)
                    return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
                return words.Length == 1 && words[0].Length > 0
                    ? words[0][0].ToString().ToUpper()
                    : "U";
            }
        }

        public string TimeAgo
        {
            get
            {
                var span = DateTime.UtcNow - CreatedAt;
                if (span.TotalSeconds < 60) return "just now";
                if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
                if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
                if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
                if (span.TotalDays < 30) return $"{(int)(span.TotalDays / 7)}w ago";
                return CreatedAt.ToString("dd MMM yyyy");
            }
        }
    }
}