// 📁 Models/UserPostStat.cs
// লোকেশন: Tutorbub/Models/UserPostStat.cs

using System;

namespace Tutorbub.Models
{
    // ============================================================
    // ===== USER POST STAT (Admin User Overview এর জন্য) =====
    // ============================================================
    /// <summary>
    /// একজন ইউজারের প্রতিটি TuitionCircularPost-এর পরিসংখ্যান।
    /// DatabaseHelper.GetUserPostStats() থেকে populate হয়।
    /// এতে লাইক, কমেন্ট এবং কতজন ভিন্ন Teacher রেসপন্স করেছে তা থাকে।
    /// </summary>
    public class UserPostStat
    {
        public int PostId { get; set; }

        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string? PreferredArea { get; set; }

        public string? ClassLevel { get; set; }

        public string? Subjects { get; set; }

        public string? Budget { get; set; }

        /// <summary>এই পোস্টে মোট লাইক সংখ্যা</summary>
        public int LikeCount { get; set; }

        /// <summary>এই পোস্টে মোট কমেন্ট সংখ্যা</summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// এই পোস্টে কতজন ভিন্ন Teacher কমেন্ট/রেসপন্স করেছে।
        /// 0 হলে বোঝা যায় এখনো কোনো টিচার সাড়া দেয়নি।
        /// </summary>
        public int TeacherResponses { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ============================================================
        // ===== Helper: Time Ago =====
        // ============================================================
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

        // ============================================================
        // ===== Helper: Content Preview =====
        // ============================================================
        public string ContentPreview
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content)) return "(No text)";
                var trimmed = Content.Trim();
                return trimmed.Length <= 140 ? trimmed : trimmed.Substring(0, 140) + "...";
            }
        }

        // ============================================================
        // ===== Helper: Total Engagement =====
        // ============================================================
        public int TotalEngagement => LikeCount + CommentCount;

        // ============================================================
        // ===== Helper: Has Teacher Response? =====
        // ============================================================
        public bool HasTeacherResponse => TeacherResponses > 0;
    }

    // ============================================================
    // ===== TEACHER ENGAGEMENT (User Overview sidebar) =====
    // ============================================================
    /// <summary>
    /// একজন ইউজারের পোস্টে যে সব Teacher রেসপন্স করেছে তাদের সারসংক্ষেপ।
    /// DatabaseHelper.GetTeachersEngagedWithUser() থেকে populate হয়।
    /// </summary>
    public class TeacherEngagement
    {
        public int TeacherId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? ProfileImageLink { get; set; }

        /// <summary>এই টিচার মোট কতবার রেসপন্স করেছে</summary>
        public int ResponseCount { get; set; }

        /// <summary>সর্বশেষ কখন রেসপন্স করেছে</summary>
        public DateTime LastResponseAt { get; set; } = DateTime.UtcNow;

        // ============================================================
        // ===== Helper: Initials =====
        // ============================================================
        public string Initials
        {
            get
            {
                var name = string.IsNullOrWhiteSpace(FullName) ? "T" : FullName;
                var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length >= 2)
                    return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
                return words.Length == 1 && words[0].Length > 0
                    ? words[0][0].ToString().ToUpper()
                    : "T";
            }
        }

        // ============================================================
        // ===== Helper: Time Ago (Last Response) =====
        // ============================================================
        public string LastResponseAgo
        {
            get
            {
                var span = DateTime.UtcNow - LastResponseAt;
                if (span.TotalSeconds < 60) return "just now";
                if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
                if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
                if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
                if (span.TotalDays < 30) return $"{(int)(span.TotalDays / 7)}w ago";
                return LastResponseAt.ToString("dd MMM yyyy");
            }
        }
    }
}