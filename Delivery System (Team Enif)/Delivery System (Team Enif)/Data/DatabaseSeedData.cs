namespace Delivery_System__Team_Enif_.Data
{

    using System;
    using System.Threading.Tasks;
    using Delivery_System__Team_Enif_.Data.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Delivery_System__Team_Enif_.Models;
    using static GoogleMaps.LocationServices.Directions;

    public static class DatabaseSeedData
    {
        public static async void Initialize(WebApplication app)
        {

            using (var serviceScope = app.Services.CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<ProjectDbContext>())
                {
                    dbContext.Database.Migrate();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    // Seed roles
                    await SeedRolesAsync(roleManager);

                    // Seed a default admin user
                    await SeedAdminUserAsync(userManager);

                    SeedDeliveryData(dbContext);
                }
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager) 
        {
            var roleNames = new[] { "Admin", "User", "Courier", "Office assistant" };

            foreach (var roleName in roleNames)
            {
                try
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                Console.WriteLine($"Error: {error.Description}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Role {roleName} created successfully.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Role {roleName} already exists.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating role {roleName}: {ex.Message}");
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
            {
                user = new ApplicationUser { 
                    Name = "admin", 
                    UserName = "admin@admin.com", 
                    Email = "admin@admin.com"
                };
                var createUser = await userManager.CreateAsync(user, "Password123!");

                if (createUser.Succeeded)
                {
                    user.EmailConfirmed = true;
                    user.ApprovalStatus = ApprovalStatus.Approved;
                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        private static void SeedDeliveryData(ProjectDbContext dbContext)
        {
            try
            {
                if (!dbContext.DeliveryOptions.Any()) // Check if the table is empty before seeding
                {
                    foreach (DeliveryOptionEnum deliveryOption in Enum.GetValues(typeof(DeliveryOptionEnum)))
                    {
                        dbContext.DeliveryOptions.Add(new DeliveryOption
                        {
                            Name = deliveryOption.ToString()
                        });
                    }
                }

                if (!dbContext.DeliveryStatuses.Any())
                {
                    foreach (DeliveryStatusEnum deliveryStatusEnum in Enum.GetValues(typeof(DeliveryStatusEnum)))
                    {
                        dbContext.DeliveryStatuses.Add(new DeliveryStatus
                        {
                            Name = deliveryStatusEnum.ToString()
                        });
                    }
                }

                if (!dbContext.DeliveryTypes.Any())
                {
                    foreach (DeliveryTypeEnum deliveryTypeEnum in Enum.GetValues(typeof(DeliveryTypeEnum)))
                    {
                        dbContext.DeliveryTypes.Add(new DeliveryType
                        {
                            Name = deliveryTypeEnum.ToString()
                        });
                    }
                }

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating delivery data: {ex.Message}");
            }
        }
    }
}
