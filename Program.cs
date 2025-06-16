using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarManagment.Data;
using CarManagment.Models;
using CarManagment.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
    options.Password.RequireDigit = builder.Configuration.GetValue<bool>("Identity:Password:RequireDigit");
    options.Password.RequireLowercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireLowercase");
    options.Password.RequireUppercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireUppercase");
    options.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumeric");
    options.Password.RequiredLength = builder.Configuration.GetValue<int>("Identity:Password:RequiredLength");
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Регистрация сервиса логирования
builder.Services.AddSingleton<ILoggingService>(sp => 
    new SerilogLoggingService(Log.Logger));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Настройка маршрутизации
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILoggingService>();

        logger.LogInformation("Начало инициализации базы данных");
        
        // Применяем миграции автоматически
        logger.LogInformation("Применение миграций...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Миграции успешно применены");

        await DatabaseInitializer.Initialize(context, userManager, roleManager, logger);
        logger.LogInformation("База данных успешно инициализирована");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILoggingService>();
        logger.LogError(ex, "Произошла ошибка при инициализации базы данных");
    }
}

app.Run();
