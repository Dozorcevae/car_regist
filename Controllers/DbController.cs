using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarManagment.Data;
using CarManagment.Models;
using CarManagment.Services;
using System.Security.Claims;

namespace CarManagment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DbController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILoggingService _logger;

        public DbController(AppDbContext context, ILoggingService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicles = await _context.Vehicles
                .Include(v => v.Owner)
                .Where(v => v.OwnerId == userId)
                .ToListAsync();

            _logger.LogInformation("Пользователь {UserId} получил список своих транспортных средств", userId ?? "unknown");
            return Ok(vehicles);
        }

        [HttpPost("vehicles")]
        public async Task<IActionResult> AddVehicle([FromBody] Vehicle vehicle)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            vehicle.OwnerId = userId;
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Пользователь {UserId} добавил новое транспортное средство {VehicleId}", userId ?? "unknown", vehicle.Id);
            return Ok(vehicle);
        }

        [HttpPost("repairs")]
        public async Task<IActionResult> AddRepair([FromBody] RepairWork repair)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == repair.VehicleId && v.OwnerId == userId);

            if (vehicle == null)
            {
                _logger.LogWarning("Попытка доступа к чужому транспортному средству {VehicleId} пользователем {UserId}", repair.VehicleId, userId ?? "unknown");
                return Forbid();
            }

            repair.Date = DateTime.UtcNow;
            _context.Repairs.Add(repair);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Пользователь {UserId} добавил запись о ремонте {RepairId} для транспортного средства {VehicleId}", 
                userId ?? "unknown", repair.Id, vehicle.Id);
            return Ok(repair);
        }

        [HttpGet("repairs/{vehicleId}")]
        public async Task<IActionResult> GetRepairs(int vehicleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.OwnerId == userId);

            if (vehicle == null)
            {
                _logger.LogWarning("Попытка доступа к ремонтам чужого транспортного средства {VehicleId} пользователем {UserId}", 
                    vehicleId, userId ?? "unknown");
                return Forbid();
            }

            var repairs = await _context.Repairs
                .Include(r => r.WorkType)
                .Where(r => r.VehicleId == vehicleId)
                .ToListAsync();

            _logger.LogInformation("Пользователь {UserId} получил список ремонтов для транспортного средства {VehicleId}", 
                userId ?? "unknown", vehicleId);
            return Ok(repairs);
        }

        [HttpGet("worktypes")]
        public async Task<IActionResult> GetWorkTypes()
        {
            var workTypes = await _context.WorkTypes.ToListAsync();
            _logger.LogInformation("Получен список типов работ");
            return Ok(workTypes);
        }
    }
}