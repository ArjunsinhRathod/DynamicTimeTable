using System.ComponentModel.DataAnnotations;

namespace DynamicTimeTable.Models
{
    public class TimetableModel
    {
        [Required]
        [Range(1, 7, ErrorMessage = "Enter a value between 1 and 7")]
        public int WorkingDays { get; set; }

        [Required]
        [Range(1, 8, ErrorMessage = "Enter a value less than 9")]
        public int SubjectsPerDay { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total subjects must be positive")]
        public int TotalSubjects { get; set; }

        public int TotalHoursForWeek => WorkingDays * SubjectsPerDay;

        public List<SubjectHoursModel> SubjectHoursList { get; set; } = new List<SubjectHoursModel>();
        public List<List<string>>? GeneratedTimetable { get; set; }
    }
    public class SubjectHoursModel
    {
        public string? SubjectName { get; set; }
        public int Hours { get; set; }
    }
}
