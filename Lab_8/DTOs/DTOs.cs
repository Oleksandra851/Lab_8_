using System.Collections.Generic;

namespace Lab_8.DTOs
{
    public class TeacherDto
    {
        public int TeacherID { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string? Workplace { get; set; }
        public int PositionID { get; set; }
        public string? PositionName { get; set; }
        public string? HomeAddress { get; set; }
        public string? Characteristic { get; set; }
        public List<SubjectDto> Subjects { get; set; } = new List<SubjectDto>();
    }

    public class SubjectDto
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int HoursRead { get; set; }
    }

    public class SubjectAssignmentDto
    {
        public int SubjectID { get; set; }
        public int HoursRead { get; set; }
    }

    public class TeacherCreateUpdateDto
    {
        public int TeacherID { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? Phone { get; set; }
        public string? Workplace { get; set; }
        public int PositionID { get; set; }
        public string? HomeAddress { get; set; }
        public string? Characteristic { get; set; }
        public List<SubjectAssignmentDto> SelectedSubjects { get; set; } = new List<SubjectAssignmentDto>();
    }
}