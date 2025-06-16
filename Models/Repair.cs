using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarManagment.Models
{
    public class Repair
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public required Vehicle Vehicle { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public int EstimatedDurationMin { get; set; }

        [Required]
        public RepairStatus Status { get; set; } = RepairStatus.Pending;

        [Required]
        public required string Notes { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        public ICollection<RepairWork> RepairWorks { get; set; } = new List<RepairWork>();
    }

    public enum RepairStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }
}
