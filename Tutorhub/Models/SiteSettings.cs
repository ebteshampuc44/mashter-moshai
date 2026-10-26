// 📁 Models/SiteSettings.cs
// লোকেশন: Tutorbub/Models/SiteSettings.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Models
{
    public class SiteSettings
    {
        public int Id { get; set; }

        // ===== Hero Images (Comma-separated URLs) =====
        public string HeroImageUrls { get; set; } = string.Empty;

        // ===== ✅ Tutor Section Image (Home page "Find Your Teacher") =====
        public string TutorSectionImageUrl { get; set; } = "https://i.ibb.co.com/Hfpwnx46/111.png";

        // ============================================================
        // ===== ✅ NEW: Branding / Logo Settings =====
        // ============================================================

        // সাইটের নাম (Navbar ও Footer-এ দেখাবে)
        public string SiteName { get; set; } = "SMART TUTOR HUB";

        // নেভবারের লোগো URL
        public string NavbarLogoUrl { get; set; } = "https://i.ibb.co/fd9sY88Y/IMG-6916-1.png";

        // ফুটারের লোগো URL
        public string FooterLogoUrl { get; set; } = "https://i.ibb.co/fd9sY88Y/IMG-6916-1.png";

        // লোগোর সাইজ (পিক্সেলে) — 24 থেকে 100 px
        public int LogoSizePx { get; set; } = 56;

        // লোগোর শেপ — circle | square | rounded
        public string LogoShape { get; set; } = "circle";

        // ===== Slider Settings =====
        public bool SliderEnabled { get; set; } = false;
        public int SliderIntervalSeconds { get; set; } = 5;

        // ===== Theme Colors =====
        public string PrimaryColor { get; set; } = "#1F3B2C";
        public string SecondaryColor { get; set; } = "#F3F1E7";
        public string AccentColor { get; set; } = "#F4C744";
        public string TextColor { get; set; } = "#4B5648";
        public string NavbarBgColor { get; set; } = "#F3F1E7";
        public string FooterBgColor { get; set; } = "#F3F1E7";

        // ===== Meta =====
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ===== Helper: Hero Images List =====
        public List<string> GetHeroImageList()
        {
            if (string.IsNullOrWhiteSpace(HeroImageUrls))
                return new List<string>();

            return HeroImageUrls
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        // ===== Helper: Logo Border Radius (CSS value) =====
        public string GetLogoBorderRadius()
        {
            return LogoShape?.ToLowerInvariant() switch
            {
                "circle" => "50%",
                "rounded" => "16px",
                "square" => "0",
                _ => "50%"
            };
        }

        // ===== Helper: Normalized Logo Size (24–100) =====
        public int GetSafeLogoSize()
        {
            if (LogoSizePx < 24) return 24;
            if (LogoSizePx > 100) return 100;
            return LogoSizePx;
        }

        // ===== Helper: Navbar Logo (fallback) =====
        public string GetNavbarLogo()
        {
            return string.IsNullOrWhiteSpace(NavbarLogoUrl)
                ? "https://i.ibb.co/fd9sY88Y/IMG-6916-1.png"
                : NavbarLogoUrl;
        }

        // ===== Helper: Footer Logo (fallback) =====
        public string GetFooterLogo()
        {
            return string.IsNullOrWhiteSpace(FooterLogoUrl)
                ? "https://i.ibb.co/fd9sY88Y/IMG-6916-1.png"
                : FooterLogoUrl;
        }

        // ===== Helper: Safe Site Name =====
        public string GetSafeSiteName()
        {
            return string.IsNullOrWhiteSpace(SiteName)
                ? "SMART TUTOR HUB"
                : SiteName.Trim();
        }
    }
}