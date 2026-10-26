// 📁 Models/TutorRequest.cs
using System;

namespace Tutorbub.Models
{
    public class TutorRequest
    {
        public int Id { get; set; }
        public string? TutorId { get; set; }
        public string? TutorName { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Email { get; set; }
        public string? StudentClass { get; set; }
        public string? RequiredSubject { get; set; }
        public string? PreferredArea { get; set; }
        public string? TeachingMode { get; set; }
        public string? PreferredDays { get; set; }
        public string? PreferredTime { get; set; }
        public string? MonthlyBudget { get; set; }
        public string? AdditionalNotes { get; set; }
        public string RequestType { get; set; } = "SpecificTutor";
        public string? RequestId { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminNote { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Helper
        public string WhatsAppLink
        {
            get
            {
                var num = (WhatsAppNumber ?? PhoneNumber ?? "").Trim();
                if (string.IsNullOrEmpty(num)) return "";

                // Bangladesh: 01XXXXXXXXX → 8801XXXXXXXXX
                if (num.StartsWith("0"))
                    num = "88" + num;
                else if (!num.StartsWith("88"))
                    num = "880" + num.TrimStart('+');

                return $"https://wa.me/{num}";
            }
        }

        public string RequestTypeLabel => RequestType switch
        {
            "SpecificTutor" => "Specific Tutor",
            "NeedTutor" => "Need a Tutor",
            "AreaTuition" => "Area Tuition",
            _ => RequestType
        };
    }
}