using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CampusLearn.Services;
using CampusLearn.Repositories;

namespace CampusLearn.Pages.Tutor
{
    public class TutorProfileModel : PageModel
    {
        private readonly TutorService _tutorService;

        public TutorProfileModel(TutorService tutorService)
        {
            _tutorService = tutorService;
        }

        public TutorCard? TutorProfile { get; set; }
        public List<CampusLearn.Repositories.TutorAvailability> Availabilities { get; set; } = new List<CampusLearn.Repositories.TutorAvailability>();

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TutorProfile = _tutorService.GetTutorProfile(id.Value);
            if (TutorProfile == null)
            {
                return NotFound();
            }

            Availabilities = _tutorService.GetTutorAvailability(id.Value);
            return Page();
        }

        public IActionResult OnPostBook(int availabilityId, int tutorId)
        {
            var success = _tutorService.BookAvailability(availabilityId);
            if (success)
            {
                TempData["Message"] = "Session booked successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to book session. It may already be booked.";
            }

            return RedirectToPage(new { id = tutorId });
        }
    }
}
