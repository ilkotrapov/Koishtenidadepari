using Delivery_System__Team_Enif_.Data;
using Delivery_System__Team_Enif_.Hubs;
using Delivery_System__Team_Enif_.Models;
using Delivery_System__Team_Enif_.Models.Stripe;
using Delivery_System__Team_Enif_.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

public class Program
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IEmailSender, EmailSender>();

        // Configure the DbContext with SQL Server and the connection string
        builder.Services.AddDbContext<ProjectDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;  // Require email confirmation for login
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ProjectDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        builder.Services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<StripeSettings>>().Value);

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Session timeout
            options.SlidingExpiration = true; // Refresh the session time
        });

        builder.Services.AddSignalR();

        builder.Services.AddControllersWithViews();
    }



    public static void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        DatabaseSeedData.Initialize(app);

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapHub<PackageHub>("/packageHub");

    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);

        /*if (builder.Environment.IsDevelopment())
        {
            // Allow invalid certificates for local testing
            HttpClientHandler.ServerCertificateCustomValidationCallback =
                (message, cert, chain, errors) => true;
        }
        */

        var app = builder.Build();
        ConfigureApp(app);
        app.Run();
        
    }
}