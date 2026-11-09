using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_8.Models
{
    [Table("Positions")]
    public class Position
    {
        [Key]
        public int PositionID { get; set; }
        [Required]
        public string PositionName { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}