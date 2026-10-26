// 📁 Program.cs
// লোকেশন: Tutorbub/Program.cs

using Microsoft.EntityFrameworkCore;
using Tutorbub.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ===== 1) MVC Services =====
// ============================================================
builder.Services.AddControllersWithViews();

// ============================================================
// ===== 2) Session Configuration =====
// ============================================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ============================================================
// ===== 3) ✅ Entity Framework Core (PostgreSQL) =====
// ============================================================
// "Database" connection string আপনার appsettings.json থেকে আসবে
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

// ============================================================
// ===== 4) HttpContextAccessor (ViewBag/Sidebar এ দরকার) =====
// ============================================================
builder.Services.AddHttpContextAccessor();

// ============================================================
// ===== Build App =====
// ============================================================
var app = builder.Build();

// ============================================================
// ===== 5) Configure HTTP Pipeline =====
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Session MUST be between UseRouting and UseAuthorization
app.UseSession();

app.UseAuthorization();

// ============================================================
// ===== 6) Default Route =====
// ============================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ============================================================
// ===== Run App =====
// ============================================================
app.Run();