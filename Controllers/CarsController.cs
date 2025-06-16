using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarManagment.Data;
using CarManagment.Models;
using CarManagment.Services;
using System.Security.Claims;

namespace CarManagment.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILoggingService _logger;

        public CarsController(AppDbContext context, ILoggingService logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicles = await _context.Vehicles
                .Where(v => v.OwnerId == userId)
                .OrderByDescending(v => v.Year)
                .ToListAsync();

            return View(vehicles);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null) return Unauthorized();
                else
                {   
                vehicle.OwnerId = userId;
                }

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Пользователь {UserId} добавил новое транспортное средство {VehicleId}", 
                    userId ?? "unknown", vehicle.Id);

                return RedirectToAction(nameof(Index));
            }

            return View(vehicle);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _context.Vehicles
                .Include(v => v.Repairs)
                    .ThenInclude(r => r.WorkType)
                .FirstOrDefaultAsync(v => v.Id == id && v.OwnerId == userId);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }
    }
}

