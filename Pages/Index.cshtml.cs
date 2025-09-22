using CampusLearn.Services;
using CampusLearn.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusLearn.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TutorService _tutorService;
        
        public IndexModel(TutorService tutorService)
        {
            _tutorService = tutorService;
        }

        public List<TutorCard> TopTutors { get; set; } = new List<TutorCard>();
        
        public void OnGet()
        {
            TopTutors = _tutorService.GetTopTutors();
        }
    }
}
