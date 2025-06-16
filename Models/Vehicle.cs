using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CarManagment.Models;

public class Vehicle
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string LicensePlate { get; set; }

    [Required]
    public required string Brand { get; set; }

    [Required]
    public required string Model { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    public required string VIN { get; set; }

    [Required]
    public required string OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public required ApplicationUser Owner { get; set; }

    public ICollection<RepairWork> Repairs { get; set; } = new List<RepairWork>();
}

