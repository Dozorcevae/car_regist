using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarManagment.Models;
using CarManagment.Services;

namespace CarManagment.Data
{
    public static class DatabaseInitializer
    {
        public static async Task Initialize(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggingService logger)
        {
            try
            {
                await CreateRoles(roleManager, logger);

                await CreateAdminUser(userManager, logger);

                await CreateWorkTypes(context, logger);

                logger.LogInformation("Инициализация базы данных завершена успешно");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при инициализации базы данных");
                throw;
            }
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager, ILoggingService logger)
        {
            var roles = new[] { "Admin", "Client" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Роль {Role} создана", role);
                }
            }
        }

        private static async Task CreateAdminUser(UserManager<ApplicationUser> userManager, ILoggingService logger)
        {
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Администратор создан");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError(new Exception(errors), "Ошибка при создании администратора");
                }
            }
        }

        private static async Task CreateWorkTypes(AppDbContext context, ILoggingService logger)
        {
            if (!context.WorkTypes.Any())
            {
                var workTypes = new[]
                {
                    new WorkType { Name = "Замена масла", Cost = 2000, DurationMin = 60, Description = "Замена моторного масла и масляного фильтра" },
                    new WorkType { Name = "Замена тормозных колодок", Cost = 3000, DurationMin = 90, Description = "Замена передних и задних тормозных колодок" },
                    new WorkType { Name = "Диагностика двигателя", Cost = 1500, DurationMin = 45, Description = "Компьютерная диагностика двигателя" },
                    new WorkType { Name = "Замена ремня ГРМ", Cost = 8000, DurationMin = 180, Description = "Замена ремня газораспределительного механизма" }
                };

                await context.WorkTypes.AddRangeAsync(workTypes);
                await context.SaveChangesAsync();
                logger.LogInformation("Типы работ созданы");
            }
        }
    }
}