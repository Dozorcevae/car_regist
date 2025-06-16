using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarManagment.Models
{
    public class RepairWork
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public required Vehicle Vehicle { get; set; }

        [Required]
        public int WorkTypeId { get; set; }
        [ForeignKey("WorkTypeId")]
        public required WorkType WorkType { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        public int? RepairId { get; set; }
        public Repair? Repair { get; set; }
    }
}

