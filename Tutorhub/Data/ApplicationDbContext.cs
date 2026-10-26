// 📁 Data/ApplicationDbContext.cs
// লোকেশন: Tutorbub/Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tutorbub.Models;

namespace Tutorbub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TuitionPost> TuitionPosts { get; set; }
        public DbSet<TuitionApplication> TuitionApplications { get; set; }

        // ✅ User এবং Notification
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================================
            // ✅ Fix: Local DateTime কে auto UTC করে দেবে
            // PostgreSQL 'timestamp with time zone' শুধু UTC নেয়,
            // তাই DateTime.Now accidentally ব্যবহার হলেও আর error আসবে না।
            // ============================================================
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue
                        ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime())
                        : v,
                v => v.HasValue
                        ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                        : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }

            // TuitionPost → TuitionApplication (one-to-many)
            modelBuilder.Entity<TuitionApplication>()
                .HasOne(a => a.TuitionPost)
                .WithMany(p => p.Applications)
                .HasForeignKey(a => a.TuitionPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Table mappings (existing PostgreSQL tables) =====
            modelBuilder.Entity<TuitionPost>().ToTable("TuitionPosts");
            modelBuilder.Entity<TuitionApplication>().ToTable("TuitionApplications");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Notification>().ToTable("Notifications");
        }
    }
}