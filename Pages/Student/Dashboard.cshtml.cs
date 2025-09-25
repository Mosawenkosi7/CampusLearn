using CampusLearn.Repositories;
using CampusLearn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusLearn.Pages.Student
{
    public class DashboardModel : PageModel
    {
        private readonly StudentDashboardService _studentDashboardService;

        //SET THE ID
       

        public DashboardModel(StudentDashboardService studentDashboardService)
        {
            _studentDashboardService = studentDashboardService;
        }


        //Number of sessions booked 
        public int totalSessions { get; set; }
        public int totalUpcomingSessions { get; set; }

        public int completedSessions { get; set; }
        //list to store bookings (paged)
        public List<AppointmentTable> appointmentTables = new List<AppointmentTable>();

        // pagination
        public int PageSize { get; } = 5;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        

        public void OnGet(int p = 1)
        {
            HttpContext.Session.SetString("role", "Student");
            HttpContext.Session.SetString("studentId", "ST12345678");
            var studentId = HttpContext.Session.GetString("studentId");
            if(string.IsNullOrEmpty(studentId))
            {
                RedirectToPage("/Authentication/LogIn");
                return; // Important: exit early if no student ID
            }

            // all data ordered from service - studentId is guaranteed non-null here
            var allAppointments = _studentDashboardService.GetAppointmentTables(studentId);

            // pagination setup
            CurrentPage = p < 1 ? 1 : p;
            TotalPages = (int)Math.Ceiling(allAppointments.Count / (double)PageSize);
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }

            appointmentTables = allAppointments
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();


            //set the total sessions booked 
            totalSessions = _studentDashboardService.BookedSessionsCount(studentId);

            totalUpcomingSessions = _studentDashboardService.UpcomingSessionsCount(studentId);

            completedSessions = _studentDashboardService.CompletedSessions(studentId);
        }

      
    }
}
