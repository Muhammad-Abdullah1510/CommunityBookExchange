using Identity.Data;
using Identity.Models;
using Identity.Models.IRepositories;
using Identity.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)  // identity user ki jagah pani class identity modifictation krte hoye
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();


 // adding signal R
 builder.Services.AddSignalR();
//dependencyinjection

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
//builder.Services.AddScoped<IUserNameRepository, UserNameRepository>();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IProfilePictureRepository, ProfilePictureRepository>();



// adding authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BothOrRenter", policy => policy.RequireClaim("UserRole", "Renter", "Both"));

    options.AddPolicy("BothOrDonor", policy => policy.RequireClaim("UserRole", "Donor", "Both"));
    

    //options.AddPolicy("BothOnly", policy => policy.RequireClaim("UserRole", "Both"));
    
     options.AddPolicy("All", policy =>policy.RequireClaim("UserRole", "Donor", "Renter", "Both"));
    

});


builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;
});


var app = builder.Build();


app.MapHub<ChatHub>("/chatHub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
