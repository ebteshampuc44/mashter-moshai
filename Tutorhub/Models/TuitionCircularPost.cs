// 📁 Models/TuitionCircularPost.cs
// লোকেশন: Tutorbub/Models/TuitionCircularPost.cs

using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    // ============================================================
    // ===== TUITION CIRCULAR POST (Student → Teacher) =====
    // ============================================================
    public class TuitionCircularPost
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        // Additional details for the tuition request
        public string? PreferredArea { get; set; }
        public string? ClassLevel { get; set; }
        public string? Subjects { get; set; }
        public string? Budget { get; set; }

        public int LikeCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ===== Display Helpers (populated by JOIN) =====
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }

        // Post-এর মেইন কমেন্টগুলো
        public List<TuitionCircularComment> Comments { get; set; } = new();
    }

    // ============================================================
    // ===== TUITION CIRCULAR COMMENT =====
    // ============================================================
    public class TuitionCircularComment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ রিপ্লাই সিস্টেমের জন্য (যে কমেন্টের নিচে রিপ্লাই দিচ্ছে তার আইডি)
        public int? ParentId { get; set; }

        // ===== Display Helpers (populated by JOIN) =====
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }

        // ✅ কমেন্ট লাইক (Facebook-style)
        public int LikeCount { get; set; } = 0;
        public bool IsLikedByMe { get; set; } = false;

        // ✅ এই কমেন্টের নিচে যত রিপ্লাই আছে
        public List<TuitionCircularComment> Replies { get; set; } = new();
    }
}