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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                ViewBag.TotalHoursForWeek = model.TotalHoursForWeek;
                model.SubjectHoursList = new List<SubjectHoursModel>((int)model.TotalSubjects);
                return View("SubjectHours", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
            
        [HttpPost]
        public IActionResult CreateTimetable(TimetableModel model)
        {
            try
            {
                int totalEnteredHours = model.SubjectHoursList.Sum(s => s.Hours);

                if (totalEnteredHours != model.TotalHoursForWeek)
                {
                    ModelState.AddModelError("", "Total subject hours must match the total hours for the week.");
                    ViewBag.TotalHoursForWeek = model.TotalHoursForWeek;
                    return View("SubjectHours", model);
                }

                model.GeneratedTimetable = GenerateSchedule(model);
                return View("Timetable", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            
        }
        //private List<List<string>> GenerateSchedule(TimetableModel model)
        //{
        //    var timetable = new List<List<string>>();
        //    var random = new Random();

        //    var subjectHours = model.SubjectHoursList.ToDictionary(s => s.SubjectName, s => s.Hours);

        //    var allSubjects = subjectHours.Keys.ToList();

        //    for (int i = 0; i < model.SubjectsPerDay; i++)
        //    {
        //        timetable.Add(new List<string>(new string[(int)model.WorkingDays]));
        //    }

        //    // Fill the timetable column-wise (day-by-day for each slot)
        //    for (int day = 0; day < model.WorkingDays; day++)
        //    {
        //        var usedSubjects = new HashSet<string>(); // Prevent repetition within the same day

        //        for (int slot = 0; slot < model.SubjectsPerDay; slot++)
        //        {
        //            var availableSubjects = allSubjects
        //                .Where(s => subjectHours[s] > 0 && !usedSubjects.Contains(s))
        //                .ToList();

        //            if (!availableSubjects.Any()) continue; // Skip if no valid subjects

        //            var selectedSubject = availableSubjects[random.Next(availableSubjects.Count)];

        //            timetable[slot][day] = selectedSubject;

        //            usedSubjects.Add(selectedSubject);
        //            subjectHours[selectedSubject]--;
        //        }
        //    }

        //    return timetable;
        //}

        private List<List<string>> GenerateSchedule(TimetableModel model)
        {
            var timetable = new List<List<string>>();
            var random = new Random();

            var subjectHours = model.SubjectHoursList.ToDictionary(s => s.SubjectName, s => s.Hours);

            var allSubjects = subjectHours.Keys.ToList();

            for (int i = 0; i < model.SubjectsPerDay; i++)
            {
                timetable.Add(new List<string>(new string[(int)model.WorkingDays]));
            }

            for (int day = 0; day < model.WorkingDays; day++)
            {
                var usedSubjects = new HashSet<string>();

                for (int slot = 0; slot < model.SubjectsPerDay; slot++)
                {
                    var availableSubjects = allSubjects
                        .Where(s => subjectHours[s] > 0 && !usedSubjects.Contains(s))
                        .ToList();

                    if (!availableSubjects.Any())
                    {
                        availableSubjects = allSubjects.Where(s => subjectHours[s] > 0).ToList();
                    }

                    if (!availableSubjects.Any()) continue; 

                    var selectedSubject = availableSubjects[random.Next(availableSubjects.Count)];

                    timetable[slot][day] = selectedSubject;

                    usedSubjects.Add(selectedSubject);
                    subjectHours[selectedSubject]--;
                }
            }

            return timetable;
        }


    }
}
