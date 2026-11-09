using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_8.Models
{
    [Table("TeacherSubjects")]
    public class TeacherSubject
    {
        public int TeacherID { get; set; }
        public int SubjectID { get; set; }
        public int HoursRead { get; set; }

        [ForeignKey("TeacherID")]
        public virtual Teacher? Teacher { get; set; }

        [ForeignKey("SubjectID")]
        public virtual Subject? Subject { get; set; }
    }
}