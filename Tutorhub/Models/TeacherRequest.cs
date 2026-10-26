// 📁 Models/TeacherRequest.cs
// লোকেশন: Tutorbub/Models/TeacherRequest.cs

using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    // ============================================================
    // ===== TEACHER REQUEST =====
    // ============================================================
    public class TeacherRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string SubjectExpertise { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string TeachingStyle { get; set; } = string.Empty;
        public string AvailableDays { get; set; } = string.Empty;
        public string PreferredTime { get; set; } = string.Empty;
        public string HourlyRate { get; set; } = string.Empty;
        public string CvLink { get; set; } = string.Empty;
        public string WhyTeach { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? ResponseDate { get; set; }
        public string? AdminNote { get; set; }
        public User? User { get; set; }
    }

    // ============================================================
    // ===== TEACHER DASHBOARD VIEW MODEL =====
    // ============================================================
    public class TeacherDashboardViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string SubjectExpertise { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string ProfileImageLink { get; set; } = string.Empty;
        public double Rating { get; set; }

        // ============================================================
        // ===== TUITION APPLICATIONS =====
        // ============================================================
        public List<TuitionApplication> TuitionApplications { get; set; } = new List<TuitionApplication>();
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ShortlistedApplications { get; set; }
        public int SelectedApplications { get; set; }
        public int RejectedApplications { get; set; }
    }
}