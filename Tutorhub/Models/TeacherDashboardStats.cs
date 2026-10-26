// 📁 Models/TeacherDashboardStats.cs
// লোকেশন: Tutorbub/Models/TeacherDashboardStats.cs

using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    // ============================================================
    // ===== TEACHER DASHBOARD STATS =====
    // ============================================================
    /// <summary>
    /// টিচার ড্যাশবোর্ডের সামগ্রিক পরিসংখ্যান।
    /// DatabaseHelper.GetTeacherDashboardStats() থেকে populate হয়।
    /// </summary>
    public class TeacherDashboardStats
    {
        /// <summary>টিচারের মোট Teach Wanted Posts সংখ্যা</summary>
        public int TotalPosts { get; set; }

        /// <summary>তার পোস্টগুলোতে আসা মোট কমেন্টের সংখ্যা</summary>
        public int TotalComments { get; set; }

        /// <summary>তার পোস্টগুলোতে পড়া মোট লাইকের সংখ্যা</summary>
        public int TotalLikes { get; set; }

        /// <summary>তার পোস্টের কমেন্টে আসা মোট রিপ্লাইয়ের সংখ্যা (ParentId != null)</summary>
        public int TotalReplies { get; set; }

        /// <summary>এই টিচারের করা মোট Tuition Applications সংখ্যা</summary>
        public int TotalTuitionApplications { get; set; }

        /// <summary>মোট Responses = Comments + Likes</summary>
        public int TotalResponses { get; set; }
    }

    // ============================================================
    // ===== TEACHER POST STAT =====
    // ============================================================
    /// <summary>
    /// টিচারের প্রতিটি পোস্টের আলাদা পরিসংখ্যান।
    /// DatabaseHelper.GetTeacherPostStats() থেকে populate হয়।
    /// </summary>
    public class TeacherPostStat
    {
        public int PostId { get; set; }

        /// <summary>পোস্টের মূল টেক্সট কনটেন্ট</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>পোস্টে যুক্ত ছবির URL (থাকলে)</summary>
        public string? ImageUrl { get; set; }

        /// <summary>এই পোস্টে মোট লাইক সংখ্যা</summary>
        public int LikeCount { get; set; }

        /// <summary>এই পোস্টে মোট কমেন্ট সংখ্যা</summary>
        public int CommentCount { get; set; }

        /// <summary>পোস্ট কখন তৈরি হয়েছিল</summary>
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
                return trimmed.Length <= 120 ? trimmed : trimmed.Substring(0, 120) + "...";
            }
        }

        // ============================================================
        // ===== Helper: Total Engagement =====
        // ============================================================
        public int TotalEngagement => LikeCount + CommentCount;
    }
}