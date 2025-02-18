using DynamicTimeTable.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamicTimeTable.Controllers
{
    public class TimetableController : Controller
    {
        public IActionResult Index()
        {
            return View(new TimetableModel());
        }
        [HttpPost]
        public IActionResult GenerateTimetable(TimetableModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            ViewBag.TotalHoursForWeek = model.TotalHoursForWeek;
            model.SubjectHoursList = new List<SubjectHoursModel>(model.TotalSubjects);
            return View("SubjectHours", model);
        }
        [HttpPost]
        public IActionResult CreateTimetable(TimetableModel model)
        {
            int totalEnteredHours = model.SubjectHoursList.Sum(s => s.Hours);

            if (totalEnteredHours != model.TotalHoursForWeek)
            {
                ModelState.AddModelError("", "Total subject hours must match the total hours for the week.");
                return View("SubjectHours", model);
            }

            model.GeneratedTimetable = GenerateSchedule(model);
            return View("Timetable", model);
        }
        private List<List<string>> GenerateSchedule(TimetableModel model)
        {
            var timetable = new List<List<string>>();
            var random = new Random();

            // Create a dictionary to track remaining hours for each subject
            var subjectHours = model.SubjectHoursList.ToDictionary(s => s.SubjectName, s => s.Hours);

            // Get all subjects
            var allSubjects = subjectHours.Keys.ToList();

            // Initialize timetable
            for (int day = 0; day < model.WorkingDays; day++)
            {
                timetable.Add(new List<string>());
            }

            // Fill timetable while ensuring no subject repeats in a single day
            for (int slot = 0; slot < model.SubjectsPerDay; slot++)
            {
                for (int day = 0; day < model.WorkingDays; day++)
                {
                    var usedSubjects = new HashSet<string>(timetable[day]); // Prevent repetition

                    // Get available subjects that have remaining hours and are not used today
                    var availableSubjects = allSubjects
                        .Where(s => subjectHours[s] > 0 && !usedSubjects.Contains(s))
                        .ToList();

                    if (!availableSubjects.Any()) continue; // Skip if no valid subjects

                    // Pick a random subject
                    var selectedSubject = availableSubjects[random.Next(availableSubjects.Count)];

                    // Assign subject and reduce remaining hours
                    timetable[day].Add(selectedSubject);
                    subjectHours[selectedSubject]--;
                }
            }

            return timetable;
        }



    }
}
