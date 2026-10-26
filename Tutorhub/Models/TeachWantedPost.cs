using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class TeachWantedPost
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int LikeCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Display Helpers (JOIN)
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }

        // পোস্টের মেইন কমেন্টগুলো
        public List<TeachWantedComment> Comments { get; set; } = new();
    }

    public class TeachWantedComment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ রিপ্লাই সিস্টেমের জন্য (যে কমেন্টের নিচে রিপ্লাই দিচ্ছে তার আইডি)
        public int? ParentId { get; set; }

        // Display Helpers (JOIN)
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }

        // ✅ কমেন্ট লাইক (Facebook-style)
        public int LikeCount { get; set; } = 0;
        public bool IsLikedByMe { get; set; } = false;

        // ✅ এই কমেন্টের নিচে যত রিপ্লাই আছে
        public List<TeachWantedComment> Replies { get; set; } = new();
    }
}