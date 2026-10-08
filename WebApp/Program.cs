using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;//truy cap o phia server JS va client khong doc cookie nay truc tiep
    options.Cookie.IsEssential = true;//danh dau Cookie Seesion la Cookie can thiet cho ung dung 
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ProdConnection")));

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();// cho phep truy cap file tinh

app.UseRouting();
app.UseSession();
app.UseAuthentication();//dang nhap hay chua

app.UseAuthorization();//quyen truy cap


app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();