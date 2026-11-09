using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_8.Models
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public int SubjectID { get; set; }
        [Required]
        public string SubjectName { get; set; } = string.Empty;
        public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    }
}