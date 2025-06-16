using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarManagment.Data;
using CarManagment.Models;
using System.Diagnostics;
using CarManagment.Services;

namespace CarManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly ILoggingService _loggingService;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, ILoggingService loggingService)
        {
            _logger = logger;
            _context = context;
            _loggingService = loggingService;
        }

        public IActionResult Index()
        {
            _loggingService.LogInformation("Пользователь зашел на главную страницу");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 