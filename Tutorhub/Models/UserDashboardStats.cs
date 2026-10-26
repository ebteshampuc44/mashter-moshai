// 📁 Models/UserDashboardStats.cs
// লোকেশন: Tutorbub/Models/UserDashboardStats.cs

using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    // ============================================================
    // ===== USER DASHBOARD STATS =====
    // ============================================================
    /// <summary>
    /// ইউজার ড্যাশবোর্ডের সামগ্রিক পরিসংখ্যান।
    /// DatabaseHelper.GetUserDashboardStats() থেকে populate হয়।
    /// </summary>
    public class UserDashboardStats
    {
        /// <summary>ইউজারের মোট পোস্ট সংখ্যা</summary>
        public int TotalPosts { get; set; }

        /// <summary>তার পোস্টগুলোতে পড়া মোট লাইকের সংখ্যা</summary>
        public int TotalLikes { get; set; }

        /// <summary>তার পোস্টগুলোতে আসা মোট কমেন্টের সংখ্যা</summary>
        public int TotalComments { get; set; }

        /// <summary>তার সাথে এখন পর্যন্ত যুক্ত হওয়া টিচারের সংখ্যা</summary>
        public int TotalTeachersConnected { get; set; }
    }
}