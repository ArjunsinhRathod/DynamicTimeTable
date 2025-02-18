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
            var subjects = new List<string>();

            foreach (var subject in model.SubjectHoursList)
            {
                subjects.AddRange(Enumerable.Repeat(subject.SubjectName, subject.Hours));
            }

            var random = new Random();
            subjects = subjects.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < model.SubjectsPerDay; i++)
            {
                var row = new List<string>();
                for (int j = 0; j < model.WorkingDays; j++)
                {
                    row.Add(subjects.FirstOrDefault() ?? "N/A");
                    subjects.RemoveAt(0);
                }
                timetable.Add(row);
            }

            return timetable;
        }
    }
}
