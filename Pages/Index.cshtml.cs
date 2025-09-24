using CampusLearn.Repositories;
using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Mvc;
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


        //list to hold the top tutors and display on the homepage
        public List<TutorCard> TopTutors { get; set; } = new List<TutorCard>();
        public void OnGet()
        {
            //get the top tutors from the service
            TopTutors = _tutorService.GetTopTutors();
        }
    }
}
