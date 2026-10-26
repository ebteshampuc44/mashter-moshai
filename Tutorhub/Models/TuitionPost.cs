using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tutorbub.Models
{
    public class TuitionPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(300)]
        public string Title { get; set; } = string.Empty;

        // "Female, Male" / "Male" / "Female"
        [StringLength(100)]
        public string? StudentGender { get; set; }

        // "9 & 5 (2 Students)"
        [StringLength(200)]
        public string? StudentClass { get; set; }

        // "All" / "Math, Physics"
        [StringLength(300)]
        public string? Subjects { get; set; }

        // "5" / "3"
        [StringLength(50)]
        public string? DaysPerWeek { get; set; }

        // "Negotiable" / "Evening 5-8 PM"
        [StringLength(200)]
        public string? PreferredTime { get; set; }

        // "4000"
        [StringLength(100)]
        public string? Salary { get; set; }

        // "অক্সিজেন, বটতল, মদিনা মসজিদ"
        [StringLength(300)]
        public string? Area { get; set; }

        // "50% (After Join)"
        [StringLength(200)]
        public string? MediaFee { get; set; }

        // "Oxygen8304"
        [StringLength(50)]
        public string? TuitionCode { get; set; }

        // "বিঃদ্রঃ দু'জনেই মাদ্রাসার ছাত্র-ছাত্রী..."
        public string? SpecialNote { get; set; }

        // "Female Teacher Needed"
        [StringLength(50)]
        public string? TutorGenderPreference { get; set; }

        // "Open" | "Closed"
        [StringLength(20)]
        public string Status { get; set; } = "Open";

        // ✅ NEW: হোম পেইজে এই টিউশন পোস্টটি দেখাবে কিনা
        public bool ShowOnHome { get; set; } = false;

        public DateTime PostedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public List<TuitionApplication> Applications { get; set; } = new();
    }
}