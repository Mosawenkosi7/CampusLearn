using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CampusLearn.Services;
using CampusLearn.Models;

namespace CampusLearn.Pages.Tutor
{
    public class AllTutorsModel : PageModel
    {
        private readonly TutorService _tutorService;
        public AllTutorsModel(TutorService tutorService)
        {
            _tutorService = tutorService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ModuleCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? YearOfStudy { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public PagedResult<TutorCard> Tutors { get; set; } = new PagedResult<TutorCard>();

        public void OnGet()
        {
            var query = new TutorQuery
            {
                Page = PageNumber,
                PageSize = 8,
                Search = Search,
                ModuleCode = ModuleCode,
                YearOfStudy = YearOfStudy
            };
            Tutors = _tutorService.GetTutors(query);
        }
    }
}

