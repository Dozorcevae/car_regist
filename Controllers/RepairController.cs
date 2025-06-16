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
    public class RepairController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILoggingService _logger;

        public RepairController(AppDbContext context, ILoggingService logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var repairs = await _context.RepairOrders
                .Include(r => r.Vehicle)
                .Where(r => r.Vehicle.OwnerId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(repairs);
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicles = await _context.Vehicles
                .Where(v => v.OwnerId == userId)
                .ToListAsync();

            ViewBag.Vehicles = vehicles;
            ViewBag.WorkTypes = await _context.WorkTypes.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Repair repair, List<int> workTypeIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == repair.VehicleId && v.OwnerId == userId);

            if (vehicle == null)
            {
                _logger.LogWarning("Попытка создания заказа для чужого транспортного средства {VehicleId} пользователем {UserId}", 
                    repair.VehicleId, userId);
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                repair.CreatedAt = DateTime.UtcNow;
                repair.Status = RepairStatus.Pending;
                _context.RepairOrders.Add(repair);
                await _context.SaveChangesAsync();

                var workTypes = await _context.WorkTypes
                    .Where(wt => workTypeIds.Contains(wt.Id))
                    .ToListAsync();

                var totalCost = 0m;
                var totalDuration = 0;

                foreach (var workType in workTypes)
                {
                    var repairWork = new RepairWork
                    {
                        VehicleId = repair.VehicleId,
                        WorkTypeId = workType.Id,
                        Date = repair.ScheduledDate,
                        Description = workType.Description,
                        Cost = workType.Cost,
                        Vehicle = repair.Vehicle,
                        WorkType = workType
                    };

                    repair.RepairWorks.Add(repairWork);
                    totalCost += workType.Cost;
                    totalDuration += workType.DurationMin;
                }

                repair.TotalCost = totalCost;
                repair.EstimatedDurationMin = totalDuration;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Пользователь {UserId} создал заказ на ремонт {RepairId} для транспортного средства {VehicleId}", 
                    userId, repair.Id, repair.VehicleId);

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Vehicles = await _context.Vehicles.Where(v => v.OwnerId == userId).ToListAsync();
            ViewBag.WorkTypes = await _context.WorkTypes.ToListAsync();
            return View(repair);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var repair = await _context.RepairOrders
                .Include(r => r.Vehicle)
                .Include(r => r.RepairWorks)
                    .ThenInclude(rw => rw.WorkType)
                .FirstOrDefaultAsync(r => r.Id == id && r.Vehicle.OwnerId == userId);

            if (repair == null)
            {
                return NotFound();
            }

            return View(repair);
        }
    }
} 