using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_8.Models
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int TeacherID { get; set; }
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string? Workplace { get; set; }
        public int PositionID { get; set; }
        public string? HomeAddress { get; set; }
        public string? Characteristic { get; set; }

        [ForeignKey("PositionID")]
        public virtual Position? Position { get; set; }
        public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    }
}