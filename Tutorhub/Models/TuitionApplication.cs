using System;
using System.ComponentModel.DataAnnotations;

namespace Tutorbub.Models
{
    public class TuitionApplication
    {
        public int Id { get; set; }

        public int TuitionPostId { get; set; }
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        public string ApplicantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(30)]
        public string? WhatsAppNumber { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "CV link is required")]
        [StringLength(500)]
        public string CvLink { get; set; } = string.Empty;

        public string? CoverNote { get; set; }

        // "Pending" | "Shortlisted" | "Selected" | "Rejected"
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public string? AdminNote { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public TuitionPost? TuitionPost { get; set; }

        // Helper: WhatsApp link
        public string WhatsAppLink
        {
            get
            {
                var num = (WhatsAppNumber ?? PhoneNumber ?? "").Trim();
                if (string.IsNullOrEmpty(num)) return "";
                if (num.StartsWith("0")) num = "88" + num;
                else if (!num.StartsWith("88")) num = "880" + num.TrimStart('+');
                return $"https://wa.me/{num}";
            }
        }

        public string StatusLabel => Status switch
        {
            "Pending" => "Pending",
            "Shortlisted" => "Shortlisted",
            "Selected" => "Selected",
            "Rejected" => "Rejected",
            _ => Status
        };
    }
}