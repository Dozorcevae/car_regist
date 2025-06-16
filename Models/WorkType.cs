using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarManagment.Models  
{
    public class WorkType
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = null!;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        
        [Required]
        public int DurationMin { get; set; }
        
        [Required]
        public string Description { get; set; } = null!;

        public ICollection<RepairWork> Repairs { get; set; } = new List<RepairWork>();
    }
}
