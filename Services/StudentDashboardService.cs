using CampusLearn.Repositories;

namespace CampusLearn.Services
{
    public class StudentDashboardService
    {
        private readonly StudentDashboardRepository _studentDashboardRepository;

        public StudentDashboardService(StudentDashboardRepository studentDashboardRepository)
        {
            _studentDashboardRepository = studentDashboardRepository;
        }

        //method to get the bookings for a student and apply business rules 
        public List<AppointmentTable> GetAppointmentTables(string studentId)
        {
            // get the data from Repository 
            var appointmentTables = _studentDashboardRepository.GetBookings(studentId);

            var now = DateTime.Now;
            var upcoming = appointmentTables
                .Where(a => a.AppointmentDate >= now)
                .OrderBy(a => a.AppointmentDate);

            var past = appointmentTables
                .Where(a => a.AppointmentDate < now)
                .OrderByDescending(a => a.AppointmentDate);

            return upcoming.Concat(past).ToList();
        }

        //create method that will count number of sessions booked
        public int BookedSessionsCount(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetBookedSessionsCount(studentId);
            return totalSessionsCount;
        }

        //create method that will count number of sessions booked
        public int UpcomingSessionsCount(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetUpComingSessions(studentId);
            return totalSessionsCount;
        }

        //create method that will count completed sessions
        public int CompletedSessions(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetCompletedSessions(studentId);
            return totalSessionsCount;
        }

    }
}
