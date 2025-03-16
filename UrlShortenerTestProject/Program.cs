using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortenerTestProject.Data;
using UrlShortenerTestProject.Models;
using UrlShortenerTestProject.Repositories.AboutRep;
using UrlShortenerTestProject.Repositories.UrlRep;
using UrlShortenerTestProject.Services.AboutService;
using UrlShortenerTestProject.Services.UrlSer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => 
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IUrlShortService, UrlShortService>();

builder.Services.AddScoped<IAboutRepository, AboutRepository >();
builder.Services.AddScoped<IAboutService, AboutService>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 4;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.User.RequireUniqueEmail = true;
})
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin")); // або policy.RequireClaim("Admin")
});

var app = builder.Build();

//Додавання ролей
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await CreateRolesAsync(services);
//}

//async Task CreateRolesAsync(IServiceProvider serviceProvider)
//{
//    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

//    string[] roleNames = { "Admin", "User" }; // Ролі, які потрібно створити
//    foreach (var roleName in roleNames)
//    {
//        var roleExists = await roleManager.RoleExistsAsync(roleName);
//        if (!roleExists)
//        {
//            await roleManager.CreateAsync(new IdentityRole(roleName));
//        }
//    }

//    // Створення адміністратора, якщо його ще немає
//    string adminEmail = "admin@admin.com";
//    string adminPassword = "1111";

//    var adminUser = await userManager.FindByEmailAsync(adminEmail);
//    if (adminUser == null)
//    {
//        adminUser = new User { FullName = "Admin" ,UserName = adminEmail, Email = adminEmail };
//        var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
//        if (createAdmin.Succeeded)
//        {
//            await userManager.AddToRoleAsync(adminUser, "Admin");
//        }
//    }
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "shortUrlRedirect",
	pattern: "{shortUrl}",
	defaults: new { controller = "Home", action = "RedirectToLongUrl" });

app.Run();
