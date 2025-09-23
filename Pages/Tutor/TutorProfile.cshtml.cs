using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CampusLearn.Services;
using CampusLearn.Models;

namespace CampusLearn.Pages.Tutor
{
    public class TutorProfileModel : PageModel
    {
        private readonly TutorService _tutorService;

        public TutorProfileModel(TutorService tutorService)
        {
            _tutorService = tutorService;
        }

        public TutorCard? Tutor { get; set; }
        public List<TutorAvailabilityView> Availabilities { get; set; } = new List<TutorAvailabilityView>();

        public IActionResult OnGet(int? tutorId)
        {
            if (tutorId == null)
            {
                return NotFound();
            }

            Tutor = _tutorService.GetTutorProfile(tutorId.Value);
            if (Tutor == null)
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
