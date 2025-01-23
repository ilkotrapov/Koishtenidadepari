using Delivery_System__Team_Enif_.Data;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ProjectDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
        });

        // Add services to the container.
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

        using (var serviceScope = app.Services.CreateScope())
        {
            using(var dbContext = serviceScope.ServiceProvider.GetService<ProjectDbContext>())
            {
                dbContext.Database.Migrate();
            }
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

    }

    public static void Main(string[] args)
    {



        var builder = WebApplication.CreateBuilder(args);


        ConfigureServices(builder);

        var app = builder.Build();
        ConfigureApp(app);
        app.Run();
        
    }
}